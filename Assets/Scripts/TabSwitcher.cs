using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSwitcher : MonoBehaviour
{
    [System.Serializable]
    public struct Tab
    {
        public Button tabButton;    // 每个选项卡的按钮
        public GameObject page;     // 对应的页面
    }

    public Tab[] tabs;  // 存储所有按钮和页面的配对

    private int currentPageIndex = 0;  // 当前页面索引

    void Start()
    {
        // 为每个按钮添加点击事件
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;  // 避免闭包问题
            tabs[i].tabButton.onClick.AddListener(() => ShowPage(index));
        }

        ShowPage(0);  // 默认显示第一个页面
    }

    // 显示对应的页面并隐藏其他页面
    public void ShowPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= tabs.Length)
        {
            Debug.LogWarning("Page index out of bounds!");
            return;
        }

        // 隐藏所有页面
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].page.SetActive(i == pageIndex);  // 只显示当前选择的页面
        }

        // 更新按钮的状态（例如禁用当前按钮）
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].tabButton.interactable = (i != pageIndex);
        }

        currentPageIndex = pageIndex;  // 更新当前页面索引
    }

    // 获取当前页面索引
    public int GetCurrentPage()
    {
        return currentPageIndex;
    }
}
