using UnityEngine;
using UnityEngine.UI;

public class PlayPauseButton : MonoBehaviour
{

    private ChildItem[] childItems;
    [HideInInspector] public static bool isPlaying = false;


    public void ToggleAllAnimations()
    {
        // ��ȡ���������д��� ChildItem �ű�������
        childItems = FindObjectsOfType<ChildItem>();
        isPlaying = !isPlaying;
        // �л����� ChildItem ����Ķ���״̬
        foreach (var item in childItems)
        {
            item.ToggleAnimation(isPlaying);
        }
    }
}
