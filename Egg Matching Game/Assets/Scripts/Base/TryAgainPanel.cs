using UnityEngine;
using UnityEngine.UI;

public class TryAgainPanel : MonoBehaviour
{
   [SerializeField] private Button tryAgainButton, add20secButton, backToHomeButton;
    private void OnEnable()
    {
        tryAgainButton.onClick.AddListener(OnTryAgainButtonClicked);
        add20secButton.onClick.AddListener(OnAdd20SecButtonClicked);
        backToHomeButton.onClick.AddListener(() => SceeneManager.instance.LoadScene(0));
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

    }
    private void OnAdd20SecButtonClicked()
    {
        PanelManager.Instance.HideAllPanel();
        GameManager.instance.addSec?.Invoke(20);
        
    }
}
