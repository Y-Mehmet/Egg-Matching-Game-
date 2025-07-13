using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LevelUpPanel : MonoBehaviour
{
    public TMP_Text levelText;
    public Button adsBtn, contunioBtn;
    
    private void OnEnable()
    {
              
        if( levelText == null)
        {
            Debug.LogError("LevelUpPanel: UI elemanlarý eksik! Lütfen tüm referanslarý atayýn.");
            return;
        }

        adsBtn.onClick.AddListener(OnAdsButtonClicked);
        contunioBtn.onClick.AddListener(OnContinueButtonClicked);
    }
    private IEnumerator enumerator()
    {
        yield return null;
    }
    private void Start()
    {
        levelText.text = ""+ GameManager.instance.gameData.levelIndex + 1;
    }
    private void OnDisable()
    {
        adsBtn.onClick.RemoveListener(OnAdsButtonClicked);
        contunioBtn.onClick.RemoveListener(OnContinueButtonClicked);
    }
    private void OnAdsButtonClicked()
    {
        Debug.Log("Ödüllü Reklam Butonu Týklandý!");

        // Ödüllü reklamý göstermeden önce hazýr olup olmadýðýný kontrol edin
        if (AdsManager.Instance != null && AdsManager.Instance.rewardedAds != null)
        {
            GameManager.instance.currentRewardedTypeChanged?.Invoke(RewardedType.Resource);
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
        GameManager.instance.ReStart();
        PanelManager.Instance.HideLastPanel();
    }
}
