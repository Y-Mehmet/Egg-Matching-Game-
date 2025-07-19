using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "New Break Slot Action", menuName = "Abilities/Actions/BreakSlotAction")]
public class AbilityActionBreakSlot : AbilityAction
{
    public Tag tag;
    public int cost = 1400;
    public override void Execute()
    {
        cost = AbilityManager.Instance.abilityDataHolder.abilities.Find(a => a.Type == AbilityType.BreakSlot).cost;
        if (ResourceManager.Instance.GetResourceAmount(ResourceType.Coin) >= cost)
        {
            AbilityManager.Instance.curentAbilityTypeChanged?.Invoke(AbilityType.BreakSlot);
            AbilityManager.Instance.breakSlotAction?.Invoke(tag);
            ResourceManager.Instance.SpendResource(ResourceType.Coin, cost);
        }
        else
        {
            AbilityManager.Instance.currentAbilityType = AbilityType.BreakSlot;
            PanelManager.Instance.ShowPanel(PanelID.BuyAbilityPanel);

        }
    }

}
