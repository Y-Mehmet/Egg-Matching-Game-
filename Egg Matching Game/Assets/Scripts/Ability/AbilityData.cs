using UnityEngine;

// Bu enum, t�m yetenek t�rlerini tan�mlar. Yeni bir yetenek ekledi�inizde buraya ekleyeceksiniz.
public enum AbilityType
{
    FreezeTime,
    BreakSlot,
    BreakEgg,
    Shuffle,
    BreakDragonEgg,
}

// Bu sat�r sayesinde Unity'de "Create > Game > Ability" men�s� ile yeni yetenek verileri olu�turabilirsiniz.
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
    public bool RequiresTarget; // Bu yetenek bir hedef se�meyi gerektiriyor mu? (Yumurta, Slot vb.)

    [Header("Targeting")]
    [Tooltip("E�er hedef gerekiyorsa, hangi Tag'e sahip objeleri hedefleyebilir?")]
    public string TargetTag; // "Egg" veya "Slot" gibi

  
    [Header("Ability Action")]
    public AbilityAction action;
}