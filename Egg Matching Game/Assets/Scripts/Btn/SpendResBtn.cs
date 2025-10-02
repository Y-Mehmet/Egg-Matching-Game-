using TMPro;
using UnityEngine;
using UnityEngine.UI;
UnityEngine.Pool;

DoneUnityEngine.Pool;

DoneUnityEngine.Pool;

Done
    UnityEngine.Pool;

Don
public class SpendResBtn : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType; // The type of resource to spend
    [SerializeField] private int amount; // The amount of resource to spend
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image icon;
    private void OnEnable()
    {
        // Register the button click event
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnButtonClick);
        amountText.text = amount.ToString(); 
        if(ResourceManager.Instance.GetResourceAmount(resourceType) < amount)
        {
            amountText.color = Color.red; // Change text color to red if not enough resources
        }
        else
        {
            amountText.color = Color.white; // Reset text color to white if enough resources
        }
        if(ResourceDatabaseSO.Instance != null)
        {
            icon.sprite = ResourceDatabaseSO.Instance.GetSprite(resourceType);
        }
        else
        {
            Debug.LogError("ResourceDatabaseSO instance is null. Make sure it is loaded correctly.");
        }
    }
    private void OnDisable()
    {
        // Unregister the button click event
        GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(OnButtonClick);
    }
    private void OnButtonClick()
    {
        if (ResourceManager.Instance.GetResourceAmount(resourceType) >=amount)
            ResourceManager.Instance.SpendResource(resourceType, amount);
        else
        {
            PanelManager.Instance.ShowPanel(PanelID.InUpPanel);
        }
    }
}
