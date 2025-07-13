using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Coroutine kullanmak i�in bu sat�r gerekli!

public class AddPanel : MonoBehaviour
{
    [SerializeField] TMP_Text titleText, infoText;
    [SerializeField] Button spendCoinBtn, addBtn, closeBtn;

    private int energyCost = 900;

    // Coroutine'i y�netmek i�in bir referans tutuyoruz.
    private Coroutine updateCoroutine;

    private void OnEnable()
    {
        // Butonlara dinleyiciler ekliyoruz
        spendCoinBtn.onClick.AddListener(OnSpendCoinButtonClicked);
        addBtn.onClick.AddListener(OnAddButtonClicked);
        closeBtn.onClick.AddListener(OnCloseButtonClicked);

        // Coroutine'i ba�lat�yoruz ve referans�n� sakl�yoruz
        updateCoroutine = StartCoroutine(UpdateUICoroutine());

        // UI'�n ilk durumunu ayarl�yoruz (Coroutine ba�lamadan �nce bir kez �al��s�n)
        InitialUISetup();
    }

    private void OnDisable()
    {
        // Panel devre d��� kald���nda dinleyicileri ve Coroutine'i durduruyoruz.
        // Bu, panel kapal�yken gereksiz yere i�lem yap�lmas�n� �nler.
        spendCoinBtn.onClick.RemoveListener(OnSpendCoinButtonClicked);
        addBtn.onClick.RemoveListener(OnAddButtonClicked);
        closeBtn.onClick.RemoveListener(OnCloseButtonClicked);

        // Coroutine'i durdur
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null; // Referans� temizle
        }
    }

    // UI'�n ba�lang�� ayarlar�n� yapan metot
    private void InitialUISetup()
    {
        titleText.text = "Add Resources";

        // Coin ile sat�n alma butonunun g�r�n�rl���n� kontrol et
        if (!ResourceManager.Instance.HasEnoughResource(ResourceType.Coin, energyCost))
        {
            spendCoinBtn.gameObject.SetActive(false);
        }
        else
        {
            spendCoinBtn.gameObject.SetActive(true);
        }
    }

    // UI'� her saniye g�ncelleyecek olan Coroutine
    private IEnumerator UpdateUICoroutine()
    {
        // Bu d�ng�, Coroutine durdurulana kadar devam eder.
        while (true)
        {
            string time = ResourceManager.Instance.GetNextEnergyCountdownText();
            infoText.text = $"Next dragon power in\n<color=yellow>{time}</color>"; // Rengi daha belirgin yapabiliriz.

            // 1 saniye bekle ve d�ng�ye devam et.
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnSpendCoinButtonClicked()
    {
        if (ResourceManager.Instance.SpendResource(ResourceType.Coin, energyCost))
        {
            ResourceManager.Instance.AddResource(ResourceType.Energy, 1);
            // Sat�n al�mdan sonra buton durumu tekrar kontrol edilebilir, 
            // ama OnEnable/OnDisable d�ng�s� bunu zaten yapacakt�r.
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
            Debug.Log("�d�ll� Reklam �a�r�ld�!");
        }
        else
        {
            Debug.LogWarning("�d�ll� Reklam haz�r de�il veya AdsManager bulunamad�.");
        }
    }

    private void OnCloseButtonClicked()
    {
        PanelManager.Instance.HideLastPanel();
    }
}