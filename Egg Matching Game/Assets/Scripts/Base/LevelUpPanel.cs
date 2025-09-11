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
            Debug.LogError("LevelUpPanel: UI elemanlar� eksik! L�tfen t�m referanslar� atay�n.");
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
        //Debug.Log("�d�ll� Reklam Butonu T�kland�!");

        // �d�ll� reklam� g�stermeden �nce haz�r olup olmad���n� kontrol edin
        if (AdsManager.Instance != null)
        {
            ResourceManager.Instance.currentRewardedTypeChanged?.Invoke(RewardedType.Resource);
            AdsManager.Instance.ShowRewardedAd();
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
        int playcount = ResourceManager.Instance.GetResourceAmount(ResourceType.PlayCount);
        if (playcount > 995 && playcount% 1 == 0 )
        {
            // Ge�i� reklam�n� g�stermeden �nce haz�r olup olmad���n� kontrol edin (iste�e ba�l� ama iyi bir pratik)
            // E�er reklam haz�rsa g�ster
            if (AdsManager.Instance != null )
            {
                ResourceManager.Instance.currentRewardedTypeChanged?.Invoke(RewardedType.OneResource);
                AdsManager.Instance.ShowInterstitialAd();
                Debug.Log("Ge�i� Reklam� �a�r�ld�!");

            }
            else
            {
                Debug.LogWarning("Ge�i� Reklam� haz�r de�il veya AdsManager bulunamad�.");
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
