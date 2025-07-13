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
            Debug.LogError("LevelUpPanel: UI elemanlar� eksik! L�tfen t�m referanslar� atay�n.");
            return;
        }
        levelText.text = "" + GameManager.instance.gameData.levelIndex + 1;
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
    }
    private void OnAdsButtonClicked()
    {
        Debug.Log("�d�ll� Reklam Butonu T�kland�!");

        // �d�ll� reklam� g�stermeden �nce haz�r olup olmad���n� kontrol edin
        if (AdsManager.Instance != null && AdsManager.Instance.rewardedAds != null)
        {
            GameManager.instance.currentRewardedTypeChanged?.Invoke(RewardedType.Resource);
            AdsManager.Instance.rewardedAds.ShowRewardedAd();
            Debug.Log("�d�ll� Reklam �a�r�ld�!");
        }
        else
        {
            Debug.LogWarning("�d�ll� Reklam haz�r de�il veya AdsManager bulunamad�.");
            // Alternatif olarak, oyuncuya reklam�n �u anda mevcut olmad���n� bildirebilirsiniz.
        }
    }
    private void OnContinueButtonClicked()
    {
        ResourceManager.Instance.AddResource(ResourceType.Coin, ResourceManager.Instance.coinsPerGame   );
        ResourceManager.Instance.AddResource(ResourceType.Gem, ResourceManager.Instance.gemsPerGame);
        PanelManager.Instance.HideLastPanel();
        SceeneManager.instance.LoadScene(0);
    }
}
