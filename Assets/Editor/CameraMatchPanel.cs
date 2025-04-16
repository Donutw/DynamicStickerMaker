using UnityEngine;

public class CameraMatchPanel : MonoBehaviour
{
    public Camera captureCamera; // 用于捕捉的摄像机
    public RectTransform panel;  // 需要捕捉的Panel

    void Start()
    {
        MatchCameraToPanel();
    }

    public void MatchCameraToPanel()
    {
        // 获取Panel在世界空间中的四个角位置
        Vector3[] panelCorners = new Vector3[4];
        panel.GetWorldCorners(panelCorners);

        // 找到Panel在世界空间的中心
        Vector3 panelCenter = (panelCorners[0] + panelCorners[2]) / 2f;

        // 计算Panel的宽度和高度
        float panelWidth = Vector3.Distance(panelCorners[0], panelCorners[3]);
        float panelHeight = Vector3.Distance(panelCorners[0], panelCorners[1]);

        // 设置摄像机为正交模式
        captureCamera.orthographic = true;

        // 设置摄像机的位置为Panel的中心
        captureCamera.transform.position = new Vector3(panelCenter.x, panelCenter.y, captureCamera.transform.position.z);

        // 设置摄像机的正交大小，使它覆盖Panel
        captureCamera.orthographicSize = panelHeight / 2;

        // 计算摄像机的宽高比，并调整摄像机的视锥
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = panelWidth / panelHeight;

        if (targetAspect < screenAspect)
        {
            // 如果 Panel 更高，需要缩小摄像机的横向视野
            captureCamera.rect = new Rect(0.5f - targetAspect / screenAspect / 2f, 0, targetAspect / screenAspect, 1);
        }
        else
        {
            // 如果 Panel 更宽，需要缩小摄像机的纵向视野
            captureCamera.rect = new Rect(0, 0.5f - screenAspect / targetAspect / 2f, 1, screenAspect / targetAspect);
        }
    }
}
