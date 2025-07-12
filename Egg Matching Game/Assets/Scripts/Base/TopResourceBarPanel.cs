using TMPro;
using UnityEngine;

public class TopResourceBarPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text energyText, coinText, gemText; 
    private void OnEnable()
    {
        if(ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += UpdateResourceDisplay;
        }
        UpdateUI();
    }
    private void OnDisable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= UpdateResourceDisplay;
        }
    }
    private void UpdateResourceDisplay(ResourceType type, int amount)
    {
       switch(type)
        {
            case ResourceType.Coin:
                coinText.text = amount.ToString();
                break;
            case ResourceType.Gem:
                gemText.text = amount.ToString();
                break;
            case ResourceType.Energy:
                energyText.text = amount+"/5";
                break;
            default:
                Debug.LogWarning("Unknown resource type: " + type);
                break;
        }
    }
    private void UpdateUI()
    {
        coinText.text =ResourceManager.Instance.GetResourceAmount(ResourceType.Coin).ToString();
        gemText.text = ResourceManager.Instance.GetResourceAmount(ResourceType.Gem).ToString();
        energyText.text = ResourceManager.Instance.GetResourceAmount(ResourceType.Energy) + "/5";
    }

}
