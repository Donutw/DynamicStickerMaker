using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AnimationLoader : MonoBehaviour
{

    // ����� Resources �ļ��е�·�������� "Images/test"
    public string folderName;
    public GameObject itemPrefab; // ���� Image �� Animator �����Ԥ����
    private Transform content; // ScrollView �� Content ����
    private List<GameObject> generatedItems = new List<GameObject>(); // ���ڴ洢���ɵ���Ʒ

    private void Awake()
    {
        if (string.IsNullOrEmpty(folderName))
        {
            Debug.LogError("AnimationLoader: û��ָ�� folderName��");
        }
    }

    void OnEnable()
    {
        content = transform; // �ٶ�����ű����� Content ������
        LoadAnimatorControllersFromFolder();
    }

    void OnDisable()
    {
        ClearItems();
    }

    // ʹ�� Resources.LoadAll ��ָ���ļ��м������� RuntimeAnimatorController
    private void LoadAnimatorControllersFromFolder()
    {
        if (string.IsNullOrEmpty(folderName))
        {
            Debug.LogError("AnimationLoader: folderName Ϊ�գ�");
            return;
        }

        // ����ʱ������Դʱ������Ӧʹ�� RuntimeAnimatorController
        RuntimeAnimatorController[] controllers = Resources.LoadAll<RuntimeAnimatorController>(folderName);
        if (controllers == null || controllers.Length == 0)
        {
            Debug.LogError("AnimationLoader: ���ļ��� " + folderName + " ��û���ҵ��κ� RuntimeAnimatorController��");
            return;
        }

        Debug.Log("AnimationLoader: �� " + folderName + " ���ҵ� " + controllers.Length + " ��������������");
        foreach (RuntimeAnimatorController controller in controllers)
        {
            LoadAnimatorController(controller);
        }
    }

    // ���ݼ��ص��� RuntimeAnimatorController ����Ԥ���岢��ֵ
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
            Debug.LogWarning("AnimationLoader: Ԥ���� " + itemPrefab.name + " ��δ�ҵ� Animator �����");
        }
        generatedItems.Add(newItem);
    }

    // ������������ɵ����
    private void ClearItems()
    {
        foreach (GameObject item in generatedItems)
        {
            Destroy(item);
        }
        generatedItems.Clear();
    }
}
