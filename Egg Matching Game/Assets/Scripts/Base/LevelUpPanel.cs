using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LevelUpPanel : MonoBehaviour
{
    public TMP_Text levelText, gemCountText, coinCountText;
    public Button adsBtn, contunioBtn;
    private int gemAmount;
    private bool isFirst = true;

    private void OnEnable()
    {
              
        if( levelText == null)
        {
            Debug.LogError("LevelUpPanel: UI elemanlarý eksik! Lütfen tüm referanslarý atayýn.");
            return;
        }
        if(isFirst)
        {
            isFirst = false;
            return;
        }
        SoundManager.instance.StopClip(SoundType.Tiktak);

        SoundManager.instance.PlaySfx(SoundType.LevelUp);

        levelText.text = "" + (ResourceManager.Instance.GetResourceAmount(ResourceType.LevelIndex)) ;
        if (GameManager.instance.isSelectTrueDaragonEgg)
            gemAmount = ResourceManager.Instance.gemsPerGame;
        else
            gemAmount = 0;
        gemCountText.text = "" + gemAmount;
        coinCountText.text = "" + ResourceManager.Instance.coinsPerGame;
        adsBtn.onClick.AddListener(OnAdsButtonClicked);
        contunioBtn.onClick.AddListener(OnContinueButtonClicked);
    }
    private IEnumerator enumerator()
    {
        yield return null;
    }
   
    private void OnDisable()
    {
        adsBtn.onClick.RemoveListener(OnAdsButtonClicked);
        contunioBtn.onClick.RemoveListener(OnContinueButtonClicked);
        PanelManager.Instance.HidePanelWithPanelID(panelID: PanelID.LevelUpPanel);

    }
    private void OnAdsButtonClicked()
    {
        //Debug.Log("Ödüllü Reklam Butonu Týklandý!");

        // Ödüllü reklamý göstermeden önce hazýr olup olmadýðýný kontrol edin
        if (AdsManager.Instance != null)
        {
            ResourceManager.Instance.currentRewardedTypeChanged?.Invoke(RewardedType.Resource);
            AdsManager.Instance.ShowRewardedAd();
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
        int playcount = ResourceManager.Instance.GetResourceAmount(ResourceType.PlayCount);
        if (playcount > 995 && playcount% 1 == 0 )
        {
            // Geçiþ reklamýný göstermeden önce hazýr olup olmadýðýný kontrol edin (isteðe baðlý ama iyi bir pratik)
            // Eðer reklam hazýrsa göster
            if (AdsManager.Instance != null )
            {
                ResourceManager.Instance.currentRewardedTypeChanged?.Invoke(RewardedType.OneResource);
                AdsManager.Instance.ShowInterstitialAd();
                Debug.Log("Geçiþ Reklamý Çaðrýldý!");

            }
            else
            {
                Debug.LogWarning("Geçiþ Reklamý hazýr deðil veya AdsManager bulunamadý.");
            }
        }
        else
        {
            ResourceManager.Instance.AddResource(ResourceType.Coin, ResourceManager.Instance.coinsPerGame);
            ResourceManager.Instance.AddResource(ResourceType.Gem, gemAmount);
            SceeneManager.instance.LoadScene(0);
        }



    }
}
