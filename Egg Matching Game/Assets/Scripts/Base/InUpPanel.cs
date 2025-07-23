using TMPro;
using UnityEngine;

public class InUpPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text gemText, coinText;

    private void OnEnable()
    {
        ResourceManager.Instance.OnResourceChanged += UIUpdate;
        UIUpdate(ResourceType.Energy,0);
    }
    private void UIUpdate(ResourceType r, int i )
    {
        gemText.text = ResourceManager.Instance.GetResourceAmount(ResourceType.Gem).ToString();
        coinText.text = ResourceManager.Instance.GetResourceAmount(ResourceType.Coin).ToString();
    }
    
}
