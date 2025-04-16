using UnityEngine;

public class HelpPanelController : MonoBehaviour
{
    // 在 Inspector 中将此变量赋值为你的帮助说明 Panel 对象
    public GameObject helpPanel;

    // 显示帮助面板
    public void ShowHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);
            // 可选：确保面板在最前面
            helpPanel.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogWarning("HelpPanelController: helpPanel 未设置！");
        }
    }

    // 隐藏帮助面板
    public void HideHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }
    }
}
