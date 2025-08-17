using UnityEngine;

public class ADBtn : MonoBehaviour
{
    private void OnEnable()
    {
        // Register the button click event
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnButtonClick);
    }
    private void OnDisable()
    {
        // Unregister the button click event
        GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(OnButtonClick);
    }
    private void OnButtonClick()
    {
        //Debug.Log("Ödüllü Reklam Butonu Týklandý!");

        //// Ödüllü reklamý göstermeden önce hazýr olup olmadýðýný kontrol edin
        //if (AdsManager.Instance != null && AdsManager.Instance.rewardedAds != null)
        //{
        //    ResourceManager.Instance.currentRewardedTypeChanged?.Invoke(RewardedType.Resource);
        //    AdsManager.Instance.rewardedAds.ShowRewardedAd();
        //    Debug.Log("Ödüllü Reklam Çaðrýldý!");
        //}
        //else
        //{
        //    Debug.LogWarning("Ödüllü Reklam hazýr deðil veya AdsManager bulunamadý.");
        //    // Alternatif olarak, oyuncuya reklamýn þu anda mevcut olmadýðýný bildirebilirsiniz.
        //}
    }
}
