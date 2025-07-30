// SettingManager.cs (G�ncellenmi� Tam Hali)

using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    // Singleton Pattern: Bu script'e her yerden kolayca eri�im sa�lamak i�in.
    public static SettingManager Instance { get; private set; }

    [Header("UI Elemanlar�")]
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
        // Singleton yap�s�n� kuruyoruz
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
        // Dinleyicileri ekleme k�sm� ayn� kal�yor
        pushAlarmToggle.onClick.AddListener(OnPushAlarmToggleChanged);
        soundFxSlider.onValueChanged.AddListener(OnSoundFxSliderChanged);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        vibrationToggle.onClick.AddListener(OnVibrationToggleChanged);

        // Oyunu a�t���m�zda kay�tl� ayarlar� yeni sistemle y�kle
        LoadSettings();
    }
    private void OnDisable()
    {
        // Dinleyicileri kald�rma k�sm� ayn� kal�yor
        pushAlarmToggle.onClick.RemoveListener(OnPushAlarmToggleChanged);
        soundFxSlider.onValueChanged.RemoveListener(OnSoundFxSliderChanged);
        musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        vibrationToggle.onClick.RemoveListener(OnVibrationToggleChanged);
        ResourceManager.Instance.SetSoundResoruce(IsPushAlarmEnabled,  SoundFxVolume, MusicVolume, IsVibrationEnabled); // Yeni sistemde ayarlar� kaydetme i�lemi
    }

 
   

    // --- DE���T� ---
    /// <summary>
    /// Kay�t dosyas�ndan (save.json) ayarlar� y�kler ve UI'� g�nceller.
    /// </summary>
    private void LoadSettings()
    {
        // 1. Kay�tl� oyun verilerini y�kle
       

        // 2. Veri objesinden ayarlar� kendi de�i�kenlerimize ata
        IsPushAlarmEnabled = ResourceManager.Instance.isPushAlarmEnabled;
        IsVibrationEnabled = ResourceManager.Instance.isVibrationEnabled;
        SoundFxVolume =      ResourceManager.Instance.soundFxVolume;
        MusicVolume = ResourceManager.Instance.musicVolume;

        // 3. Y�klenen bu de�erleri UI elemanlar�na yans�t
        MoveHandle(pushAlarmToggle.transform, IsPushAlarmEnabled);
        MoveHandle(vibrationToggle.transform, IsVibrationEnabled);
        soundFxSlider.value = SoundFxVolume;
        musicSlider.value = MusicVolume;

        Debug.Log("Ayarlar yeni SaveSystem'den y�klendi.");
    }

    // --- DE���T� ---
    // De�i�iklikleri dinleyen metotlar art�k PlayerPrefs yerine SaveSettings() �a��racak.

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

       

        Debug.Log("Vibration ayar� de�i�tirildi: " + IsVibrationEnabled);
        if (IsVibrationEnabled)
        {
            // Test titre�imi...
        }
    }

    // MoveHandle metodu de�i�meden ayn� kalabilir.
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