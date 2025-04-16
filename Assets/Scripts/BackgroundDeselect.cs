using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundDeselect : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        // ���������ʱȡ����ǰѡ����
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.Deselect();
        }
    }

}
