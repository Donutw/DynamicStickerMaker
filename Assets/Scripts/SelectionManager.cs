using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    private ChildItem currentSelectedItem;

    // ѡ���¼�
    public event System.Action<ChildItem> OnItemSelected;
    public event System.Action OnItemDeselected; // ȡ��ѡ���¼�

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // ȷ��ֻ��һ�� SelectionManager ʵ��
        }
    }

    // ѡ��һ������
    public void SelectItem(ChildItem newItem)
    {
        // ���������ǵ�ǰѡ�е����壬��ȡ��ѡ��
        if (currentSelectedItem == newItem)
        {
            Debug.Log("Click on already selected item: " + newItem.name + ". Triggering Deselect.");
            Deselect();
            return;
        }
        else
        {
            // �������ѡ�������ȡ��ѡ��
            if (currentSelectedItem != null)
            {
                Debug.Log("Deselecting current item: " + currentSelectedItem.name);
                currentSelectedItem.Deselect();
                OnItemDeselected?.Invoke();
            }

            // ���µ�ǰѡ�е�����Ϊ�µ�����
            currentSelectedItem = newItem;
            Debug.Log("Selecting new item: " + newItem.name);
            currentSelectedItem.Select();
            OnItemSelected?.Invoke(currentSelectedItem);
        }
    }


    // ȡ��ѡ������
    public void Deselect()
    {
        if (currentSelectedItem != null)
        {
            Debug.Log("deselected" + currentSelectedItem.name);
            currentSelectedItem.Deselect();
            OnItemDeselected?.Invoke(); // ����ȡ��ѡ���¼�
            currentSelectedItem = null; // ��յ�ǰѡ�е�����
        }
    }

    // ��ȡ��ǰѡ�е�����
    public ChildItem GetSelectedItem()
    {
        return currentSelectedItem;
    }
}
