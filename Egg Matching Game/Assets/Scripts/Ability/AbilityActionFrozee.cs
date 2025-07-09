using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "New Froze Action", menuName = "Abilities/Actions/FrozeeAction")]
public class AbilityActionFrozee : AbilityAction
{
    public int duration;
    public override void Execute()
    {
        AbilityManager.Instance.frezzeTimeAction?.Invoke(10);
        AbilityManager.Instance.curentAbilityTypeChanged?.Invoke(AbilityType.FreezeTime);

    }
   
}
