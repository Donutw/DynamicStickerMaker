using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using ImageMagick;

public class ExportPanelToPNG : MonoBehaviour
{
    [Header("相关使用的资产")]
    public GameObject drawingPanel; // 原始的 Drawing Panel
    public GameObject exportCanvas; // 导出 Canvas
    public Camera exportCamera;     // 导出用的 Camera
    public PlayPauseButton playPauseButton;
    public GifCreator gifCreator;

    [Header("输出选项")] // 在 Inspector 里显示一个标题
    [Tooltip("输出图片的尺寸，单位：像素")]
    public int imageSize = 512;
    [Tooltip("（测试使用）需要捕获的动画总帧数")]
    public int totalFrames = 8; //也应该和组件的总帧数相等，但是现在还在测试先留着

    private int panelSize = 770;    // renderText 的正方形尺寸
    public float captureFrameInterval = 0.1f;  //截取的的时间间隔，单位：秒
                                                //（应当和组件帧间隔时长相等）
    private string tmpFolderPath;  // 保存文件的目录
    private GameObject clonedPanel;
    private Animator[] animators;
    private bool isExporting = false;


    void Start()
    {
        // 初始化文件夹路径
        if (string.IsNullOrEmpty(tmpFolderPath))
        {
            tmpFolderPath = Application.dataPath + "/tmp";
        }

        // 如果文件夹不存在，则创建
        if (!Directory.Exists(tmpFolderPath))
        {
            Directory.CreateDirectory(tmpFolderPath);
        }
    }

    // 调用该函数来逐帧导出 PNG 图像
    public void ExportFramesToPNG()
    {
        if (isExporting) return;
        isExporting = true;

        animators = FindObjectsOfType<Animator>(); // 获取所有 Animator
        StartCoroutine(CaptureFrames());
    }

    private IEnumerator CaptureFrames()
    {
        float startTime = Time.time; // 记录开始时间

        // **启动所有动画并从第一帧开始**
        foreach (var animator in animators)
        {
            PlayPauseButton.isPlaying = false;
            playPauseButton.ToggleAllAnimations();
        }

        // **克隆 Drawing Panel**
        if (clonedPanel != null)
        {
            Destroy(clonedPanel);
        }

        exportCamera.clearFlags = CameraClearFlags.SolidColor;
        exportCamera.backgroundColor = new Color(0, 0, 0, 0); // 透明背景

        var eCInitialSize = exportCamera.orthographicSize;
        var eCInitialAspect = exportCamera.aspect;
        var eCInitialPosition = exportCamera.transform.position;

        clonedPanel = Instantiate(drawingPanel);
        clonedPanel.GetComponent<Mask>().enabled = false; // 禁用 Mask

        clonedPanel.transform.SetParent(exportCanvas.transform, false); // 将克隆 Panel 放入导出 Canvas
        clonedPanel.transform.localPosition = Vector3.zero;

        // **创建 RenderTexture**
        RenderTexture renderTexture = new RenderTexture(panelSize, panelSize, 32, RenderTextureFormat.ARGB32);
        renderTexture.useMipMap = false;
        renderTexture.filterMode = FilterMode.Bilinear;
        exportCamera.targetTexture = renderTexture;

        AdjustCameraToFitPanel(clonedPanel); // 调整相机视野以匹配 Panel

        // **逐帧截图并保存**
        for (int i = 0; i < totalFrames; i++)
        {
            // 手动推进 Animator（跳过时间问题，确保捕获正确帧）
            foreach (var animator in animators)
            {
                animator.Update(captureFrameInterval); // 手动更新动画帧
            }

            // **立即渲染并等待更新完成**
            exportCamera.Render();
            yield return new WaitForEndOfFrame();

            // **开始截图**
            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(panelSize, panelSize, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, panelSize, panelSize), 0, 0);
            texture.Apply();

            // 缩放到目标尺寸
            Texture2D scaledTexture = ScaleTexture(texture, imageSize, imageSize); // 目标大小可以根据需要调整

            // **保存截图**
            string fileName = $"frame_{i:D4}.png";
            string filePath = Path.Combine(tmpFolderPath, fileName);
            File.WriteAllBytes(filePath, scaledTexture.EncodeToPNG());
            Debug.Log($"Frame {i} saved: {filePath}");

            Destroy(texture); // 清理纹理以释放内存
        }

        // **截图完成后暂停动画并恢复状态**
        foreach (var animator in animators)
        {
            PlayPauseButton.isPlaying = true;
            playPauseButton.ToggleAllAnimations();
        }

        // **恢复相机参数**
        exportCamera.orthographicSize = eCInitialSize;
        exportCamera.aspect = eCInitialAspect;
        exportCamera.transform.position = eCInitialPosition;

        // **释放资源**
        exportCamera.targetTexture = null;
        RenderTexture.active = null;

        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }

        if (clonedPanel != null)
        {
            Destroy(clonedPanel);
            clonedPanel = null; // 防止残留引用
        }

        Debug.Log($"所有帧已保存到文件夹: {tmpFolderPath}");

        // **调用 GIF 生成函数**
        gifCreator.CreateGif();

        isExporting = false;
    }

    private void AdjustCameraToFitPanel(GameObject clonedPanel)
    {
        RectTransform panelRect = clonedPanel.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        panelRect.GetWorldCorners(corners);

        exportCamera.orthographic = true;

        float panelHeight = Vector3.Distance(corners[0], corners[1]);
        exportCamera.orthographicSize = panelHeight / 2f;

        Vector3 panelCenter = (corners[0] + corners[2]) / 2f;
        exportCamera.transform.position = new Vector3(panelCenter.x, panelCenter.y, exportCamera.transform.position.z);

        float panelWidth = Vector3.Distance(corners[0], corners[3]);
        exportCamera.aspect = panelWidth / panelHeight;
    }

    private Texture2D ScaleTexture(Texture2D texture, int width, int height)
    {
        Texture2D newTexture = new Texture2D(width, height);
        Color[] pixels = texture.GetPixels();
        Color[] newPixels = newTexture.GetPixels();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                newPixels[y * width + x] = pixels[(y * texture.height / height) * texture.width + (x * texture.width / width)];
            }
        }

        newTexture.SetPixels(newPixels);
        newTexture.Apply();
        return newTexture;

    }
}