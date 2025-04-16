using UnityEngine;

public class ToolButton : MonoBehaviour
{
    // ˮƽ��ת���� Y ����ת 180�� ���л�ˮƽ��ת��־
    public void HorizontalFlip()
    {
        ChildItem selectedItem = SelectionManager.Instance.GetSelectedItem();
        if (selectedItem != null)
        {
            Transform t = selectedItem.transform;
            // �л�ˮƽ��ת״̬
            selectedItem.isFlippedHorizontally = !selectedItem.isFlippedHorizontally;
            // �� Y ����ת 180�㣨ע�⣺������ Space.Self ��֤���ھֲ��ռ�����ת��
            t.Rotate(0, 180, 0, Space.Self);
        }
    }

    // ��ֱ��ת���� X ����ת 180�� ���л���ֱ��ת��־
    public void VerticalFlip()
    {
        ChildItem selectedItem = SelectionManager.Instance.GetSelectedItem();
        if (selectedItem != null)
        {
            Transform t = selectedItem.transform;
            selectedItem.isFlippedVertically = !selectedItem.isFlippedVertically;
            // �� X ����ת 180��
            t.Rotate(180, 0, 0, Space.Self);
        }
    }

    // ����һ�㣺ֻ��ͬһ������ϵ�Ԫ�ؽ����������
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

    // ����һ�㣺ֻ��ͬһ������ϵ�Ԫ�ؽ����������
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
