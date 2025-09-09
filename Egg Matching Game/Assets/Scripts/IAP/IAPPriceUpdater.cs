using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPPriceUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private CodelessIAPButton IAP;

    private void Start()
    {
        var product = CodelessIAPStoreListener.Instance.GetProduct(IAP.productId);
        if (product != null && product.metadata != null)
        {
            priceText.text = "Price: "+product.metadata.localizedPriceString;
        }
    }
}
