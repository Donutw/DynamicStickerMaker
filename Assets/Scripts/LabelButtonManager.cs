using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LabelButtonManager : MonoBehaviour
{
    // 内部类，用于保存每个按钮的原始数据
    private class ButtonOriginalData
    {
        public Transform originalParent; // 按钮原始父物体（例如 panel）
        public int siblingIndex;         // 在 panel 中的原始排序位置
    }

    // 存储所有 tag 为 "label" 的按钮及其原始数据
    private Dictionary<Button, ButtonOriginalData> buttonData = new Dictionary<Button, ButtonOriginalData>();

    void Start()
    {
        // 使用 Resources.FindObjectsOfTypeAll 来查找所有 GameObject（包括 inactive 的）
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject go in allObjects)
        {
            // 过滤：只处理标签为 "label" 且在已加载的场景中的对象
            if (go.CompareTag("label") && go.scene.isLoaded)
            {
                Button btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    ButtonOriginalData data = new ButtonOriginalData();
                    data.originalParent = btn.transform.parent;
                    data.siblingIndex = btn.transform.GetSiblingIndex();
                    buttonData[btn] = data;
                }
                else
                {
                    Debug.LogWarning($"对象 {go.name} 被标记为 'label' 但没有 Button 组件。");
                }
            }
        }
    }

    void Update()
    {
        foreach (var kvp in buttonData)
        {
            Button btn = kvp.Key;
            ButtonOriginalData data = kvp.Value;

            // 检查按钮的 interactable 状态
            if (!btn.interactable)
            {
                // 当按钮被禁用时，移动到其父物体的父物体中（如果存在）
                if (data.originalParent != null && data.originalParent.parent != null)
                {
                    Transform targetParent = data.originalParent.parent;
                    if (btn.transform.parent != targetParent)
                    {
                        btn.transform.SetParent(targetParent, false);
                    }
                    // 设置到目标父物体的最上层（最后一个子对象）
                    btn.transform.SetAsLastSibling();
                }
                else
                {
                    Debug.LogWarning($"按钮 {btn.gameObject.name} 没有找到合适的父物体的父物体。");
                }
            }
            else
            {
                // 当按钮启用时，将其还原回原 panel 中，并恢复原来的排序位置
                if (btn.transform.parent != data.originalParent)
                {
                    btn.transform.SetParent(data.originalParent, false);
                    btn.transform.SetSiblingIndex(data.siblingIndex);
                }
            }
        }
    }
}
