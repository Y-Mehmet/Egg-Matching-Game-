using UnityEngine;
[CreateAssetMenu(fileName = "New Shuffle Action", menuName = "Abilities/Actions/ShuffleAction")]
public class AbilityActionShuffle : AbilityAction
{
    public int cost = 1900;
    public override void Execute()
    {

        cost = AbilityManager.Instance.abilityDataHolder.abilities.Find(a => a.Type == AbilityType.Shuffle).cost;

        if (ResourceManager.Instance.GetResourceAmount(ResourceType.Coin) >= cost)
        {
            AbilityManager.Instance.curentAbilityTypeChanged?.Invoke(AbilityType.Shuffle);
            GameManager.instance.Shufle();
            ResourceManager.Instance.SpendResource(ResourceType.Coin, cost);
        }
        else
        {
            AbilityManager.Instance.currentAbilityType = AbilityType.Shuffle;
            PanelManager.Instance.ShowPanel(PanelID.BuyAbilityPanel);

        }

    }
}
