using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance; // "intance" yerine "instance" yazmak daha yaygýndýr
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
        // Uygulama baþlar baþlamaz periyodik kontrole baþla
        StartCoroutine(CheckConnectionRoutine());
    }

    // Uygulama tekrar odaklandýðýnda baðlantýyý kontrol et
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            // Anýnda bir kontrol yap
            StartCoroutine(CheckInternet());
        }
    }

    // Belirli aralýklarla baðlantýyý kontrol eden ana döngü
    private IEnumerator CheckConnectionRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(CheckInternet()); // Kontrolün bitmesini bekle
            yield return new WaitForSeconds(5f); // Kontrol sýklýðýný biraz artýrabiliriz (ör: 10 saniye)
        }
    }

    // Asýl internet kontrolünü yapan coroutine
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

    // OK Butonuna basýldýðýnda
    public void OnOkButtonClicked()
    {
        // Wi-Fi ayarlarýna yönlendir
#if UNITY_ANDROID && !UNITY_EDITOR
            OpenWifiSettingsAndroid();
#endif

        // Paneli tekrar kontrol etmeden önce hemen gizle
        PanelManager.Instance.HidePanelWithPanelID(PanelID.ErrorNetwork);

        // Kullanýcý "Tamam" dediðinde hemen tekrar bir kontrol yapabiliriz
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