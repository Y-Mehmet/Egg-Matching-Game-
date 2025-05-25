using UnityEngine;
using UnityEngine.UI;

public class CloseBtn : MonoBehaviour
{
    public GameObject parentPanel;
    void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(OnClickBtn) ;
        GameManager.instance.AnyPanelisOpen = true;
    }
    private void OnDisable()
    {
        // Remove the listener when the object is disabled
        GetComponent<Button>().onClick.RemoveListener(OnClickBtn);
        GameManager.instance.AnyPanelisOpen = false;
    }

    void OnClickBtn()
    {


       parentPanel.SetActive(false);


    }
}
