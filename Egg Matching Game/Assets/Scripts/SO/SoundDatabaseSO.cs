using UnityEngine;

// Bu sýnýf, bir ses klibini onun enum türüyle eþleþtirmek için kullanýlýr.
[System.Serializable]
public class Sound
{
    public SoundType soundType;
    public AudioClip audioClip;
    [Range(0f, 1f)]
    public float volume = .3f; // Sesin varsayýlan ses seviyesi
    public bool loop = false; // Sesin döngüde çalýnýp çalýnmayacaðý

}

// Bu attribute, Unity menüsüne yeni bir asset oluþturma seçeneði ekler.
[CreateAssetMenu(fileName = "SoundDatabase", menuName = "ScriptableObjects/Sound Database")]
public class SoundDatabaseSO : ScriptableObject
{
    public Sound[] sounds;
}