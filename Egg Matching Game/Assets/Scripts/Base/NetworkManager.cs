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
            StartCoroutine(CheckConnectionRoutine()); // Ba�lant� kontrol�n� ba�lat
        }
        else
        {
            Destroy(gameObject); // Singleton prensibine g�re, zaten var olan bir �rne�i yok et
        }
    }


    void Start()
    {
        // Uygulama ba�lar ba�lamaz ba�lant�y� kontrol et
        CheckInternetConnection();
    }

    // Belirli aral�klarla ba�lant�y� kontrol etmek i�in bir metot (iste�e ba�l�)
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
            StopCoroutine(checkConnectionCoroutine); // �nceki kontrol� durdur
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
        // Wi-Fi ayarlar�na y�nlendir (Android �zel)
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
                // Wi-Fi ayarlar�na gitmek i�in intent olu�tur
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.settings.WIFI_SETTINGS");
                activity.Call("startActivity", intent);
            }
        }
    }

    // Uygulama tekrar odakland���nda ba�lant�y� kontrol et
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            CheckInternetConnection();
        }
    }
}