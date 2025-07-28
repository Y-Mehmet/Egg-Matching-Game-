using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TryAgainPanel : MonoBehaviour
{
   [SerializeField] private Button tryAgainButton, add20secButton, backToHomeButton;
    [SerializeField] private int coinCost = 2000; // Örnek olarak 50 deðerini kullanabilirsiniz
    [SerializeField] private TMP_Text tryAgainBtnValueText;
    private void OnEnable()
    {
        tryAgainButton.onClick.AddListener(OnTryAgainButtonClicked);
        add20secButton.onClick.AddListener(OnAdd20SecButtonClicked);
        backToHomeButton.onClick.AddListener(() => SceeneManager.instance.LoadScene(0));
        tryAgainBtnValueText.text= coinCost.ToString();
    }
    private void OnDisable()
    {
        tryAgainButton.onClick.RemoveListener(OnTryAgainButtonClicked);
        add20secButton.onClick.RemoveListener(OnAdd20SecButtonClicked);
        backToHomeButton.onClick.RemoveListener(() => SceeneManager.instance.LoadScene(0));
    }
    private void OnTryAgainButtonClicked()
    {
        PanelManager.Instance.HideAllPanel();
        GameManager.instance.gameReStart?.Invoke();
        GameManager.instance.ReStart();
        

    }
    private void OnAdd20SecButtonClicked()
    {
        if(ResourceManager.Instance.SpendResource(ResourceType.Coin,coinCost))
        {
            PanelManager.Instance.HideAllPanel();
            GameManager.instance.addSec?.Invoke(20);
        }
        
    }
}
