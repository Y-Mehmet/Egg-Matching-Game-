using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "New Froze Action", menuName = "Abilities/Actions/FrozeeAction")]
public class AbilityActionFrozee : AbilityAction
{
    public int duration;
    private int cost = 900;
    public override void Execute()
    {
       cost = AbilityManager.Instance.abilityDataHolder.abilities.Find(a => a.Type == AbilityType.FreezeTime).cost;
     

        if (ResourceManager.Instance.GetResourceAmount(ResourceType.Coin)>=cost)
        {
            AbilityManager.Instance.curentAbilityTypeChanged?.Invoke(AbilityType.FreezeTime);
            AbilityManager.Instance.frezzeTimeAction?.Invoke(10);
            ResourceManager.Instance.SpendResource(ResourceType.Coin, cost);
            SoundManager.instance.PlaySfx(SoundType.Coin);
            SoundManager.instance.PlaySfx(SoundType.FreezeTime,0,false);
        }
            else
        {
            AbilityManager.Instance.currentAbilityType= AbilityType.FreezeTime;
            PanelManager.Instance.ShowPanel(PanelID.BuyAbilityPanel);
            SoundManager.instance.PlaySfx(SoundType.EmptyCoin);

        }
      

    }
   
}
