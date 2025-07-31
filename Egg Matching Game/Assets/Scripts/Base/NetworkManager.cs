using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager intance;
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
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            PanelManager.Instance.ShowPanel(PanelID.ErrorNetwork, PanelShowBehavior.HIDE_PREVISE);
           
        }
        else
        {
            PanelManager.Instance.HideAllPanel();
        }
    }

    public void OnOkButtonClicked()
    {
        // Wi-Fi ayarlarýna yönlendir (Android özel)
#if UNITY_ANDROID
        OpenWifiSettingsAndroid();
#endif

        // Paneli kapat
        PanelManager.Instance.HideAllPanel();
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