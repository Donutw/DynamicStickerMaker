using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using ImageMagick;

public class ExportPanelToPNG : MonoBehaviour
{
    [Header("���ʹ�õ��ʲ�")]
    public GameObject drawingPanel; // ԭʼ�� Drawing Panel
    public GameObject exportCanvas; // ���� Canvas
    public Camera exportCamera;     // �����õ� Camera
    public PlayPauseButton playPauseButton;
    public GifCreator gifCreator;

    [Header("���ѡ��")] // �� Inspector ����ʾһ������
    [Tooltip("���ͼƬ�ĳߴ磬��λ������")]
    public int imageSize = 512;
    [Tooltip("������ʹ�ã���Ҫ����Ķ�����֡��")]
    public int totalFrames = 8; //ҲӦ�ú��������֡����ȣ��������ڻ��ڲ���������

    private int panelSize = 770;    // renderText �������γߴ�
    public float captureFrameInterval = 0.1f;  //��ȡ�ĵ�ʱ��������λ����
                                                //��Ӧ�������֡���ʱ����ȣ�
    private string tmpFolderPath;  // �����ļ���Ŀ¼
    private GameObject clonedPanel;
    private Animator[] animators;
    private bool isExporting = false;


    void Start()
    {
        // ��ʼ���ļ���·��
        if (string.IsNullOrEmpty(tmpFolderPath))
        {
            tmpFolderPath = Application.dataPath + "/tmp";
        }

        // ����ļ��в����ڣ��򴴽�
        if (!Directory.Exists(tmpFolderPath))
        {
            Directory.CreateDirectory(tmpFolderPath);
        }
    }

    // ���øú�������֡���� PNG ͼ��
    public void ExportFramesToPNG()
    {
        if (isExporting) return;
        isExporting = true;

        animators = FindObjectsOfType<Animator>(); // ��ȡ���� Animator
        StartCoroutine(CaptureFrames());
    }

    private IEnumerator CaptureFrames()
    {
        float startTime = Time.time; // ��¼��ʼʱ��

        // **�������ж������ӵ�һ֡��ʼ**
        foreach (var animator in animators)
        {
            PlayPauseButton.isPlaying = false;
            playPauseButton.ToggleAllAnimations();
        }

        // **��¡ Drawing Panel**
        if (clonedPanel != null)
        {
            Destroy(clonedPanel);
        }

        exportCamera.clearFlags = CameraClearFlags.SolidColor;
        exportCamera.backgroundColor = new Color(0, 0, 0, 0); // ͸������

        var eCInitialSize = exportCamera.orthographicSize;
        var eCInitialAspect = exportCamera.aspect;
        var eCInitialPosition = exportCamera.transform.position;

        clonedPanel = Instantiate(drawingPanel);
        clonedPanel.GetComponent<Mask>().enabled = false; // ���� Mask

        clonedPanel.transform.SetParent(exportCanvas.transform, false); // ����¡ Panel ���뵼�� Canvas
        clonedPanel.transform.localPosition = Vector3.zero;

        // **���� RenderTexture**
        RenderTexture renderTexture = new RenderTexture(panelSize, panelSize, 32, RenderTextureFormat.ARGB32);
        renderTexture.useMipMap = false;
        renderTexture.filterMode = FilterMode.Bilinear;
        exportCamera.targetTexture = renderTexture;

        AdjustCameraToFitPanel(clonedPanel); // ���������Ұ��ƥ�� Panel

        // **��֡��ͼ������**
        for (int i = 0; i < totalFrames; i++)
        {
            // �ֶ��ƽ� Animator������ʱ�����⣬ȷ��������ȷ֡��
            foreach (var animator in animators)
            {
                animator.Update(captureFrameInterval); // �ֶ����¶���֡
            }

            // **������Ⱦ���ȴ��������**
            exportCamera.Render();
            yield return new WaitForEndOfFrame();

            // **��ʼ��ͼ**
            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(panelSize, panelSize, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, panelSize, panelSize), 0, 0);
            texture.Apply();

            // ���ŵ�Ŀ��ߴ�
            Texture2D scaledTexture = ScaleTexture(texture, imageSize, imageSize); // Ŀ���С���Ը�����Ҫ����

            // **�����ͼ**
            string fileName = $"frame_{i:D4}.png";
            string filePath = Path.Combine(tmpFolderPath, fileName);
            File.WriteAllBytes(filePath, scaledTexture.EncodeToPNG());
            Debug.Log($"Frame {i} saved: {filePath}");

            Destroy(texture); // �����������ͷ��ڴ�
        }

        // **��ͼ��ɺ���ͣ�������ָ�״̬**
        foreach (var animator in animators)
        {
            PlayPauseButton.isPlaying = true;
            playPauseButton.ToggleAllAnimations();
        }

        // **�ָ��������**
        exportCamera.orthographicSize = eCInitialSize;
        exportCamera.aspect = eCInitialAspect;
        exportCamera.transform.position = eCInitialPosition;

        // **�ͷ���Դ**
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
            clonedPanel = null; // ��ֹ��������
        }

        Debug.Log($"����֡�ѱ��浽�ļ���: {tmpFolderPath}");

        // **���� GIF ���ɺ���**
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