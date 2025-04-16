using UnityEngine;

public class ClearAllChildItem : MonoBehaviour
{
    // 按钮 OnClick 调用此方法
    public void ClearAll()
    {
        // 查找场景中所有 ChildItem 组件
        ChildItem[] items = FindObjectsOfType<ChildItem>();
        foreach (ChildItem item in items)
        {
            Destroy(item.gameObject);
        }
        Debug.Log("已清除所有 ChildItem 对象");
    }
}
