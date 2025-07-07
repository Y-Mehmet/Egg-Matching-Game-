using UnityEngine;
[CreateAssetMenu(fileName = "New Shuffle Action", menuName = "Abilities/Actions/ShuffleAction")]
public class AbilityActionShuffle : AbilityAction
{
    public override void Execute()
    {
        // AbilityManager.Instance.shuffleAction?.Invoke();
        GameManager.instance.Shufle();

    }
}
