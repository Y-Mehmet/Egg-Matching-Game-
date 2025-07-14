using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LevelUpPanel : MonoBehaviour
{
    public TMP_Text levelText, gemCountText, coinCountText;
    public Button adsBtn, contunioBtn;


    private void OnEnable()
    {
              
        if( levelText == null)
        {
            Debug.LogError("LevelUpPanel: UI elemanlarý eksik! Lütfen tüm referanslarý atayýn.");
            return;
        }
        levelText.text = "" + (ResourceManager.Instance.GetResourceAmount(ResourceType.LevelIndex)) ;
        gemCountText.text = "" + ResourceManager.Instance.gemsPerGame;
        coinCountText.text = "" + ResourceManager.Instance.coinsPerGame;
        adsBtn.onClick.AddListener(OnAdsButtonClicked);
        contunioBtn.onClick.AddListener(OnContinueButtonClicked);
    }
    private IEnumerator enumerator()
    {
        yield return null;
    }
    private void Start()
    {
       
    }
    private void OnDisable()
    {
        adsBtn.onClick.RemoveListener(OnAdsButtonClicked);
        contunioBtn.onClick.RemoveListener(OnContinueButtonClicked);
        PanelManager.Instance.HideAllPanel();
        
    }
    private void OnAdsButtonClicked()
    {
        Debug.Log("Ödüllü Reklam Butonu Týklandý!");

        // Ödüllü reklamý göstermeden önce hazýr olup olmadýðýný kontrol edin
        if (AdsManager.Instance != null && AdsManager.Instance.rewardedAds != null)
        {
            ResourceManager.Instance.currentRewardedTypeChanged?.Invoke(RewardedType.Resource);
            AdsManager.Instance.rewardedAds.ShowRewardedAd();
            Debug.Log("Ödüllü Reklam Çaðrýldý!");
        }
        else
        {
            Debug.LogWarning("Ödüllü Reklam hazýr deðil veya AdsManager bulunamadý.");
            // Alternatif olarak, oyuncuya reklamýn þu anda mevcut olmadýðýný bildirebilirsiniz.
        }
    }
    private void OnContinueButtonClicked()
    {
        ResourceManager.Instance.AddResource(ResourceType.Coin, ResourceManager.Instance.coinsPerGame   );
        ResourceManager.Instance.AddResource(ResourceType.Gem, ResourceManager.Instance.gemsPerGame);
        
        SceeneManager.instance.LoadScene(0);
    }
}
