// SettingManager.cs (Güncellenmiþ Tam Hali)

using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    // Singleton Pattern: Bu script'e her yerden kolayca eriþim saðlamak için.
    public static SettingManager Instance { get; private set; }

    [Header("UI Elemanlarý")]
    [SerializeField] private Button pushAlarmToggle;
    [SerializeField] private Slider soundFxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button vibrationToggle;


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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Dinleyicileri ekleme kýsmý ayný kalýyor
        pushAlarmToggle.onClick.AddListener(OnPushAlarmToggleChanged);
        soundFxSlider.onValueChanged.AddListener(OnSoundFxSliderChanged);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        vibrationToggle.onClick.AddListener(OnVibrationToggleChanged);

        // Oyunu açtýðýmýzda kayýtlý ayarlarý yeni sistemle yükle
        LoadSettings();
    }
    private void OnDisable()
    {
        // Dinleyicileri kaldýrma kýsmý ayný kalýyor
        pushAlarmToggle.onClick.RemoveListener(OnPushAlarmToggleChanged);
        soundFxSlider.onValueChanged.RemoveListener(OnSoundFxSliderChanged);
        musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        vibrationToggle.onClick.RemoveListener(OnVibrationToggleChanged);
        ResourceManager.Instance.SetSoundResoruce(IsPushAlarmEnabled,  SoundFxVolume, MusicVolume, IsVibrationEnabled); // Yeni sistemde ayarlarý kaydetme iþlemi
    }

 
   

    // --- DEÐÝÞTÝ ---
    /// <summary>
    /// Kayýt dosyasýndan (save.json) ayarlarý yükler ve UI'ý günceller.
    /// </summary>
    private void LoadSettings()
    {
        // 1. Kayýtlý oyun verilerini yükle
       

        // 2. Veri objesinden ayarlarý kendi deðiþkenlerimize ata
        IsPushAlarmEnabled = ResourceManager.Instance.isPushAlarmEnabled;
        IsVibrationEnabled = ResourceManager.Instance.isVibrationEnabled;
        SoundFxVolume =      ResourceManager.Instance.soundFxVolume;
        MusicVolume = ResourceManager.Instance.musicVolume;

        // 3. Yüklenen bu deðerleri UI elemanlarýna yansýt
        MoveHandle(pushAlarmToggle.transform, IsPushAlarmEnabled);
        MoveHandle(vibrationToggle.transform, IsVibrationEnabled);
        soundFxSlider.value = SoundFxVolume;
        musicSlider.value = MusicVolume;

        Debug.Log("Ayarlar yeni SaveSystem'den yüklendi.");
    }

    // --- DEÐÝÞTÝ ---
    // Deðiþiklikleri dinleyen metotlar artýk PlayerPrefs yerine SaveSettings() çaðýracak.

    public void OnPushAlarmToggleChanged()
    {
        IsPushAlarmEnabled = !IsPushAlarmEnabled;
        MoveHandle(pushAlarmToggle.transform, IsPushAlarmEnabled);

       

       
    }

    public void OnSoundFxSliderChanged(float value)
    {
        SoundFxVolume = value;

        SoundManager.instance.SetSfxVolume(value);

      
    }

    public void OnMusicSliderChanged(float value)
    {
        MusicVolume = value;



        SoundManager.instance.SetBgmVolume(value);
    }

    public void OnVibrationToggleChanged()
    {
        IsVibrationEnabled = !IsVibrationEnabled;
        MoveHandle(vibrationToggle.transform, IsVibrationEnabled);

       

        Debug.Log("Vibration ayarý deðiþtirildi: " + IsVibrationEnabled);
        if (IsVibrationEnabled)
        {
            // Test titreþimi...
        }
    }

    // MoveHandle metodu deðiþmeden ayný kalabilir.
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
                Color tempColor = toggleImage.color;
                tempColor.a = 1f;
                toggleImage.color = tempColor;
            }
            else
            {
                Vector2 newPosition = childRectTransform.anchoredPosition;
                newPosition.x = -50;
                childRectTransform.anchoredPosition = newPosition;
                Image toggleImage = Toggletransform.GetComponent<Image>();
                Color tempColor = toggleImage.color;
                tempColor.a = 0.05f;
                toggleImage.color = tempColor;
            }
        }
    }
}