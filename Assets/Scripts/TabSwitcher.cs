using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSwitcher : MonoBehaviour
{
    [System.Serializable]
    public struct Tab
    {
        public Button tabButton;    // ÿ��ѡ��İ�ť
        public GameObject page;     // ��Ӧ��ҳ��
    }

    public Tab[] tabs;  // �洢���а�ť��ҳ������

    private int currentPageIndex = 0;  // ��ǰҳ������

    void Start()
    {
        // Ϊÿ����ť��ӵ���¼�
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;  // ����հ�����
            tabs[i].tabButton.onClick.AddListener(() => ShowPage(index));
        }

        ShowPage(0);  // Ĭ����ʾ��һ��ҳ��
    }

    // ��ʾ��Ӧ��ҳ�沢��������ҳ��
    public void ShowPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= tabs.Length)
        {
            Debug.LogWarning("Page index out of bounds!");
            return;
        }

        // ��������ҳ��
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].page.SetActive(i == pageIndex);  // ֻ��ʾ��ǰѡ���ҳ��
        }

        // ���°�ť��״̬��������õ�ǰ��ť��
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].tabButton.interactable = (i != pageIndex);
        }

        currentPageIndex = pageIndex;  // ���µ�ǰҳ������
    }

    // ��ȡ��ǰҳ������
    public int GetCurrentPage()
    {
        return currentPageIndex;
    }
}
