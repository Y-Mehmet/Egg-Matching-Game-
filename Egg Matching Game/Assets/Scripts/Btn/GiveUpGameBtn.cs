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
        ResourceManager.Instance.SpendResource(ResourceType.Energy, 1);
        SceeneManager.instance.LoadScene(0);
    }
}
