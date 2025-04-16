using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LabelButtonManager : MonoBehaviour
{
    // �ڲ��࣬���ڱ���ÿ����ť��ԭʼ����
    private class ButtonOriginalData
    {
        public Transform originalParent; // ��ťԭʼ�����壨���� panel��
        public int siblingIndex;         // �� panel �е�ԭʼ����λ��
    }

    // �洢���� tag Ϊ "label" �İ�ť����ԭʼ����
    private Dictionary<Button, ButtonOriginalData> buttonData = new Dictionary<Button, ButtonOriginalData>();

    void Start()
    {
        // ʹ�� Resources.FindObjectsOfTypeAll ���������� GameObject������ inactive �ģ�
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject go in allObjects)
        {
            // ���ˣ�ֻ�����ǩΪ "label" �����Ѽ��صĳ����еĶ���
            if (go.CompareTag("label") && go.scene.isLoaded)
            {
                Button btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    ButtonOriginalData data = new ButtonOriginalData();
                    data.originalParent = btn.transform.parent;
                    data.siblingIndex = btn.transform.GetSiblingIndex();
                    buttonData[btn] = data;
                }
                else
                {
                    Debug.LogWarning($"���� {go.name} �����Ϊ 'label' ��û�� Button �����");
                }
            }
        }
    }

    void Update()
    {
        foreach (var kvp in buttonData)
        {
            Button btn = kvp.Key;
            ButtonOriginalData data = kvp.Value;

            // ��鰴ť�� interactable ״̬
            if (!btn.interactable)
            {
                // ����ť������ʱ���ƶ����丸����ĸ������У�������ڣ�
                if (data.originalParent != null && data.originalParent.parent != null)
                {
                    Transform targetParent = data.originalParent.parent;
                    if (btn.transform.parent != targetParent)
                    {
                        btn.transform.SetParent(targetParent, false);
                    }
                    // ���õ�Ŀ�길��������ϲ㣨���һ���Ӷ���
                    btn.transform.SetAsLastSibling();
                }
                else
                {
                    Debug.LogWarning($"��ť {btn.gameObject.name} û���ҵ����ʵĸ�����ĸ����塣");
                }
            }
            else
            {
                // ����ť����ʱ�����仹ԭ��ԭ panel �У����ָ�ԭ��������λ��
                if (btn.transform.parent != data.originalParent)
                {
                    btn.transform.SetParent(data.originalParent, false);
                    btn.transform.SetSiblingIndex(data.siblingIndex);
                }
            }
        }
    }
}
