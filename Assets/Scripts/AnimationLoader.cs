using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AnimationLoader : MonoBehaviour
{

    // 相对于 Resources 文件夹的路径，例如 "Images/test"
    public string folderName;
    public GameObject itemPrefab; // 带有 Image 和 Animator 组件的预制体
    private Transform content; // ScrollView 的 Content 容器
    private List<GameObject> generatedItems = new List<GameObject>(); // 用于存储生成的物品

    private void Awake()
    {
        if (string.IsNullOrEmpty(folderName))
        {
            Debug.LogError("AnimationLoader: 没有指定 folderName！");
        }
    }

    void OnEnable()
    {
        content = transform; // 假定这个脚本挂在 Content 容器上
        LoadAnimatorControllersFromFolder();
    }

    void OnDisable()
    {
        ClearItems();
    }

    // 使用 Resources.LoadAll 从指定文件夹加载所有 RuntimeAnimatorController
    private void LoadAnimatorControllersFromFolder()
    {
        if (string.IsNullOrEmpty(folderName))
        {
            Debug.LogError("AnimationLoader: folderName 为空！");
            return;
        }

        // 运行时加载资源时，类型应使用 RuntimeAnimatorController
        RuntimeAnimatorController[] controllers = Resources.LoadAll<RuntimeAnimatorController>(folderName);
        if (controllers == null || controllers.Length == 0)
        {
            Debug.LogError("AnimationLoader: 在文件夹 " + folderName + " 中没有找到任何 RuntimeAnimatorController！");
            return;
        }

        Debug.Log("AnimationLoader: 在 " + folderName + " 中找到 " + controllers.Length + " 个动画控制器。");
        foreach (RuntimeAnimatorController controller in controllers)
        {
            LoadAnimatorController(controller);
        }
    }

    // 根据加载到的 RuntimeAnimatorController 生成预制体并赋值
    private void LoadAnimatorController(RuntimeAnimatorController controller)
    {
        if (controller == null)
            return;

        GameObject newItem = Instantiate(itemPrefab, content);
        Animator animator = newItem.GetComponent<Animator>();
        if (animator != null)
        {
            animator.runtimeAnimatorController = controller;
        }
        else
        {
            Debug.LogWarning("AnimationLoader: 预制体 " + itemPrefab.name + " 上未找到 Animator 组件！");
        }
        generatedItems.Add(newItem);
    }

    // 清除所有已生成的物件
    private void ClearItems()
    {
        foreach (GameObject item in generatedItems)
        {
            Destroy(item);
        }
        generatedItems.Clear();
    }
}
