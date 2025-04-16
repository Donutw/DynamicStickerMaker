using UnityEngine;

public class HelpPanelController : MonoBehaviour
{
    // �� Inspector �н��˱�����ֵΪ��İ���˵�� Panel ����
    public GameObject helpPanel;

    // ��ʾ�������
    public void ShowHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);
            // ��ѡ��ȷ���������ǰ��
            helpPanel.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogWarning("HelpPanelController: helpPanel δ���ã�");
        }
    }

    // ���ذ������
    public void HideHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }
    }
}
