using UnityEngine;
[CreateAssetMenu(fileName = "New Break Dragon Egg Action", menuName = "Abilities/Actions/BreakDragonEggAction")]
public class AbilityActionBreakDragonEgg : AbilityAction
{
    public Tag tag;
    public int cost = 1400;
    public override void Execute()
    {
       // cost = AbilityManager.Instance.abilityDataHolder.abilities.Find(a => a.Type == AbilityType.BreakDragonEgg).cost;
        
        {
            Debug.Log("dragon egg action executed");
            AbilityManager.Instance.currentAbilityType= AbilityType.BreakDragonEgg;
            AbilityManager.Instance.breakDragonEggAction?.Invoke(tag);
            //ResourceManager.Instance.SpendResource(ResourceType.Coin, cost);
        }
       
    }
}
