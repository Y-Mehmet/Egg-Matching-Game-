using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyAbilityPanel : MonoBehaviour
{
    [SerializeField] private Button buyBtn;
    [SerializeField] private TMP_Text costText, desciriptionText;
    [SerializeField] private Image boosterImage;
    
    private void OnEnable()
    {
        var abilityData = AbilityManager.Instance.abilityDataHolder.abilities.Find(a => a.Type == AbilityManager.Instance.currentAbilityType);
        buyBtn.onClick.AddListener(OnClick);
        costText.text = abilityData.cost + "";
        desciriptionText.text = abilityData.Description;
        boosterImage.sprite = abilityData.Icon;
        
    }
    private void OnDisable()
    {
        buyBtn.onClick.RemoveListener(OnClick);
    }
    private void OnClick()
    {
        SoundManager.instance.PlayBtnClick();
        PanelManager.Instance.ShowPanel(PanelID.InUpPanel);
    }
}
