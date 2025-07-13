using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Coroutine kullanmak için bu satýr gerekli!

public class AddPanel : MonoBehaviour
{
    [SerializeField] TMP_Text titleText, infoText;
    [SerializeField] Button spendCoinBtn, addBtn, closeBtn;

    private int energyCost = 900;

    // Coroutine'i yönetmek için bir referans tutuyoruz.
    private Coroutine updateCoroutine;

    private void OnEnable()
    {
        // Butonlara dinleyiciler ekliyoruz
        spendCoinBtn.onClick.AddListener(OnSpendCoinButtonClicked);
        addBtn.onClick.AddListener(OnAddButtonClicked);
        closeBtn.onClick.AddListener(OnCloseButtonClicked);

        // Coroutine'i baþlatýyoruz ve referansýný saklýyoruz
        updateCoroutine = StartCoroutine(UpdateUICoroutine());

        // UI'ýn ilk durumunu ayarlýyoruz (Coroutine baþlamadan önce bir kez çalýþsýn)
        InitialUISetup();
    }

    private void OnDisable()
    {
        // Panel devre dýþý kaldýðýnda dinleyicileri ve Coroutine'i durduruyoruz.
        // Bu, panel kapalýyken gereksiz yere iþlem yapýlmasýný önler.
        spendCoinBtn.onClick.RemoveListener(OnSpendCoinButtonClicked);
        addBtn.onClick.RemoveListener(OnAddButtonClicked);
        closeBtn.onClick.RemoveListener(OnCloseButtonClicked);

        // Coroutine'i durdur
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null; // Referansý temizle
        }
    }

    // UI'ýn baþlangýç ayarlarýný yapan metot
    private void InitialUISetup()
    {
        titleText.text = "Add Resources";

        // Coin ile satýn alma butonunun görünürlüðünü kontrol et
        if (!ResourceManager.Instance.HasEnoughResource(ResourceType.Coin, energyCost))
        {
            spendCoinBtn.gameObject.SetActive(false);
        }
        else
        {
            spendCoinBtn.gameObject.SetActive(true);
        }
    }

    // UI'ý her saniye güncelleyecek olan Coroutine
    private IEnumerator UpdateUICoroutine()
    {
        // Bu döngü, Coroutine durdurulana kadar devam eder.
        while (true)
        {
            string time = ResourceManager.Instance.GetNextEnergyCountdownText();
            infoText.text = $"Next dragon power in\n<color=yellow>{time}</color>"; // Rengi daha belirgin yapabiliriz.

            // 1 saniye bekle ve döngüye devam et.
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnSpendCoinButtonClicked()
    {
        if (ResourceManager.Instance.SpendResource(ResourceType.Coin, energyCost))
        {
            ResourceManager.Instance.AddResource(ResourceType.Energy, 1);
            // Satýn alýmdan sonra buton durumu tekrar kontrol edilebilir, 
            // ama OnEnable/OnDisable döngüsü bunu zaten yapacaktýr.
        }
        else
        {
            Debug.LogWarning("Not enough coins to spend.");
        }
    }

    private void OnAddButtonClicked()
    {
        if (AdsManager.Instance != null && AdsManager.Instance.rewardedAds != null)
        {
            GameManager.instance.currentRewardedTypeChanged?.Invoke(RewardedType.Energy);
            AdsManager.Instance.rewardedAds.ShowRewardedAd();
            Debug.Log("Ödüllü Reklam Çaðrýldý!");
        }
        else
        {
            Debug.LogWarning("Ödüllü Reklam hazýr deðil veya AdsManager bulunamadý.");
        }
    }

    private void OnCloseButtonClicked()
    {
        PanelManager.Instance.HideLastPanel();
    }
}