using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay; // Video oynat�lacak RawImage
    public RawImage loadingScreen; // Y�kleme ekran� olarak kullan�lacak RawImage

    void Start()
    {
        // Video oynat�c�y� ve y�kleme ekran�n� haz�rla
        videoDisplay.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);

        // Video haz�rlama i�lemini ba�lat
        StartCoroutine(PrepareAndPlayVideo());
    }
    void OnEnable()
    {
        // Video oynat�c�y� ve y�kleme ekran�n� haz�rla
        videoDisplay.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);

        // Video haz�rlama i�lemini ba�lat
        StartCoroutine(PrepareAndPlayVideo());
    }
    IEnumerator PrepareAndPlayVideo()
    {
        // Video kayna��n� ayarla (e�er kodla yap�yorsan�z)
        // videoPlayer.url = "path/to/your/video.mp4";

        videoPlayer.Prepare();
        Debug.Log("Video haz�rlan�yor...");

        // Video haz�r olana kadar bekle
        while (!videoPlayer.isPrepared)
        {
            Debug.Log("Video haz�r de�il, bekleniyor...");
            yield return null;
        }

        Debug.Log("Video haz�r! Oynat�l�yor...");

        // Y�kleme ekran�n� kapat ve video oynat�c�y� aktif et
        loadingScreen.gameObject.SetActive(false);
        videoDisplay.gameObject.SetActive(true);
        videoPlayer.Play();
    }
    private void OnDisable()
    {
        StopCoroutine(PrepareAndPlayVideo());
        videoDisplay.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);
    }
}