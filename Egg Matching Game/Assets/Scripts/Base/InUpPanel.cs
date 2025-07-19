using TMPro;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class InUpPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text gemText, coinText;

    private void OnEnable()
    {
        gemText.text = ResourceManager.Instance.GetResourceAmount(ResourceType.Gem).ToString();
        coinText.text= ResourceManager.Instance.GetResourceAmount(ResourceType.Coin).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
