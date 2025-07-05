using UnityEngine;

// Bu enum, t�m yetenek t�rlerini tan�mlar. Yeni bir yetenek ekledi�inizde buraya ekleyeceksiniz.
public enum AbilityType
{
    FreezeTime,
    BreakSlot,
    BreakEgg
}

// Bu sat�r sayesinde Unity'de "Create > Game > Ability" men�s� ile yeni yetenek verileri olu�turabilirsiniz.
[CreateAssetMenu(fileName = "New Ability", menuName = "Game/Ability")]
public class AbilityData : ScriptableObject
{
    [Header("Ability Info")]
    public AbilityType Type;
    public string Name;
    public Sprite Icon;

    [Header("Behavior")]
    public bool RequiresTarget; // Bu yetenek bir hedef se�meyi gerektiriyor mu? (Yumurta, Slot vb.)

    [Header("Targeting")]
    [Tooltip("E�er hedef gerekiyorsa, hangi Tag'e sahip objeleri hedefleyebilir?")]
    public string TargetTag; // "Egg" veya "Slot" gibi

    [Header("Effect Settings")]
    [Tooltip("Zaman Dondurma i�in saniye cinsinden s�re")]
    public float Duration = 10f; // Sadece FreezeTime i�in ge�erli
}