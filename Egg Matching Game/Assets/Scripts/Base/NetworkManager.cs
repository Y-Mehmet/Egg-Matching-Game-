using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance; // "intance" yerine "instance" yazmak daha yayg�nd�r
    public bool isNetworkOpen=false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Uygulama ba�lar ba�lamaz periyodik kontrole ba�la
        StartCoroutine(CheckConnectionRoutine());
    }

    // Uygulama tekrar odakland���nda ba�lant�y� kontrol et
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            // An�nda bir kontrol yap
            StartCoroutine(CheckInternet());
        }
    }

    // Belirli aral�klarla ba�lant�y� kontrol eden ana d�ng�
    private IEnumerator CheckConnectionRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(CheckInternet()); // Kontrol�n bitmesini bekle
            yield return new WaitForSeconds(5f); // Kontrol s�kl���n� biraz art�rabiliriz (�r: 10 saniye)
        }
    }

    // As�l internet kontrol�n� yapan coroutine
    private IEnumerator CheckInternet()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://www.google.com");
        request.timeout = 5;

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            isNetworkOpen = false;
            PanelManager.Instance.ShowPanel(PanelID.ErrorNetwork, PanelShowBehavior.HIDE_PREVISE);
        }
        else
        {
            isNetworkOpen = true;
            PanelManager.Instance.HidePanelWithPanelID(PanelID.ErrorNetwork);
        }

        request.Dispose();
    }

    // OK Butonuna bas�ld���nda
    public void OnOkButtonClicked()
    {
        // Wi-Fi ayarlar�na y�nlendir
#if UNITY_ANDROID && !UNITY_EDITOR
            OpenWifiSettingsAndroid();
#endif

        // Paneli tekrar kontrol etmeden �nce hemen gizle
        PanelManager.Instance.HidePanelWithPanelID(PanelID.ErrorNetwork);

        // Kullan�c� "Tamam" dedi�inde hemen tekrar bir kontrol yapabiliriz
        StartCoroutine(CheckInternet());
    }

    private void OpenWifiSettingsAndroid()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            var intent = new AndroidJavaObject("android.content.Intent", "android.settings.WIFI_SETTINGS");
            activity.Call("startActivity", intent);
        }
    }
}