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
        // ���Ի�ȡ���е� Outline �������û�������һ��
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.8f, 0.8f, 0.8f, 0.3f); // ѡ��һ���߶Աȶ���ɫ
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

    public void OnPointerClick(PointerEventData eventData)//���ڶ���ԭ��������������Ŀ
    {
        SelectionManager.Instance.SelectItem(this);
    }

    public void Select()
    {
        isSelected = true;
        // �ı� Outline ״̬
        if (outline != null)
        {
            outline.enabled = true;
        }
        if (image != null)
        {
            image.color = new Color(0.8f, 0.8f, 0.8f, 1f);  // ����Ϊ��ɫ�����Ը������������ɫ
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
            image.color = Color.white;  // �ָ�ԭ������ɫ
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // ��¼��ǰ�� sibling index
        originalSiblingIndex = transform.GetSiblingIndex();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.5f;
        transform.SetParent(draggingPanel.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // �����������ק
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

        // �ָ�ԭ���� sibling index
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
