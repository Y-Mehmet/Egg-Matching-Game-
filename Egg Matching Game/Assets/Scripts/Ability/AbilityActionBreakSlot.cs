using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "New Break Slot Action", menuName = "Abilities/Actions/BreakSlotAction")]
public class AbilityActionBreakSlot : AbilityAction
{
    public Tag tag;
    public override void Execute()
    {
        
        AbilityManager.Instance.breakSlotAction?.Invoke(tag);
        AbilityManager.Instance.curentAbilityTypeChanged?.Invoke(AbilityType.BreakSlot);
    }

}
