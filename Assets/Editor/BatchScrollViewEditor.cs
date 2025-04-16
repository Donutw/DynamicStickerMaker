using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class BatchScrollViewEditor : EditorWindow
{
    private float newScrollSensitivity = 10f;
    private Color newScrollbarBackgroundColor = new Color(1f, 1f, 1f, 0f); // 全透明
    private Color newHandleColor = new Color(1f, 0.97f, 0.8f, 0.5f);       // 奶黄色 + 50% 透明度

    [MenuItem("Tools/批量修改 ScrollView 设置")]
    public static void ShowWindow()
    {
        GetWindow<BatchScrollViewEditor>("批量修改 ScrollView 设置");
    }


    private void OnGUI()
    {
        GUILayout.Label("批量修改场景中的 ScrollRect & Scrollbar 外观", EditorStyles.boldLabel);

        // 鼠标滚动灵敏度
        newScrollSensitivity = EditorGUILayout.FloatField("Scroll Sensitivity", newScrollSensitivity);

        // 滚动条背景色（这里默认全透明）
        newScrollbarBackgroundColor = EditorGUILayout.ColorField("Scrollbar 背景色", newScrollbarBackgroundColor);

        // Handle 滑块颜色（奶黄色、半透明）
        newHandleColor = EditorGUILayout.ColorField("Handle 颜色", newHandleColor);

        if (GUILayout.Button("应用到场景中的所有 Scroll View"))
        {
            ApplyToAllScrollViews();
        }
    }

    private void ApplyToAllScrollViews()
    {
        // 修改所有场景中的 ScrollRect
        ScrollRect[] scrollRects = FindObjectsOfType<ScrollRect>();
        foreach (ScrollRect sr in scrollRects)
        {
            Undo.RecordObject(sr, "Batch Modify ScrollRect");
            sr.scrollSensitivity = newScrollSensitivity; // 设置鼠标滚动灵敏度
            EditorUtility.SetDirty(sr);

            // 如果它有垂直滚动条，改一下滚动条背景与 Handle 的颜色
            if (sr.verticalScrollbar != null)
            {
                // 1) 滚动条背景变透明
                Image scrollbarImage = sr.verticalScrollbar.GetComponent<Image>();
                if (scrollbarImage != null)
                {
                    Undo.RecordObject(scrollbarImage, "Batch Modify Scrollbar Background");
                    scrollbarImage.color = newScrollbarBackgroundColor;
                    EditorUtility.SetDirty(scrollbarImage);
                }

                // 2) Handle 的颜色改为奶黄色半透明
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

        Debug.Log("批量修改完成！");
    }
}
