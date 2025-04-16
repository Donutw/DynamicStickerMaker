#if UNITY_EDITOR
using UnityEditor;
#endif
using SFB;
using System.IO;
using UnityEngine;
using ImageMagick;

public class GifCreator : MonoBehaviour
{
    [Tooltip("GIF 存放的文件夹名称")]
    public string outputFolderName = "ExportedImages";

    [Tooltip("GIF 两帧之间的间隔时间")]
    public float gifFrameInterval = 0.1f;

    private string tmpFolderPath;

    private void Start()
    {
        tmpFolderPath = Path.Combine(Application.dataPath, "tmp");
    }

    /// <summary>
    /// 从 tmp 文件夹中的图片生成 GIF
    /// </summary>
    public void CreateGif()
    {
        if (!Directory.Exists(tmpFolderPath))
        {
            Debug.LogError($"临时文件夹不存在：{tmpFolderPath}");
            return;
        }

        string[] files = Directory.GetFiles(tmpFolderPath, "*.png");
        if (files.Length == 0)
        {
            Debug.LogError($"临时文件夹中没有找到 PNG 图片：{tmpFolderPath}");
            return;
        }

        System.Array.Sort(files);

        string outputFolderPath = Path.Combine(Application.dataPath, outputFolderName);
        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
        }

        // **获取用户选择的文件路径**
        string outputFilePath = SaveGifWithFileDialog(outputFolderPath);

        if (string.IsNullOrEmpty(outputFilePath))
        {
            Debug.Log("用户取消了保存 GIF。");
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

        Debug.Log($"GIF 创建成功！路径：{outputFilePath}");

        ClearTmpFolder();
    }

    /// <summary>
    /// 弹出保存对话框，并自动递增文件名
    /// </summary>
    private string SaveGifWithFileDialog(string defaultFolder)
    {
#if UNITY_EDITOR
        // 如果在编辑器下，你可以选择使用 EditorUtility
        string fileName = GenerateUniqueFileName(defaultFolder, "sticker", "gif");
        string savePath = EditorUtility.SaveFilePanel("保存 GIF", defaultFolder, fileName, "gif");
        return savePath;
#else
    // 非编辑器模式下使用 Standalone File Browser
    string fileName = GenerateUniqueFileName(defaultFolder, "sticker", "gif");
    string savePath = StandaloneFileBrowser.SaveFilePanel("保存 GIF", defaultFolder, fileName, "gif");
    return savePath;
#endif
    }


    /// <summary>
    /// 生成唯一文件名（如果 sticker01.gif 已存在，下一个就是 sticker02.gif）
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
    /// 清空临时文件夹
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
            Debug.Log($"已清空临时文件夹：{tmpFolderPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"清空临时文件夹失败：{ex.Message}");
        }
    }
}
