using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine.UI;

public class CreateAnimationsFromInputFolders : EditorWindow
{
    // 输入的主文件夹路径：里面包含多个子文件夹，每个子文件夹包含动画帧图片
    private string inputFolderPath = "Assets/YourInputFolder";
    // 输出的目标文件夹路径：所有生成的 AnimationClip 和 AnimatorController 都放在这里
    private string outputFolderPath = "Assets/YourOutputFolder";
    // 帧率（每秒播放多少帧）
    private float frameRate = 10f;
    // 是否采用往返循环（Ping Pong）
    private bool isPingPong = false;

    [MenuItem("Tools/Create Animations & Animators From Input Folder")]
    public static void ShowWindow()
    {
        GetWindow<CreateAnimationsFromInputFolders>("生成动画与 Animator");
    }

    private void OnGUI()
    {
        GUILayout.Label("从输入文件夹中批量生成 AnimationClip 和 Animator Controller", EditorStyles.boldLabel);
        inputFolderPath = EditorGUILayout.TextField("输入文件夹路径", inputFolderPath);
        outputFolderPath = EditorGUILayout.TextField("输出文件夹路径", outputFolderPath);
        frameRate = EditorGUILayout.FloatField("帧率 (帧/秒)", frameRate);
        isPingPong = EditorGUILayout.Toggle("Ping Pong Mode", isPingPong);

        if (GUILayout.Button("生成所有子文件夹的动画"))
        {
            ProcessInputFolder();
        }
    }

    private void ProcessInputFolder()
    {
        if (!AssetDatabase.IsValidFolder(inputFolderPath))
        {
            Debug.LogError("输入文件夹路径无效: " + inputFolderPath);
            return;
        }

        // 如果输出文件夹不存在则创建
        if (!AssetDatabase.IsValidFolder(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
            AssetDatabase.Refresh();
        }

        // 获取输入文件夹下所有的直接子文件夹
        string[] subfolderGuids = AssetDatabase.FindAssets("t:Folder", new string[] { inputFolderPath });
        foreach (string guid in subfolderGuids)
        {
            string subfolderPath = AssetDatabase.GUIDToAssetPath(guid);
            
            // 以子文件夹的名字作为动画名称
            string animationName = Path.GetFileName(subfolderPath);
            ProcessSubfolder(subfolderPath, animationName);
        }
        AssetDatabase.SaveAssets();
    }

    private void ProcessSubfolder(string folderPath, string animationName)
    {
        // 在子文件夹中查找所有 Sprite
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new string[] { folderPath });
        var sprites = guids.Select(guid => AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid)))
                           .OrderBy(sprite => sprite.name)
                           .ToArray();
        if (sprites.Length == 0)
        {
            Debug.LogWarning("文件夹 " + folderPath + " 中没有找到 Sprite");
            return;
        }

        // 创建 AnimationClip 并设置帧率
        AnimationClip clip = new AnimationClip();
        clip.frameRate = frameRate;

        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        // 这里以 UI Image 组件为例进行动画绑定
        spriteBinding.type = typeof(Image);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] keyFrames;

        if (!isPingPong)
        {
            // 顺序循环：例如 1,2,3,1
            keyFrames = new ObjectReferenceKeyframe[sprites.Length + 1];
            for (int i = 0; i < sprites.Length; i++)
            {
                keyFrames[i] = new ObjectReferenceKeyframe();
                keyFrames[i].time = i / frameRate;
                keyFrames[i].value = sprites[i];
            }
            // 添加额外关键帧：复制第一帧，保证循环平滑
            keyFrames[sprites.Length] = new ObjectReferenceKeyframe();
            keyFrames[sprites.Length].time = sprites.Length / frameRate;
            keyFrames[sprites.Length].value = sprites[0];
        }
        else
        {
            // 往返循环：例如 1,2,3,2,1
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
                // 正向序列
                for (int i = 0; i < forwardCount; i++, index++)
                {
                    keyFrames[index] = new ObjectReferenceKeyframe();
                    keyFrames[index].time = index / frameRate;
                    keyFrames[index].value = sprites[i];
                }
                // 反向序列（从倒数第二帧到第一帧）
                for (int i = sprites.Length - 2; i >= 0; i--, index++)
                {
                    keyFrames[index] = new ObjectReferenceKeyframe();
                    keyFrames[index].time = index / frameRate;
                    keyFrames[index].value = sprites[i];
                }
            }
        }

        // 应用关键帧到 AnimationClip
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyFrames);

        // 自动设置 AnimationClip 的循环属性
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

        // 保存 AnimationClip 到输出文件夹，文件名使用子文件夹的名字
        string clipPath = Path.Combine(outputFolderPath, animationName + ".anim");
        AssetDatabase.CreateAsset(clip, clipPath);

        // 创建 Animator Controller，同样使用子文件夹名字作为状态名
        string controllerPath = Path.Combine(outputFolderPath, animationName + ".controller");
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        AnimatorControllerLayer layer = controller.layers[0];
        AnimatorStateMachine stateMachine = layer.stateMachine;

        AnimatorState state = stateMachine.AddState(animationName);
        state.motion = clip;
        stateMachine.defaultState = state;

        Debug.Log("生成动画: " + animationName + "，来自文件夹: " + folderPath);
    }
}
