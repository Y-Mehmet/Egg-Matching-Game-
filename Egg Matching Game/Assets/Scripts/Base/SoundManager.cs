using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Ses Veritaban�")]
    [SerializeField] private SoundDatabaseSO soundDatabase;

    [Header("Audio Source'lar")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    // Sesleri h�zl�ca bulmak i�in bir s�zl�k (dictionary) kullan�yoruz.
    private Dictionary<SoundType, AudioClip> soundDictionary;

    private void Awake()
    {
        // Singleton Deseni
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Veritaban�ndaki sesleri s�zl��e y�kle
        InitializeSounds();
    }
    private void Start()
    {

        bgmSource.volume = ResourceManager.Instance.musicVolume;
        sfxSource.volume = ResourceManager.Instance.soundFxVolume;
    }
    private void InitializeSounds()
    {
        soundDictionary = new Dictionary<SoundType, AudioClip>();
        foreach (var sound in soundDatabase.sounds)
        {
            if (!soundDictionary.ContainsKey(sound.soundType))
            {
                soundDictionary.Add(sound.soundType, sound.audioClip);
            }
            else
            {
                Debug.LogWarning("SoundManager: '" + sound.soundType + "' i�in zaten bir ses klibi mevcut!");
            }
        }
       

    }

    // Arka plan m�zi�ini �almak i�in
    public void PlayBgm(SoundType soundType)
    {
        if (soundDictionary.TryGetValue(soundType, out AudioClip clip))
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("SoundManager: '" + soundType + "' isimli BGM bulunamad�!");
        }
    }

    // Ses efektlerini �almak i�in
    public void PlaySfx(SoundType soundType, float startTime = 0f, bool PlayOneShot=true, float playbackSpeed = 1.0f)
    {
        if (soundDictionary.TryGetValue(soundType, out AudioClip clip))
        {
            sfxSource.pitch= Mathf.Max(0.1f, playbackSpeed);
            if (startTime <= 0f || PlayOneShot)
            {
                sfxSource.PlayOneShot(clip);
            }
            // E�er belirli bir saniyeden ba�lamas� isteniyorsa:
            else
            {
                // D�KKAT: Bu y�ntem sfxSource'da �alan mevcut sesi durdurur!
                sfxSource.clip = clip;      // 1. Klibi ata
                sfxSource.time = startTime; // 2. Ba�lang�� saniyesini ayarla
                sfxSource.Play();           // 3. Oynat
            }
        }
        else
        {
            Debug.LogWarning("SoundManager: '" + soundType + "' isimli SFX bulunamad�!");
        }
    }
    public void PlayBtnClick()
    {
        if (soundDictionary.TryGetValue(SoundType.btnClick, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SoundManager: btn isimli SFX bulunamad�!");
        }
    }
    public void StopBG()
    {
        bgmSource.Stop();
    }
    public void StopClip(SoundType soundType)
    {
        if (soundDictionary.TryGetValue(soundType, out AudioClip clip))
        {
            if (sfxSource.clip == clip)
                sfxSource.Stop();
        }
    }

    // Ses ayarlar� i�in fonksiyonlar (opsiyonel, �nceki �rnekteki gibi eklenebilir)
    public void SetBgmVolume(float volume)
    {
        bgmSource.volume = volume;
        ResourceManager.Instance.musicVolume = volume;
        ResourceManager.Instance.SaveResources();
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = volume;
        ResourceManager.Instance.soundFxVolume = volume;
        ResourceManager.Instance.SaveResources();
        
    }
    
}
public enum SoundType
{
    FreezeTime,
    BreakSlot,
    BreakEgg,
    Shuffle,
    BreakDragonEgg,
    Coin,
    Gem,
    Energy,
    Check,
    GoToSlot,
    GoToStart,
    BG,
    btnClick,
    EmptyCoin, 
    Tiktak,
    BrokenEgg,
    LevelUp,
    Temp,
    


}