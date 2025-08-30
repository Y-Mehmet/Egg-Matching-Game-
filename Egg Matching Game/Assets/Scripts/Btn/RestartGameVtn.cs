using UnityEngine;

public class RestartGameVtn : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClick);
        if (ResourceManager.Instance.HasEnoughResource(ResourceType.Energy, 1))
        {
            GetComponent<UnityEngine.UI.Button>().image.color = Color.white;
        }else
        {
            GetComponent<UnityEngine.UI.Button>().image.color = Color.red;
        }
    }
    private void OnDisable()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(OnClick);
    }
    private void OnClick()
    {
       if(ResourceManager.Instance.HasEnoughResource(ResourceType.Energy, 2))
            {
            GameManager.instance.gameReStart?.Invoke();
            ResourceManager.Instance.SpendResource(ResourceType.Energy, 2);
        }else
        {
            SoundManager.instance.PlaySfx(SoundType.EmptyCoin);
           
        }

    }
}
