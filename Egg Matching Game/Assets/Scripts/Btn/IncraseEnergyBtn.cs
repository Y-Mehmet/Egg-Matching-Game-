using UnityEngine;

public class IncraseEnergyBtn : MonoBehaviour
{
    private void OnEnable()
    {
        if (ResourceManager.Instance.GetResourceAmount(ResourceType.Energy) >= ResourceManager.Instance.maxEnergy)
        {
            transform.GetChild(0).gameObject.SetActive(false); 
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClick);
        }
            
        
    }
    private void OnDisable()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(OnClick);
    }
    private void OnClick()
    {
        if (ResourceManager.Instance.GetResourceAmount(ResourceType.Energy) < ResourceManager.Instance.maxEnergy)
        {
            SoundManager.instance.PlaySfx(SoundType.btnClick);
            PanelManager.Instance.ShowPanel(PanelID.RefillEnergyPanel);
        }
        else
        {
            Debug.Log("Enerji zaten maksimum seviyede.");
        }
    }
}
