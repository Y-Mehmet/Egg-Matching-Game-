using UnityEngine;
[CreateAssetMenu(fileName = "New Shuffle Action", menuName = "Abilities/Actions/ShuffleAction")]
public class AbilityActionShuffle : AbilityAction
{
    public override void Execute()
    {
        AbilityManager.Instance.curentAbilityTypeChanged?.Invoke(AbilityType.Shuffle);
        GameManager.instance.Shufle();
        

    }
}
