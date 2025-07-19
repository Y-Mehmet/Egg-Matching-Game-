using UnityEngine;
[CreateAssetMenu(fileName = "New Break Egg Action", menuName = "Abilities/Actions/BreakEgg")]
public class AbilityActionBreakEgg : AbilityAction
{
    public Tag tag;
    public int cost = 1400;
    public override void Execute()
    {
        cost = AbilityManager.Instance.abilityDataHolder.abilities.Find(a => a.Type == AbilityType.BreakEgg).cost;
        if (ResourceManager.Instance.GetResourceAmount(ResourceType.Coin) >= cost)
        {
            AbilityManager.Instance.curentAbilityTypeChanged?.Invoke(AbilityType.BreakEgg);
            AbilityManager.Instance.breakEggAction?.Invoke(tag);

            ResourceManager.Instance.SpendResource(ResourceType.Coin, cost);
        }
        else
        {
            AbilityManager.Instance.currentAbilityType = AbilityType.BreakEgg;
            PanelManager.Instance.ShowPanel(PanelID.BuyAbilityPanel);

        }
    }
}
