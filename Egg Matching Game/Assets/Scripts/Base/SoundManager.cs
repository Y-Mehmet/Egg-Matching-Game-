using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Ses Veritabaný")]
    [SerializeField] private SoundDatabaseSO soundDatabase;

    [Header("Audio Source'lar")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    // Sesleri hýzlýca bulmak için bir sözlük (dictionary) kullanýyoruz.
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

        // Veritabanýndaki sesleri sözlüðe yükle
        InitializeSounds();
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
                Debug.LogWarning("SoundManager: '" + sound.soundType + "' için zaten bir ses klibi mevcut!");
            }
        }
       

    }

    // Arka plan müziðini çalmak için
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
            Debug.LogWarning("SoundManager: '" + soundType + "' isimli BGM bulunamadý!");
        }
    }

    // Ses efektlerini çalmak için
    public void PlaySfx(SoundType soundType=SoundType.btnClick)
    {
        if (soundDictionary.TryGetValue(soundType, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SoundManager: '" + soundType + "' isimli SFX bulunamadý!");
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
            Debug.LogWarning("SoundManager: btn isimli SFX bulunamadý!");
        }
    }
    public void StopBG()
    {
        bgmSource.Stop();
    }

    // Ses ayarlarý için fonksiyonlar (opsiyonel, önceki örnekteki gibi eklenebilir)
    public void SetBgmVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = volume;
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