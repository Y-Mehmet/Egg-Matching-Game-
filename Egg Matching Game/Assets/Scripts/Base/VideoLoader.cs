using UnityEngine;
using UnityEngine.Video; // VideoPlayer için gerekli kütüphane

public class VideoLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Inspector'dan atayacaðýnýz VideoPlayer komponenti
    

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                Debug.LogError("VideoPlayer komponenti bulunamadý!");
                return;
            }
        }

        LoadVideo();
    }

    void LoadVideo()
    {
        if (DragonManager.Instance == null || DragonManager.Instance.GetCurrentDragonSO() == null || ResourceManager.Instance.SelectedDragonIndex==-1)
            return;
        VideoClip clip = DragonManager.Instance.dragonHolder.dragonSOList[ResourceManager.Instance.SelectedDragonIndex].videoClip;

        if (clip != null)
        {
            videoPlayer.clip = clip; // VideoPlayer'a klibi ata
            videoPlayer.Play();      // Videoyu oynat
            videoPlayer.isLooping = true;
           
        }
       
    }
}