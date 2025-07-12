using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddPanel : MonoBehaviour
{
    [SerializeField] TMP_Text titleText, infoText;
    [SerializeField] Button spendCoinBtn, addBtn, closeBtn;
    private int energyCost = 900; // Enerji eklemek için gereken coin miktarý
    private void OnEnable()
    {
        // Butonlara dinleyiciler ekliyoruz
        spendCoinBtn.onClick.AddListener(OnSpendCoinButtonClicked);
        addBtn.onClick.AddListener(OnAddButtonClicked);
        closeBtn.onClick.AddListener(OnCloseButtonClicked);

        // Baþlýk ve bilgi metinlerini ayarlýyoruz
        titleText.text = "Add Resources";
        infoText.text = "Do you want to spend coins to add resources?";
        if (!ResourceManager.Instance.HasEnoughResource(ResourceType.Coin, energyCost))
        {
           spendCoinBtn.gameObject.SetActive(false);
        }
        else
        {
            spendCoinBtn.gameObject.SetActive(true);
        }
    }
    private void OnDisable()
    {
        // Dinleyicileri kaldýrýyoruz
        spendCoinBtn.onClick.RemoveListener(OnSpendCoinButtonClicked);
        addBtn.onClick.RemoveListener(OnAddButtonClicked);
        closeBtn.onClick.RemoveListener(OnCloseButtonClicked);
    }
    private void OnSpendCoinButtonClicked()
    {
        // Enerji ekleme iþlemi
        if (ResourceManager.Instance.HasEnoughResource(ResourceType.Coin, energyCost))
        {
            ResourceManager.Instance.AddResource(ResourceType.Energy, 1);
            ResourceManager.Instance.SpendResource(ResourceType.Coin, energyCost);
            
        }
        else
        {
            Debug.LogWarning("Not enough coins to spend.");
        }
    }
    private void OnAddButtonClicked()
    {
        
    }
    private void OnCloseButtonClicked()
    {
        PanelManager.Instance.HideLastPanel();
    }
}
