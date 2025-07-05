using UnityEngine;

// Bu enum, tüm yetenek türlerini tanýmlar. Yeni bir yetenek eklediðinizde buraya ekleyeceksiniz.
public enum AbilityType
{
    FreezeTime,
    BreakSlot,
    BreakEgg
}

// Bu satýr sayesinde Unity'de "Create > Game > Ability" menüsü ile yeni yetenek verileri oluþturabilirsiniz.
[CreateAssetMenu(fileName = "New Ability", menuName = "Game/Ability")]
public class AbilityData : ScriptableObject
{
    [Header("Ability Info")]
    public AbilityType Type;
    public string Name;
    public Sprite Icon;

    [Header("Behavior")]
    public bool RequiresTarget; // Bu yetenek bir hedef seçmeyi gerektiriyor mu? (Yumurta, Slot vb.)

    [Header("Targeting")]
    [Tooltip("Eðer hedef gerekiyorsa, hangi Tag'e sahip objeleri hedefleyebilir?")]
    public string TargetTag; // "Egg" veya "Slot" gibi

    [Header("Effect Settings")]
    [Tooltip("Zaman Dondurma için saniye cinsinden süre")]
    public float Duration = 10f; // Sadece FreezeTime için geçerli
}