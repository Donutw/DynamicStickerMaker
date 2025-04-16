using UnityEngine;
using UnityEngine.UI;

public class ScrollViewDeselector : MonoBehaviour
{
    // ��Ҫ���� ScrollRect ͬһ������
    private ScrollRect scrollRect;
    private RectTransform rectTransform;
    private Canvas canvas; // �����ж����λ��ת������� Canvas �� ScreenSpace - Overlay�������Ϊ null��

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        rectTransform = scrollRect.GetComponent<RectTransform>();

        // ������ Canvas �� ScreenSpace - Camera������Ҫָ�� canvas.worldCamera
        // ����� ScreenSpace - Overlay������ null����
        canvas = GetComponentInParent<Canvas>();
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void OnScrollValueChanged(Vector2 value)
    {
        // �� ScrollRect ����λ�ñ仯ʱ���������Ƿ������������
        if (IsPointerOverScrollRect())
        {
            SelectionManager.Instance?.Deselect();
        }
    }

    private bool IsPointerOverScrollRect()
    {
        // ��� Canvas �� Overlay�����Դ��� null
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, cam);
    }
}
