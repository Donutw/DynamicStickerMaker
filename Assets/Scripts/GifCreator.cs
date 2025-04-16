#if UNITY_EDITOR
using UnityEditor;
#endif
using SFB;
using System.IO;
using UnityEngine;
using ImageMagick;

public class GifCreator : MonoBehaviour
{
    [Tooltip("GIF ��ŵ��ļ�������")]
    public string outputFolderName = "ExportedImages";

    [Tooltip("GIF ��֮֡��ļ��ʱ��")]
    public float gifFrameInterval = 0.1f;

    private string tmpFolderPath;

    private void Start()
    {
        tmpFolderPath = Path.Combine(Application.dataPath, "tmp");
    }

    /// <summary>
    /// �� tmp �ļ����е�ͼƬ���� GIF
    /// </summary>
    public void CreateGif()
    {
        if (!Directory.Exists(tmpFolderPath))
        {
            Debug.LogError($"��ʱ�ļ��в����ڣ�{tmpFolderPath}");
            return;
        }

        string[] files = Directory.GetFiles(tmpFolderPath, "*.png");
        if (files.Length == 0)
        {
            Debug.LogError($"��ʱ�ļ�����û���ҵ� PNG ͼƬ��{tmpFolderPath}");
            return;
        }

        System.Array.Sort(files);

        string outputFolderPath = Path.Combine(Application.dataPath, outputFolderName);
        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
        }

        // **��ȡ�û�ѡ����ļ�·��**
        string outputFilePath = SaveGifWithFileDialog(outputFolderPath);

        if (string.IsNullOrEmpty(outputFilePath))
        {
            Debug.Log("�û�ȡ���˱��� GIF��");
            return;
        }

        using (var images = new MagickImageCollection())
        {
            foreach (string filePath in files)
            {
                var image = new MagickImage(filePath);
                image.AnimationDelay = (uint)(gifFrameInterval * 100);
                image.GifDisposeMethod = GifDisposeMethod.Background;
                images.Add(image);
            }

            images[0].AnimationIterations = 0;

            var settings = new QuantizeSettings { Colors = 256 };
            images.Quantize(settings);
            images.Write(outputFilePath);
        }

        Debug.Log($"GIF �����ɹ���·����{outputFilePath}");

        ClearTmpFolder();
    }

    /// <summary>
    /// ��������Ի��򣬲��Զ������ļ���
    /// </summary>
    private string SaveGifWithFileDialog(string defaultFolder)
    {
#if UNITY_EDITOR
        // ����ڱ༭���£������ѡ��ʹ�� EditorUtility
        string fileName = GenerateUniqueFileName(defaultFolder, "sticker", "gif");
        string savePath = EditorUtility.SaveFilePanel("���� GIF", defaultFolder, fileName, "gif");
        return savePath;
#else
    // �Ǳ༭��ģʽ��ʹ�� Standalone File Browser
    string fileName = GenerateUniqueFileName(defaultFolder, "sticker", "gif");
    string savePath = StandaloneFileBrowser.SaveFilePanel("���� GIF", defaultFolder, fileName, "gif");
    return savePath;
#endif
    }


    /// <summary>
    /// ����Ψһ�ļ�������� sticker01.gif �Ѵ��ڣ���һ������ sticker02.gif��
    /// </summary>
    private string GenerateUniqueFileName(string folderPath, string baseFileName, string extension)
    {
        int counter = 1;
        string fileName;
        string filePath;

        do
        {
            fileName = $"{baseFileName}{counter:D2}.{extension}";
            filePath = Path.Combine(folderPath, fileName);
            counter++;
        } while (File.Exists(filePath));

        return fileName;
    }

    /// <summary>
    /// �����ʱ�ļ���
    /// </summary>
    private void ClearTmpFolder()
    {
        try
        {
            string[] files = Directory.GetFiles(tmpFolderPath);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            Debug.Log($"�������ʱ�ļ��У�{tmpFolderPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"�����ʱ�ļ���ʧ�ܣ�{ex.Message}");
        }
    }
}
