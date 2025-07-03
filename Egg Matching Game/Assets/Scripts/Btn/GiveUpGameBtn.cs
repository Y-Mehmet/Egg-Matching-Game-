using UnityEngine;

public class GiveUpGameBtn : MonoBehaviour
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
        SceeneManager.instance.LoadScene(0);
    }
}
