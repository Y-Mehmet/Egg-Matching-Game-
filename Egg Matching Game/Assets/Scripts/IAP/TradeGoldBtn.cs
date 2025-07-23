using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeGoldBtn : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText, gemText;
    [SerializeField] private int coinAmount , gemAmount;
    [SerializeField] private Button tradeButton;


    private void OnEnable()
    {
        UIUpdate();
        tradeButton.GetComponent<Button>().onClick.AddListener(OnTradeButtonClicked);
        ResourceManager.Instance.onIAPchanged += UIUpdate;

    }
    private void OnDisable()
    {

        tradeButton.GetComponent<Button>().onClick.RemoveListener(OnTradeButtonClicked);
        ResourceManager.Instance.onIAPchanged -= UIUpdate;
    }
    private void UIUpdate()
    {
        if (coinText != null)
        {
            coinText.text = GetFormattedNumber(coinAmount);

        }
        if (gemText != null)
        {
            gemText.text = GetFormattedNumber(gemAmount);
            if (ResourceManager.Instance.GetResourceAmount(ResourceType.Gem) < gemAmount)
                gemText.color = Color.red;
            else
                gemText.color = Color.white;
        }
    }
    private void OnTradeButtonClicked()
    {
        if(ResourceManager.Instance.GetResourceAmount(ResourceType.Gem) >=gemAmount)
        {
            ResourceManager.Instance.Trade(gemAmount, coinAmount);
        }
    }
    public static string GetFormattedNumber(int number)
    {
        // Özellikle virgül kullanmak için "en-US" kültürünü belirtiyoruz
        return number.ToString("N0", new CultureInfo("en-US"));
    }
}
