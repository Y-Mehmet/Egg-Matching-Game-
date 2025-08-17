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
        //Debug.Log("�d�ll� Reklam Butonu T�kland�!");

        //// �d�ll� reklam� g�stermeden �nce haz�r olup olmad���n� kontrol edin
        //if (AdsManager.Instance != null && AdsManager.Instance.rewardedAds != null)
        //{
        //    ResourceManager.Instance.currentRewardedTypeChanged?.Invoke(RewardedType.Resource);
        //    AdsManager.Instance.rewardedAds.ShowRewardedAd();
        //    Debug.Log("�d�ll� Reklam �a�r�ld�!");
        //}
        //else
        //{
        //    Debug.LogWarning("�d�ll� Reklam haz�r de�il veya AdsManager bulunamad�.");
        //    // Alternatif olarak, oyuncuya reklam�n �u anda mevcut olmad���n� bildirebilirsiniz.
        //}
    }
}
