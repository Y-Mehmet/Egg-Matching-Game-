using UnityEngine;

public class CloseAllBtn : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClickBtn);
        
    }
    private void OnDisable()
    {
        // Remove the listener when the object is disabled
        GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(OnClickBtn);
    }
    void OnClickBtn()
    {
        PanelManager.Instance.HideAllPanel();
    }
}
