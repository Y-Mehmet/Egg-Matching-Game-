using UnityEngine;

public class RestartGameVtn : MonoBehaviour
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
        GameManager.instance.gameReStart?.Invoke();
        ResourceManager.Instance.SpendResource(ResourceType.Energy, 1);

    }
}
