using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// Toggle ve Slider için bu kütüphane gerekli

public class SettingManager : MonoBehaviour
{
    // Singleton Pattern: Bu script'e her yerden kolayca eriþim saðlamak için.
    public static SettingManager Instance { get; private set; }

    [Header("UI Elemanlarý")]
    [SerializeField] private Button pushAlarmToggle;
    [SerializeField] private Slider soundFxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button vibrationToggle;

    // PlayerPrefs'te verileri saklamak için kullanacaðýmýz anahtarlar (key).
    // Bunlarý sabit (const) olarak tanýmlamak yazým hatalarýný önler.
    private const string PUSH_ALARM_KEY = "Settings_PushAlarm";
    private const string SOUND_FX_VOLUME_KEY = "Settings_SoundFxVolume";
    private const string MUSIC_VOLUME_KEY = "Settings_MusicVolume";
    private const string VIBRATION_KEY = "Settings_Vibration";

    // Diðer script'lerin ayar durumlarýný kolayca okuyabilmesi için public özellikler
    public bool IsVibrationEnabled { get; private set; }
    public float MusicVolume { get; private set; }
    public float SoundFxVolume { get; private set; }
    public bool IsPushAlarmEnabled { get; private set; }


    private void Awake()
    {
        // Singleton yapýsýný kuruyoruz
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ayarlarýn sahneler arasý geçiþte kaybolmamasýný saðlar
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // UI elemanlarýna dinleyicileri (listeners) ekle.
        // Bu sayede kullanýcý bir ayarý deðiþtirdiðinde ilgili metodumuz çalýþýr.
        pushAlarmToggle.onClick.AddListener(OnPushAlarmToggleChanged);
        soundFxSlider.onValueChanged.AddListener(OnSoundFxSliderChanged);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        vibrationToggle.onClick.AddListener(OnVibrationToggleChanged);

        // Oyunu açtýðýmýzda kayýtlý ayarlarý yükle
        LoadSettings();
    }

    /// <summary>
    /// Cihaz hafýzasýndan (PlayerPrefs) kayýtlý ayarlarý yükler ve UI'ý günceller.
    /// </summary>
    private void LoadSettings()
    {
        // Ayarlarý yüklerken, eðer daha önce hiç kaydedilmemiþse varsayýlan (default) deðerler atarýz.

        // Toggle'lar için int kullanýyoruz (1 = true, 0 = false)
        // Varsayýlan olarak bildirimler ve titreþim açýk olsun (1).
        IsPushAlarmEnabled = PlayerPrefs.GetInt(PUSH_ALARM_KEY, 1) == 1;
        IsVibrationEnabled = PlayerPrefs.GetInt(VIBRATION_KEY, 1) == 1;
        MoveHandle(pushAlarmToggle.transform, IsPushAlarmEnabled);
        MoveHandle(vibrationToggle.transform, IsVibrationEnabled);

        // Slider'lar için float kullanýyoruz.
        // Varsayýlan olarak ses seviyeleri %80 olsun (0.8f).
        SoundFxVolume = PlayerPrefs.GetFloat(SOUND_FX_VOLUME_KEY, 0.8f);
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.8f);

        // Yüklenen bu deðerleri UI elemanlarýna yansýt
        // `SetValueWithoutNotify` kullanmýyoruz çünkü ilk açýlýþta diðer sistemlerin de (örn. AudioManager)
        // bu deðerleri almasýný isteyebiliriz. Direkt atama yapmak `onValueChanged` event'ini tetikler.
       
        soundFxSlider.value = SoundFxVolume;
        musicSlider.value = MusicVolume;

        Debug.Log("Ayarlar yüklendi.");
    }

    // --- Deðiþiklikleri Dinleyen Metotlar ---

    public void OnPushAlarmToggleChanged()
    {
        
       

        // BURAYA PUSH NOTIFICATION SÝSTEMÝNÝ AÇIP KAPATAN KOD GELECEK
        // Örnek: NotificationManager.Instance.SetNotifications(isOn);
        IsPushAlarmEnabled = !IsPushAlarmEnabled;
        MoveHandle(pushAlarmToggle.transform, IsPushAlarmEnabled);



        PlayerPrefs.SetInt(PUSH_ALARM_KEY, IsPushAlarmEnabled ? 1 : 0);
        Debug.Log("Push Alarm ayarý deðiþtirildi: " + IsPushAlarmEnabled);

        
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

                // 2. Mevcut rengin bir kopyasýný yeni bir deðiþkene al
                Color tempColor = toggleImage.color;

                // 3. Bu kopyanýn alpha deðerini deðiþtir
                tempColor.a = 1f;

                // 4. Deðiþtirilmiþ rengi bütünüyle geri ata
                toggleImage.color = tempColor;
            }
            else
            {
                Vector2 newPosition = childRectTransform.anchoredPosition;
                newPosition.x = -50;
                childRectTransform.anchoredPosition = newPosition;
                Image toggleImage = Toggletransform.GetComponent<Image>();

                // 2. Mevcut rengin bir kopyasýný yeni bir deðiþkene al
                Color tempColor = toggleImage.color;

                // 3. Bu kopyanýn alpha deðerini deðiþtir
                tempColor.a = 0.05f;

                // 4. Deðiþtirilmiþ rengi bütünüyle geri ata
                toggleImage.color = tempColor;
            }

        }
    }

    public void OnSoundFxSliderChanged(float value)
    {
        SoundFxVolume = value;
        PlayerPrefs.SetFloat(SOUND_FX_VOLUME_KEY, value);
        Debug.Log("Sound FX Volume ayarý deðiþtirildi: " + value);

        // BURAYA OYUN ÝÇÝ SES EFEKTLERÝNÝN SESÝNÝ DEÐÝÞTÝREN KOD GELECEK
        // Örnek: AudioManager.Instance.SetSoundFxVolume(value);
    }

    public void OnMusicSliderChanged(float value)
    {
        MusicVolume = value;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        Debug.Log("Music Volume ayarý deðiþtirildi: " + value);

        // BURAYA OYUN MÜZÝÐÝNÝN SESÝNÝ DEÐÝÞTÝREN KOD GELECEK
        // Örnek: AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnVibrationToggleChanged()
    {
        IsVibrationEnabled = !IsVibrationEnabled;
        MoveHandle(vibrationToggle.transform, IsVibrationEnabled);
        PlayerPrefs.SetInt(VIBRATION_KEY, IsVibrationEnabled ? 1 : 0);
        Debug.Log("Vibration ayarý deðiþtirildi: " +   IsVibrationEnabled );

        // Titreþim ayarý deðiþtiðinde örnek bir titreþim göndererek kullanýcýya geri bildirim verilebilir.
        if (    IsVibrationEnabled)
        {
            // Handheld.Vibrate(); // Basit titreþim için (Not: Bu metot artýk önerilmiyor, yeni sistemler var)
            Debug.Log("Titreþim AÇIK, test titreþimi gönderiliyor...");
        }
    }

    private void OnDisable()
    {
        // Obje deaktif olduðunda veya yok olduðunda dinleyicileri kaldýrmak iyi bir pratiktir.
        // Bu, olasý hafýza sýzýntýlarýný ve hatalarý önler.
        pushAlarmToggle.onClick.RemoveAllListeners();
        soundFxSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        vibrationToggle.onClick.RemoveAllListeners();
    }
}