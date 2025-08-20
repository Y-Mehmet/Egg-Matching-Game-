using UnityEngine;

public class ContinueGameBtn : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClick);
    }
    private void OnDisable()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(OnClick);
    }
    private void OnClick()
    {
        GameManager.instance.continueGame?.Invoke();
        PanelManager.Instance.HideLastPanel();
    }
}
