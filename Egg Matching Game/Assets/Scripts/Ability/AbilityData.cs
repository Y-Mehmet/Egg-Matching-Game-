using UnityEngine;

// Bu enum, tüm yetenek türlerini tanýmlar. Yeni bir yetenek eklediðinizde buraya ekleyeceksiniz.
public enum AbilityType
{
    FreezeTime,
    BreakSlot,
    BreakEgg,
    Shuffle,
    BreakDragonEgg,
}

// Bu satýr sayesinde Unity'de "Create > Game > Ability" menüsü ile yeni yetenek verileri oluþturabilirsiniz.
[CreateAssetMenu(fileName = "New Ability", menuName = "Game/Ability")]
public class AbilityData : ScriptableObject
{
    [Header("Ability Info")]
    public AbilityType Type;
    public string Name;
    public Sprite Icon;
    public int RequiredLevel;
    public string Title, Description;
    public GameObject ParticlePerfabs, AbilityWorkerPrefab;
    public int ParticleStartTime = 0;
    public int cost = 0;
    [Header("Behavior")]
    public bool RequiresTarget; // Bu yetenek bir hedef seçmeyi gerektiriyor mu? (Yumurta, Slot vb.)

    [Header("Targeting")]
    [Tooltip("Eðer hedef gerekiyorsa, hangi Tag'e sahip objeleri hedefleyebilir?")]
    public string TargetTag; // "Egg" veya "Slot" gibi

  
    [Header("Ability Action")]
    public AbilityAction action;
}