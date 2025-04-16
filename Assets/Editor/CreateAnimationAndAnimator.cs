using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine.UI;

public class CreateAnimationsFromInputFolders : EditorWindow
{
    // ��������ļ���·�����������������ļ��У�ÿ�����ļ��а�������֡ͼƬ
    private string inputFolderPath = "Assets/YourInputFolder";
    // �����Ŀ���ļ���·�����������ɵ� AnimationClip �� AnimatorController ����������
    private string outputFolderPath = "Assets/YourOutputFolder";
    // ֡�ʣ�ÿ�벥�Ŷ���֡��
    private float frameRate = 10f;
    // �Ƿ��������ѭ����Ping Pong��
    private bool isPingPong = false;

    [MenuItem("Tools/Create Animations & Animators From Input Folder")]
    public static void ShowWindow()
    {
        GetWindow<CreateAnimationsFromInputFolders>("���ɶ����� Animator");
    }

    private void OnGUI()
    {
        GUILayout.Label("�������ļ������������� AnimationClip �� Animator Controller", EditorStyles.boldLabel);
        inputFolderPath = EditorGUILayout.TextField("�����ļ���·��", inputFolderPath);
        outputFolderPath = EditorGUILayout.TextField("����ļ���·��", outputFolderPath);
        frameRate = EditorGUILayout.FloatField("֡�� (֡/��)", frameRate);
        isPingPong = EditorGUILayout.Toggle("Ping Pong Mode", isPingPong);

        if (GUILayout.Button("�����������ļ��еĶ���"))
        {
            ProcessInputFolder();
        }
    }

    private void ProcessInputFolder()
    {
        if (!AssetDatabase.IsValidFolder(inputFolderPath))
        {
            Debug.LogError("�����ļ���·����Ч: " + inputFolderPath);
            return;
        }

        // �������ļ��в������򴴽�
        if (!AssetDatabase.IsValidFolder(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
            AssetDatabase.Refresh();
        }

        // ��ȡ�����ļ��������е�ֱ�����ļ���
        string[] subfolderGuids = AssetDatabase.FindAssets("t:Folder", new string[] { inputFolderPath });
        foreach (string guid in subfolderGuids)
        {
            string subfolderPath = AssetDatabase.GUIDToAssetPath(guid);
            
            // �����ļ��е�������Ϊ��������
            string animationName = Path.GetFileName(subfolderPath);
            ProcessSubfolder(subfolderPath, animationName);
        }
        AssetDatabase.SaveAssets();
    }

    private void ProcessSubfolder(string folderPath, string animationName)
    {
        // �����ļ����в������� Sprite
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new string[] { folderPath });
        var sprites = guids.Select(guid => AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid)))
                           .OrderBy(sprite => sprite.name)
                           .ToArray();
        if (sprites.Length == 0)
        {
            Debug.LogWarning("�ļ��� " + folderPath + " ��û���ҵ� Sprite");
            return;
        }

        // ���� AnimationClip ������֡��
        AnimationClip clip = new AnimationClip();
        clip.frameRate = frameRate;

        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        // ������ UI Image ���Ϊ�����ж�����
        spriteBinding.type = typeof(Image);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] keyFrames;

        if (!isPingPong)
        {
            // ˳��ѭ�������� 1,2,3,1
            keyFrames = new ObjectReferenceKeyframe[sprites.Length + 1];
            for (int i = 0; i < sprites.Length; i++)
            {
                keyFrames[i] = new ObjectReferenceKeyframe();
                keyFrames[i].time = i / frameRate;
                keyFrames[i].value = sprites[i];
            }
            // ��Ӷ���ؼ�֡�����Ƶ�һ֡����֤ѭ��ƽ��
            keyFrames[sprites.Length] = new ObjectReferenceKeyframe();
            keyFrames[sprites.Length].time = sprites.Length / frameRate;
            keyFrames[sprites.Length].value = sprites[0];
        }
        else
        {
            // ����ѭ�������� 1,2,3,2,1
            if (sprites.Length == 1)
            {
                keyFrames = new ObjectReferenceKeyframe[1];
                keyFrames[0] = new ObjectReferenceKeyframe();
                keyFrames[0].time = 0;
                keyFrames[0].value = sprites[0];
            }
            else
            {
                int forwardCount = sprites.Length;
                int backwardCount = sprites.Length - 1;
                int totalFrames = forwardCount + backwardCount;
                keyFrames = new ObjectReferenceKeyframe[totalFrames];

                int index = 0;
                // ��������
                for (int i = 0; i < forwardCount; i++, index++)
                {
                    keyFrames[index] = new ObjectReferenceKeyframe();
                    keyFrames[index].time = index / frameRate;
                    keyFrames[index].value = sprites[i];
                }
                // �������У��ӵ����ڶ�֡����һ֡��
                for (int i = sprites.Length - 2; i >= 0; i--, index++)
                {
                    keyFrames[index] = new ObjectReferenceKeyframe();
                    keyFrames[index].time = index / frameRate;
                    keyFrames[index].value = sprites[i];
                }
            }
        }

        // Ӧ�ùؼ�֡�� AnimationClip
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyFrames);

        // �Զ����� AnimationClip ��ѭ������
        SerializedObject serializedClip = new SerializedObject(clip);
        SerializedProperty clipSettings = serializedClip.FindProperty("m_AnimationClipSettings");
        if (clipSettings != null)
        {
            SerializedProperty loopTimeProp = clipSettings.FindPropertyRelative("m_LoopTime");
            if (loopTimeProp != null)
            {
                loopTimeProp.boolValue = true;
            }
        }
        serializedClip.ApplyModifiedProperties();

        // ���� AnimationClip ������ļ��У��ļ���ʹ�����ļ��е�����
        string clipPath = Path.Combine(outputFolderPath, animationName + ".anim");
        AssetDatabase.CreateAsset(clip, clipPath);

        // ���� Animator Controller��ͬ��ʹ�����ļ���������Ϊ״̬��
        string controllerPath = Path.Combine(outputFolderPath, animationName + ".controller");
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        AnimatorControllerLayer layer = controller.layers[0];
        AnimatorStateMachine stateMachine = layer.stateMachine;

        AnimatorState state = stateMachine.AddState(animationName);
        state.motion = clip;
        stateMachine.defaultState = state;

        Debug.Log("���ɶ���: " + animationName + "�������ļ���: " + folderPath);
    }
}
