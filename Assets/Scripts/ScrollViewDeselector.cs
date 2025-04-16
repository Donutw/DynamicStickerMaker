using UnityEngine;
using UnityEngine.UI;

public class ScrollViewDeselector : MonoBehaviour
{
    // 需要挂在 ScrollRect 同一物体上
    private ScrollRect scrollRect;
    private RectTransform rectTransform;
    private Canvas canvas; // 用于判断鼠标位置转换（如果 Canvas 是 ScreenSpace - Overlay，则可设为 null）

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        rectTransform = scrollRect.GetComponent<RectTransform>();

        // 如果你的 Canvas 是 ScreenSpace - Camera，则需要指定 canvas.worldCamera
        // 如果是 ScreenSpace - Overlay，则传入 null即可
        canvas = GetComponentInParent<Canvas>();
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void OnScrollValueChanged(Vector2 value)
    {
        // 当 ScrollRect 滚动位置变化时，检测鼠标是否在这个区域内
        if (IsPointerOverScrollRect())
        {
            SelectionManager.Instance?.Deselect();
        }
    }

    private bool IsPointerOverScrollRect()
    {
        // 如果 Canvas 是 Overlay，可以传入 null
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, cam);
    }
}
