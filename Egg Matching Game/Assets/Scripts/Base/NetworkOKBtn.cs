using UnityEngine;
using UnityEngine.UI;

public class NetworkOKBtn : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(OnOkButtonClicked);
    }
    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveListener(OnOkButtonClicked);

    }

    public void OnOkButtonClicked()
    {
        NetworkManager.intance.OnOkButtonClicked();
    }
}
