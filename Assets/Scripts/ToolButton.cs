using UnityEngine;

public class ToolButton : MonoBehaviour
{
    // 水平翻转：绕 Y 轴旋转 180° 并切换水平翻转标志
    public void HorizontalFlip()
    {
        ChildItem selectedItem = SelectionManager.Instance.GetSelectedItem();
        if (selectedItem != null)
        {
            Transform t = selectedItem.transform;
            // 切换水平翻转状态
            selectedItem.isFlippedHorizontally = !selectedItem.isFlippedHorizontally;
            // 绕 Y 轴旋转 180°（注意：这里用 Space.Self 保证是在局部空间中旋转）
            t.Rotate(0, 180, 0, Space.Self);
        }
    }

    // 竖直翻转：绕 X 轴旋转 180° 并切换竖直翻转标志
    public void VerticalFlip()
    {
        ChildItem selectedItem = SelectionManager.Instance.GetSelectedItem();
        if (selectedItem != null)
        {
            Transform t = selectedItem.transform;
            selectedItem.isFlippedVertically = !selectedItem.isFlippedVertically;
            // 绕 X 轴旋转 180°
            t.Rotate(180, 0, 0, Space.Self);
        }
    }

    // 上移一层：只对同一父面板上的元素进行排序调整
    public void BringForward()
    {
        ChildItem selectedItem = SelectionManager.Instance.GetSelectedItem();
        if (selectedItem != null)
        {
            Transform t = selectedItem.transform;
            if (t.parent != null)
            {
                int currentIndex = t.GetSiblingIndex();
                int maxIndex = t.parent.childCount - 1;
                if (currentIndex < maxIndex)
                {
                    t.SetSiblingIndex(currentIndex + 1);
                }
            }
        }
    }

    // 下移一层：只对同一父面板上的元素进行排序调整
    public void SendBackward()
    {
        ChildItem selectedItem = SelectionManager.Instance.GetSelectedItem();
        if (selectedItem != null)
        {
            Transform t = selectedItem.transform;
            if (t.parent != null)
            {
                int currentIndex = t.GetSiblingIndex();
                if (currentIndex > 0)
                {
                    t.SetSiblingIndex(currentIndex - 1);
                }
            }
        }
    }
}
