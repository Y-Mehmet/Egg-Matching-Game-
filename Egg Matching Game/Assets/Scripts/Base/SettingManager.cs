using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// Toggle ve Slider i�in bu k�t�phane gerekli

public class SettingManager : MonoBehaviour
{
    // Singleton Pattern: Bu script'e her yerden kolayca eri�im sa�lamak i�in.
    public static SettingManager Instance { get; private set; }

    [Header("UI Elemanlar�")]
    [SerializeField] private Button pushAlarmToggle;
    [SerializeField] private Slider soundFxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button vibrationToggle;

    // PlayerPrefs'te verileri saklamak i�in kullanaca��m�z anahtarlar (key).
    // Bunlar� sabit (const) olarak tan�mlamak yaz�m hatalar�n� �nler.
    private const string PUSH_ALARM_KEY = "Settings_PushAlarm";
    private const string SOUND_FX_VOLUME_KEY = "Settings_SoundFxVolume";
    private const string MUSIC_VOLUME_KEY = "Settings_MusicVolume";
    private const string VIBRATION_KEY = "Settings_Vibration";

    // Di�er script'lerin ayar durumlar�n� kolayca okuyabilmesi i�in public �zellikler
    public bool IsVibrationEnabled { get; private set; }
    public float MusicVolume { get; private set; }
    public float SoundFxVolume { get; private set; }
    public bool IsPushAlarmEnabled { get; private set; }


    private void Awake()
    {
        // Singleton yap�s�n� kuruyoruz
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ayarlar�n sahneler aras� ge�i�te kaybolmamas�n� sa�lar
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // UI elemanlar�na dinleyicileri (listeners) ekle.
        // Bu sayede kullan�c� bir ayar� de�i�tirdi�inde ilgili metodumuz �al���r.
        pushAlarmToggle.onClick.AddListener(OnPushAlarmToggleChanged);
        soundFxSlider.onValueChanged.AddListener(OnSoundFxSliderChanged);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        vibrationToggle.onClick.AddListener(OnVibrationToggleChanged);

        // Oyunu a�t���m�zda kay�tl� ayarlar� y�kle
        LoadSettings();
    }

    /// <summary>
    /// Cihaz haf�zas�ndan (PlayerPrefs) kay�tl� ayarlar� y�kler ve UI'� g�nceller.
    /// </summary>
    private void LoadSettings()
    {
        // Ayarlar� y�klerken, e�er daha �nce hi� kaydedilmemi�se varsay�lan (default) de�erler atar�z.

        // Toggle'lar i�in int kullan�yoruz (1 = true, 0 = false)
        // Varsay�lan olarak bildirimler ve titre�im a��k olsun (1).
        IsPushAlarmEnabled = PlayerPrefs.GetInt(PUSH_ALARM_KEY, 1) == 1;
        IsVibrationEnabled = PlayerPrefs.GetInt(VIBRATION_KEY, 1) == 1;
        MoveHandle(pushAlarmToggle.transform, IsPushAlarmEnabled);
        MoveHandle(vibrationToggle.transform, IsVibrationEnabled);

        // Slider'lar i�in float kullan�yoruz.
        // Varsay�lan olarak ses seviyeleri %80 olsun (0.8f).
        SoundFxVolume = PlayerPrefs.GetFloat(SOUND_FX_VOLUME_KEY, 0.8f);
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.8f);

        // Y�klenen bu de�erleri UI elemanlar�na yans�t
        // `SetValueWithoutNotify` kullanm�yoruz ��nk� ilk a��l��ta di�er sistemlerin de (�rn. AudioManager)
        // bu de�erleri almas�n� isteyebiliriz. Direkt atama yapmak `onValueChanged` event'ini tetikler.
       
        soundFxSlider.value = SoundFxVolume;
        musicSlider.value = MusicVolume;

        Debug.Log("Ayarlar y�klendi.");
    }

    // --- De�i�iklikleri Dinleyen Metotlar ---

    public void OnPushAlarmToggleChanged()
    {
        
       

        // BURAYA PUSH NOTIFICATION S�STEM�N� A�IP KAPATAN KOD GELECEK
        // �rnek: NotificationManager.Instance.SetNotifications(isOn);
        IsPushAlarmEnabled = !IsPushAlarmEnabled;
        MoveHandle(pushAlarmToggle.transform, IsPushAlarmEnabled);



        PlayerPrefs.SetInt(PUSH_ALARM_KEY, IsPushAlarmEnabled ? 1 : 0);
        Debug.Log("Push Alarm ayar� de�i�tirildi: " + IsPushAlarmEnabled);

        
    }
    private void MoveHandle(Transform Toggletransform, bool isOn)
    {
        Transform childTransform = Toggletransform.GetChild(0);
        RectTransform childRectTransform = childTransform.GetComponent<RectTransform>();

        if (childRectTransform != null)
        {
            if (isOn)
            {
                Vector2 newPosition = childRectTransform.anchoredPosition;
                newPosition.x = 50;
                childRectTransform.anchoredPosition = newPosition;
                Image toggleImage = Toggletransform.GetComponent<Image>();

                // 2. Mevcut rengin bir kopyas�n� yeni bir de�i�kene al
                Color tempColor = toggleImage.color;

                // 3. Bu kopyan�n alpha de�erini de�i�tir
                tempColor.a = 1f;

                // 4. De�i�tirilmi� rengi b�t�n�yle geri ata
                toggleImage.color = tempColor;
            }
            else
            {
                Vector2 newPosition = childRectTransform.anchoredPosition;
                newPosition.x = -50;
                childRectTransform.anchoredPosition = newPosition;
                Image toggleImage = Toggletransform.GetComponent<Image>();

                // 2. Mevcut rengin bir kopyas�n� yeni bir de�i�kene al
                Color tempColor = toggleImage.color;

                // 3. Bu kopyan�n alpha de�erini de�i�tir
                tempColor.a = 0.05f;

                // 4. De�i�tirilmi� rengi b�t�n�yle geri ata
                toggleImage.color = tempColor;
            }

        }
    }

    public void OnSoundFxSliderChanged(float value)
    {
        SoundFxVolume = value;
        PlayerPrefs.SetFloat(SOUND_FX_VOLUME_KEY, value);
        Debug.Log("Sound FX Volume ayar� de�i�tirildi: " + value);

        // BURAYA OYUN ��� SES EFEKTLER�N�N SES�N� DE���T�REN KOD GELECEK
        // �rnek: AudioManager.Instance.SetSoundFxVolume(value);
    }

    public void OnMusicSliderChanged(float value)
    {
        MusicVolume = value;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        Debug.Log("Music Volume ayar� de�i�tirildi: " + value);

        // BURAYA OYUN M�Z���N�N SES�N� DE���T�REN KOD GELECEK
        // �rnek: AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnVibrationToggleChanged()
    {
        IsVibrationEnabled = !IsVibrationEnabled;
        MoveHandle(vibrationToggle.transform, IsVibrationEnabled);
        PlayerPrefs.SetInt(VIBRATION_KEY, IsVibrationEnabled ? 1 : 0);
        Debug.Log("Vibration ayar� de�i�tirildi: " +   IsVibrationEnabled );

        // Titre�im ayar� de�i�ti�inde �rnek bir titre�im g�ndererek kullan�c�ya geri bildirim verilebilir.
        if (    IsVibrationEnabled)
        {
            // Handheld.Vibrate(); // Basit titre�im i�in (Not: Bu metot art�k �nerilmiyor, yeni sistemler var)
            Debug.Log("Titre�im A�IK, test titre�imi g�nderiliyor...");
        }
    }

    private void OnDisable()
    {
        // Obje deaktif oldu�unda veya yok oldu�unda dinleyicileri kald�rmak iyi bir pratiktir.
        // Bu, olas� haf�za s�z�nt�lar�n� ve hatalar� �nler.
        pushAlarmToggle.onClick.RemoveAllListeners();
        soundFxSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        vibrationToggle.onClick.RemoveAllListeners();
    }
}