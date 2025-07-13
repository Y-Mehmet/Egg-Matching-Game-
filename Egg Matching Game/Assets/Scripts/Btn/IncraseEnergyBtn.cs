using UnityEngine;

public class IncraseEnergyBtn : MonoBehaviour
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
        if (ResourceManager.Instance.GetResourceAmount(ResourceType.Energy) < 5)
        {
            PanelManager.Instance.ShowPanel(PanelID.RefillEnergyPanel);
        }
        else
        {
            Debug.Log("Enerji zaten maksimum seviyede.");
        }
    }
}
