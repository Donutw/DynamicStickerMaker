using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChildItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform draggingPanel;
    private CanvasGroup canvasGroup;
    private RectTransform drawingPanel;
    private Animator animator;
    private AnimationClip animationClip;

    private Outline outline;
    private Image image;
    public bool isSelected = false;

    private int originalSiblingIndex;

    public bool isFlippedHorizontally = false;
    public bool isFlippedVertically = false;


    private void Awake()
    {
        image = GetComponent<Image>();
        // 尝试获取现有的 Outline 组件，若没有则添加一个
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.8f, 0.8f, 0.8f, 0.3f); // 选择一个高对比度颜色
            outline.effectDistance = new Vector2(5, 5);
            outline.enabled = false;
        }
    }

    private void Start()
    {
        draggingPanel = GameObject.Find("DraggingPanel").GetComponent<RectTransform>();
        drawingPanel = GameObject.Find("DrawingPanel").GetComponent<RectTransform>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;

        animator = GetComponent<Animator>();
        animationClip = animator.runtimeAnimatorController.animationClips[0];

        UpdateAnimationState();
    }

    public void UpdateAnimationState()
    {
        if (animator != null)
        {
            animator.speed = PlayPauseButton.isPlaying ? 1f : 0f;
        }
    }

    public void OnPointerClick(PointerEventData eventData)//终于对了原来问题出在这里，泪目
    {
        SelectionManager.Instance.SelectItem(this);
    }

    public void Select()
    {
        isSelected = true;
        // 改变 Outline 状态
        if (outline != null)
        {
            outline.enabled = true;
        }
        if (image != null)
        {
            image.color = new Color(0.8f, 0.8f, 0.8f, 1f);  // 调整为灰色，可以根据需求调整颜色
        }
    }

    public void Deselect()
    {
        isSelected = false;

        if (outline != null)
        {
            outline.enabled = false;
        }
        if (image != null)
        {
            image.color = Color.white;  // 恢复原来的颜色
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // 记录当前的 sibling index
        originalSiblingIndex = transform.GetSiblingIndex();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.5f;
        transform.SetParent(draggingPanel.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 仅处理左键拖拽
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(draggingPanel, eventData.position, eventData.pressEventCamera, out localPoint);
        rectTransform.anchoredPosition = localPoint;

        if (!RectTransformUtility.RectangleContainsScreenPoint(draggingPanel, eventData.position, eventData.pressEventCamera))
        {
            image.color = new Color(1f, 0.7f, 0.7f, 1f);
        }else if (isSelected)
        {
            image.color = image.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        }
        else
        {
            image.color = Color.white;
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        transform.SetParent(drawingPanel.transform);

        // 恢复原来的 sibling index
        transform.SetSiblingIndex(originalSiblingIndex);

        if (!RectTransformUtility.RectangleContainsScreenPoint(draggingPanel, eventData.position, eventData.pressEventCamera))
        {
            Destroy(gameObject);
        }
    }


    public void ToggleAnimation(bool isPlaying)
    {
        Deselect();
        if (isPlaying)
        {
            animator.Play(animationClip.name);
            animator.speed = 1;
        }
        else
        {
            animator.Play(animationClip.name, -1, 0f);
            animator.speed = 0;
        }
    }

}
