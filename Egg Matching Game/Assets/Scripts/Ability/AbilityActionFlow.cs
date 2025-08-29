
using UnityEngine;
[CreateAssetMenu(fileName = "New Flow Action", menuName = "Abilities/Actions/FlowAction")]
public class AbilityActionFlow : AbilityAction
{
   
    private int cost = 0;
    public override void Execute()
    {
        cost = AbilityManager.Instance.abilityDataHolder.abilities.Find(a => a.Type == AbilityType.Flow).cost;
        if (ResourceManager.Instance.GetResourceAmount(ResourceType.Coin) >= cost)
        {
            AbilityManager.Instance.curentAbilityTypeChanged?.Invoke(AbilityType.Flow);
            AbilityManager.Instance.flowAction?.Invoke();
            ResourceManager.Instance.SpendResource(ResourceType.Coin, cost);
            SoundManager.instance.PlaySfx(SoundType.Coin);
        }
        else
        {
            AbilityManager.Instance.currentAbilityType = AbilityType.Flow;
            PanelManager.Instance.ShowPanel(PanelID.BuyAbilityPanel);
            SoundManager.instance.PlaySfx(SoundType.EmptyCoin);

        }
    }

}

