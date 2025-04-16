using UnityEngine;
using UnityEngine.UI;

public class PlayPauseButton : MonoBehaviour
{

    private ChildItem[] childItems;
    [HideInInspector] public static bool isPlaying = false;


    public void ToggleAllAnimations()
    {
        // 获取场景中所有带有 ChildItem 脚本的物体
        childItems = FindObjectsOfType<ChildItem>();
        isPlaying = !isPlaying;
        // 切换所有 ChildItem 物体的动画状态
        foreach (var item in childItems)
        {
            item.ToggleAnimation(isPlaying);
        }
    }
}
