using UnityEngine;
[CreateAssetMenu(fileName = "New Break Egg Action", menuName = "Abilities/Actions/BreakEgg")]
public class AbilityActionBreakEgg : AbilityAction
{
    public Tag tag;
    public override void Execute()
    {
        AbilityManager.Instance.breakEggAction?.Invoke(tag);
        AbilityManager.Instance.curentAbilityTypeChanged?.Invoke(AbilityType.BreakEgg);

    }
}
