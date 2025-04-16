using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MotherItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject child;
    private RectTransform draggingPanel;
    private CanvasGroup canvasGroup;
    private RectTransform drawingPanel;

    private Animator animator;  // ���ڿ��ƶ���
    private bool isMouseOver = false;  // �Ƿ������ͣ
    private bool isDragging = false;  // �Ƿ�������ק

    private string defaultAnimationClipName;  // ��������Ĭ�϶���������

    private void Start()
    {
        // �Զ���ȡ��ǰ�����е� DrawingPanel
        draggingPanel = GameObject.Find("DraggingPanel").GetComponent<RectTransform>();
        drawingPanel = GameObject.Find("DrawingPanel").GetComponent<RectTransform>();

        // ��ȡ Animator ���
        animator = GetComponent<Animator>();

        // ��ȡĬ�ϵĶ���Ƭ�����ƣ�����ж���Ƭ�εĻ���
        if (animator != null)
        {
            // ��ȡ��ǰ���������������ж���Ƭ��
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

            // �����ǰ�ж���Ƭ�Σ���ȡ��һ��Ƭ�ε�����
            if (clipInfo.Length > 0)
            {
                defaultAnimationClipName = clipInfo[0].clip.name;
            }
        }

        // ȷ��һ��ʼ�����Ǿ�̬�ģ��ص���һ֡��
        if (animator != null && !string.IsNullOrEmpty(defaultAnimationClipName))
        {
            animator.speed = 0;  // ֹͣ����
            animator.Play(defaultAnimationClipName, 0, 0f);  // �ص���һ֡
        }
    }

    private void Update()
    {
        // ���������ק����ǿ��ֹͣ����
        if (isDragging)
        {
            if (animator != null && !string.IsNullOrEmpty(defaultAnimationClipName))
            {
                animator.speed = 0;  // ֹͣ����
                animator.Play(defaultAnimationClipName, 0, 0f);  // �ص���һ֡
            }
            return; // �����������
        }

        // ��������ͣ���򲥷Ŷ���������ͣ�ڵ�һ֡
        if (isMouseOver && animator != null)
        {
            animator.speed = 1;  // �ָ���������
        }
        else if (!isMouseOver && animator != null)
        {
            animator.speed = 0;  // ֹͣ�������ص���һ֡
            if (!string.IsNullOrEmpty(defaultAnimationClipName))
            {
                animator.Play(defaultAnimationClipName, 0, 0f);  // ȷ���ص���һ֡
            }
        }
    }


    // ������ʱ��ʼ���Ŷ���
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    // ����˳�ʱֹͣ����
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    // ��ק��ʼ
    public void OnBeginDrag(PointerEventData eventData)
    {
        // �����������ק
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        isDragging = true;  // ���Ϊ������ק

        // ��¡ĸ����Ʒ������Ϊ DraggingPanel ���Ӷ���
        child = Instantiate(gameObject, draggingPanel);

        // �Ƴ�ĸ��� MotherItem �ű������ ChildItem �ű�
        Destroy(child.GetComponent<MotherItem>());
        child.AddComponent<ChildItem>();

        // ���ÿ�¡��Ʒ�� RectTransform
        RectTransform childRect = child.GetComponent<RectTransform>();
        childRect.anchorMin = new Vector2(0.5f, 0.5f);
        childRect.anchorMax = new Vector2(0.5f, 0.5f);
        childRect.pivot = new Vector2(0.5f, 0.5f);

        // ��λ�����������ĳ�ʼλ��
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingPanel, eventData.position, eventData.pressEventCamera, out localPoint);
        childRect.anchoredPosition = localPoint;

        // ���ú��ӵĴ�СΪ�̶��� 150x150
        childRect.sizeDelta = new Vector2(180, 180);

        // ��� CanvasGroup �����赲���� UI Ԫ��
        canvasGroup = child.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;
    }

    // ��ק������
    public void OnDrag(PointerEventData eventData)
    {
        // �����������ק
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        // ������קʱ��λ��
        RectTransform childRect = child.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingPanel, eventData.position, eventData.pressEventCamera, out localPoint);
        childRect.anchoredPosition = localPoint;

        // �����Ʒ�Ƿ���DrawingPanel��Χ��
        if (!RectTransformUtility.RectangleContainsScreenPoint(drawingPanel, eventData.position, eventData.pressEventCamera))
        {
            child.GetComponent<Image>().color = new Color(1f, 0.8f, 0.8f, 1f);  // �ⲿʱ���
        }
        else
        {
            child.GetComponent<Image>().color = Color.white;  // �ڲ�ʱ�ָ�������ɫ
        }
    }

    // ��ק����
    public void OnEndDrag(PointerEventData eventData)
    {
        // �����������ק
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        // ��ק������������Ʒ�����¼�
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        child.transform.SetParent(drawingPanel.transform, false);

        // �����Ʒû������ DrawingPanel �ڣ�������
        if (!RectTransformUtility.RectangleContainsScreenPoint(drawingPanel, eventData.position, eventData.pressEventCamera))
        {
            Destroy(child);
        }

        isDragging = false;  // �����ק����
    }
}
