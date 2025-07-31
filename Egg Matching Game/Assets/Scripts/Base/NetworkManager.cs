using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager intance;
    private Coroutine checkConnectionCoroutine;
    private void Awake()
    {
        if(intance == null)
        {
            intance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(CheckConnectionRoutine()); // Baðlantý kontrolünü baþlat
        }
        else
        {
            Destroy(gameObject); // Singleton prensibine göre, zaten var olan bir örneði yok et
        }
    }


    void Start()
    {
        // Uygulama baþlar baþlamaz baðlantýyý kontrol et
        CheckInternetConnection();
    }

    // Belirli aralýklarla baðlantýyý kontrol etmek için bir metot (isteðe baðlý)
    IEnumerator CheckConnectionRoutine()
    {
        while (true)
        {
            CheckInternetConnection();
            yield return new WaitForSeconds(5f); // Her 5 saniyede bir kontrol et
        }
    }

    public void CheckInternetConnection()
    {
        if(checkConnectionCoroutine!= null)
        {
            StopCoroutine(checkConnectionCoroutine); // Önceki kontrolü durdur
        }
        checkConnectionCoroutine = StartCoroutine(CheckInternet());
    }
    private IEnumerator CheckInternet()
    {
        UnityWebRequest request = new UnityWebRequest("https://www.google.com");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            PanelManager.Instance.ShowPanel(PanelID.ErrorNetwork, PanelShowBehavior.HIDE_PREVISE);

        }else
            PanelManager.Instance.HidePanelWithPanelID(PanelID.ErrorNetwork);

    }
    public void OnOkButtonClicked()
    {
        // Wi-Fi ayarlarýna yönlendir (Android özel)
#if UNITY_ANDROID
        OpenWifiSettingsAndroid();
#endif

        // Paneli kapat
        PanelManager.Instance.HidePanelWithPanelID(PanelID.ErrorNetwork);
    }

    private void OpenWifiSettingsAndroid()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                // Wi-Fi ayarlarýna gitmek için intent oluþtur
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.settings.WIFI_SETTINGS");
                activity.Call("startActivity", intent);
            }
        }
    }

    // Uygulama tekrar odaklandýðýnda baðlantýyý kontrol et
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            CheckInternetConnection();
        }
    }
}