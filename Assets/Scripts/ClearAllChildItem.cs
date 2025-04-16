using UnityEngine;

public class ClearAllChildItem : MonoBehaviour
{
    // ��ť OnClick ���ô˷���
    public void ClearAll()
    {
        // ���ҳ��������� ChildItem ���
        ChildItem[] items = FindObjectsOfType<ChildItem>();
        foreach (ChildItem item in items)
        {
            Destroy(item.gameObject);
        }
        Debug.Log("��������� ChildItem ����");
    }
}
