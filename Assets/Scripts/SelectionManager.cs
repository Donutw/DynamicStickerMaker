using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    private ChildItem currentSelectedItem;

    // 选中事件
    public event System.Action<ChildItem> OnItemSelected;
    public event System.Action OnItemDeselected; // 取消选中事件

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 确保只有一个 SelectionManager 实例
        }
    }

    // 选择一个物体
    public void SelectItem(ChildItem newItem)
    {
        // 如果点击的是当前选中的物体，就取消选中
        if (currentSelectedItem == newItem)
        {
            Debug.Log("Click on already selected item: " + newItem.name + ". Triggering Deselect.");
            Deselect();
            return;
        }
        else
        {
            // 如果已有选中项，则先取消选中
            if (currentSelectedItem != null)
            {
                Debug.Log("Deselecting current item: " + currentSelectedItem.name);
                currentSelectedItem.Deselect();
                OnItemDeselected?.Invoke();
            }

            // 更新当前选中的物体为新的物体
            currentSelectedItem = newItem;
            Debug.Log("Selecting new item: " + newItem.name);
            currentSelectedItem.Select();
            OnItemSelected?.Invoke(currentSelectedItem);
        }
    }


    // 取消选中物体
    public void Deselect()
    {
        if (currentSelectedItem != null)
        {
            Debug.Log("deselected" + currentSelectedItem.name);
            currentSelectedItem.Deselect();
            OnItemDeselected?.Invoke(); // 触发取消选中事件
            currentSelectedItem = null; // 清空当前选中的物体
        }
    }

    // 获取当前选中的物体
    public ChildItem GetSelectedItem()
    {
        return currentSelectedItem;
    }
}
