using UnityEngine;
using UnityEngine.UI;

public class PlaySingleSound : MonoBehaviour
{
    private Button btn;
    public SoundType soundType;
    private void OnEnable()
    {
        if(SoundManager.instance!= null)
        {
            GetComponent<Button>().onClick.AddListener(PlaySound);
        }
    }
    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveListener(PlaySound);
    }
    private void PlaySound()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySfx(soundType);
        }
    }
}
