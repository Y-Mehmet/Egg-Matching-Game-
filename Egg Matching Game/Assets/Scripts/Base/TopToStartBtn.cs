using UnityEngine;
using UnityEngine.UI;

public class TopToStartBtn : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(OnClickBtn);
    }
    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveListener(OnClickBtn);
    }
    private void OnClickBtn()
    {
        CountdownController.Instance.StartCountdown();
    }
}
