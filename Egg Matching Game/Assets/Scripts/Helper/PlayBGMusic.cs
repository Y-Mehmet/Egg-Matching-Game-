using UnityEngine;

public class PlayBGMusic : MonoBehaviour
{
    private void OnEnable()
    {
        SoundManager.instance.PlayBgm(SoundType.BG);
    }
    private void OnDisable()
    {
        SoundManager.instance.StopBG();
    }
}
