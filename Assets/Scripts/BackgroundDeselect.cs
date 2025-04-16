using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundDeselect : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        // 当点击背景时取消当前选中项
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.Deselect();
        }
    }

}
