using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MotherItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject child;
    private RectTransform draggingPanel;
    private CanvasGroup canvasGroup;
    private RectTransform drawingPanel;

    private Animator animator;  // 用于控制动画
    private bool isMouseOver = false;  // 是否鼠标悬停
    private bool isDragging = false;  // 是否正在拖拽

    private string defaultAnimationClipName;  // 用来保存默认动画的名字

    private void Start()
    {
        // 自动获取当前场景中的 DrawingPanel
        draggingPanel = GameObject.Find("DraggingPanel").GetComponent<RectTransform>();
        drawingPanel = GameObject.Find("DrawingPanel").GetComponent<RectTransform>();

        // 获取 Animator 组件
        animator = GetComponent<Animator>();

        // 获取默认的动画片段名称（如果有动画片段的话）
        if (animator != null)
        {
            // 获取当前动画控制器的所有动画片段
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

            // 如果当前有动画片段，则取第一个片段的名称
            if (clipInfo.Length > 0)
            {
                defaultAnimationClipName = clipInfo[0].clip.name;
            }
        }

        // 确保一开始动画是静态的（回到第一帧）
        if (animator != null && !string.IsNullOrEmpty(defaultAnimationClipName))
        {
            animator.speed = 0;  // 停止动画
            animator.Play(defaultAnimationClipName, 0, 0f);  // 回到第一帧
        }
    }

    private void Update()
    {
        // 如果正在拖拽，则强制停止动画
        if (isDragging)
        {
            if (animator != null && !string.IsNullOrEmpty(defaultAnimationClipName))
            {
                animator.speed = 0;  // 停止动画
                animator.Play(defaultAnimationClipName, 0, 0f);  // 回到第一帧
            }
            return; // 跳过后续检查
        }

        // 如果鼠标悬停，则播放动画，否则停在第一帧
        if (isMouseOver && animator != null)
        {
            animator.speed = 1;  // 恢复动画播放
        }
        else if (!isMouseOver && animator != null)
        {
            animator.speed = 0;  // 停止动画并回到第一帧
            if (!string.IsNullOrEmpty(defaultAnimationClipName))
            {
                animator.Play(defaultAnimationClipName, 0, 0f);  // 确保回到第一帧
            }
        }
    }


    // 鼠标进入时开始播放动画
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    // 鼠标退出时停止动画
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    // 拖拽开始
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 仅处理左键拖拽
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        isDragging = true;  // 标记为正在拖拽

        // 克隆母体物品并设置为 DraggingPanel 的子对象
        child = Instantiate(gameObject, draggingPanel);

        // 移除母体的 MotherItem 脚本并添加 ChildItem 脚本
        Destroy(child.GetComponent<MotherItem>());
        child.AddComponent<ChildItem>();

        // 重置克隆物品的 RectTransform
        RectTransform childRect = child.GetComponent<RectTransform>();
        childRect.anchorMin = new Vector2(0.5f, 0.5f);
        childRect.anchorMax = new Vector2(0.5f, 0.5f);
        childRect.pivot = new Vector2(0.5f, 0.5f);

        // 将位置设置在鼠标的初始位置
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingPanel, eventData.position, eventData.pressEventCamera, out localPoint);
        childRect.anchoredPosition = localPoint;

        // 设置孩子的大小为固定的 150x150
        childRect.sizeDelta = new Vector2(180, 180);

        // 添加 CanvasGroup 避免阻挡其他 UI 元素
        canvasGroup = child.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;
    }

    // 拖拽过程中
    public void OnDrag(PointerEventData eventData)
    {
        // 仅处理左键拖拽
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        // 更新拖拽时的位置
        RectTransform childRect = child.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingPanel, eventData.position, eventData.pressEventCamera, out localPoint);
        childRect.anchoredPosition = localPoint;

        // 检查物品是否在DrawingPanel范围外
        if (!RectTransformUtility.RectangleContainsScreenPoint(drawingPanel, eventData.position, eventData.pressEventCamera))
        {
            child.GetComponent<Image>().color = new Color(1f, 0.8f, 0.8f, 1f);  // 外部时变红
        }
        else
        {
            child.GetComponent<Image>().color = Color.white;  // 内部时恢复正常颜色
        }
    }

    // 拖拽结束
    public void OnEndDrag(PointerEventData eventData)
    {
        // 仅处理左键拖拽
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        // 拖拽结束后允许物品接收事件
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        child.transform.SetParent(drawingPanel.transform, false);

        // 如果物品没有留在 DrawingPanel 内，则销毁
        if (!RectTransformUtility.RectangleContainsScreenPoint(drawingPanel, eventData.position, eventData.pressEventCamera))
        {
            Destroy(child);
        }

        isDragging = false;  // 标记拖拽结束
    }
}
