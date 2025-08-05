using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay; // Video oynatýlacak RawImage
    public RawImage loadingScreen; // Yükleme ekraný olarak kullanýlacak RawImage

    void Start()
    {
        // Video oynatýcýyý ve yükleme ekranýný hazýrla
        videoDisplay.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);

        // Video hazýrlama iþlemini baþlat
        StartCoroutine(PrepareAndPlayVideo());
    }
    void OnEnable()
    {
        // Video oynatýcýyý ve yükleme ekranýný hazýrla
        videoDisplay.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);

        // Video hazýrlama iþlemini baþlat
        StartCoroutine(PrepareAndPlayVideo());
    }
    IEnumerator PrepareAndPlayVideo()
    {
        // Video kaynaðýný ayarla (eðer kodla yapýyorsanýz)
        // videoPlayer.url = "path/to/your/video.mp4";

        videoPlayer.Prepare();
        Debug.Log("Video hazýrlanýyor...");

        // Video hazýr olana kadar bekle
        while (!videoPlayer.isPrepared)
        {
            Debug.Log("Video hazýr deðil, bekleniyor...");
            yield return null;
        }

        Debug.Log("Video hazýr! Oynatýlýyor...");

        // Yükleme ekranýný kapat ve video oynatýcýyý aktif et
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