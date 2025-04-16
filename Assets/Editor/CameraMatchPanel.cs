using UnityEngine;

public class CameraMatchPanel : MonoBehaviour
{
    public Camera captureCamera; // ���ڲ�׽�������
    public RectTransform panel;  // ��Ҫ��׽��Panel

    void Start()
    {
        MatchCameraToPanel();
    }

    public void MatchCameraToPanel()
    {
        // ��ȡPanel������ռ��е��ĸ���λ��
        Vector3[] panelCorners = new Vector3[4];
        panel.GetWorldCorners(panelCorners);

        // �ҵ�Panel������ռ������
        Vector3 panelCenter = (panelCorners[0] + panelCorners[2]) / 2f;

        // ����Panel�Ŀ�Ⱥ͸߶�
        float panelWidth = Vector3.Distance(panelCorners[0], panelCorners[3]);
        float panelHeight = Vector3.Distance(panelCorners[0], panelCorners[1]);

        // ���������Ϊ����ģʽ
        captureCamera.orthographic = true;

        // �����������λ��ΪPanel������
        captureCamera.transform.position = new Vector3(panelCenter.x, panelCenter.y, captureCamera.transform.position.z);

        // �����������������С��ʹ������Panel
        captureCamera.orthographicSize = panelHeight / 2;

        // ����������Ŀ�߱ȣ����������������׶
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = panelWidth / panelHeight;

        if (targetAspect < screenAspect)
        {
            // ��� Panel ���ߣ���Ҫ��С������ĺ�����Ұ
            captureCamera.rect = new Rect(0.5f - targetAspect / screenAspect / 2f, 0, targetAspect / screenAspect, 1);
        }
        else
        {
            // ��� Panel ������Ҫ��С�������������Ұ
            captureCamera.rect = new Rect(0, 0.5f - screenAspect / targetAspect / 2f, 1, screenAspect / targetAspect);
        }
    }
}
