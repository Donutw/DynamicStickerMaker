using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class BatchScrollViewEditor : EditorWindow
{
    private float newScrollSensitivity = 10f;
    private Color newScrollbarBackgroundColor = new Color(1f, 1f, 1f, 0f); // ȫ͸��
    private Color newHandleColor = new Color(1f, 0.97f, 0.8f, 0.5f);       // �̻�ɫ + 50% ͸����

    [MenuItem("Tools/�����޸� ScrollView ����")]
    public static void ShowWindow()
    {
        GetWindow<BatchScrollViewEditor>("�����޸� ScrollView ����");
    }


    private void OnGUI()
    {
        GUILayout.Label("�����޸ĳ����е� ScrollRect & Scrollbar ���", EditorStyles.boldLabel);

        // ������������
        newScrollSensitivity = EditorGUILayout.FloatField("Scroll Sensitivity", newScrollSensitivity);

        // ����������ɫ������Ĭ��ȫ͸����
        newScrollbarBackgroundColor = EditorGUILayout.ColorField("Scrollbar ����ɫ", newScrollbarBackgroundColor);

        // Handle ������ɫ���̻�ɫ����͸����
        newHandleColor = EditorGUILayout.ColorField("Handle ��ɫ", newHandleColor);

        if (GUILayout.Button("Ӧ�õ������е����� Scroll View"))
        {
            ApplyToAllScrollViews();
        }
    }

    private void ApplyToAllScrollViews()
    {
        // �޸����г����е� ScrollRect
        ScrollRect[] scrollRects = FindObjectsOfType<ScrollRect>();
        foreach (ScrollRect sr in scrollRects)
        {
            Undo.RecordObject(sr, "Batch Modify ScrollRect");
            sr.scrollSensitivity = newScrollSensitivity; // ����������������
            EditorUtility.SetDirty(sr);

            // ������д�ֱ����������һ�¹����������� Handle ����ɫ
            if (sr.verticalScrollbar != null)
            {
                // 1) ������������͸��
                Image scrollbarImage = sr.verticalScrollbar.GetComponent<Image>();
                if (scrollbarImage != null)
                {
                    Undo.RecordObject(scrollbarImage, "Batch Modify Scrollbar Background");
                    scrollbarImage.color = newScrollbarBackgroundColor;
                    EditorUtility.SetDirty(scrollbarImage);
                }

                // 2) Handle ����ɫ��Ϊ�̻�ɫ��͸��
                Transform handleTransform = sr.verticalScrollbar.transform.Find("Sliding Area/Handle");
                if (handleTransform != null)
                {
                    Image handleImage = handleTransform.GetComponent<Image>();
                    if (handleImage != null)
                    {
                        Undo.RecordObject(handleImage, "Batch Modify Handle Color");
                        handleImage.color = newHandleColor;
                        EditorUtility.SetDirty(handleImage);
                    }
                }
            }
        }

        Debug.Log("�����޸���ɣ�");
    }
}
