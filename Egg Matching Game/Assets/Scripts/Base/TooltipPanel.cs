using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TooltipPanel : MonoBehaviour
{
    [SerializeField] TMP_Text titleText, descritptionText, tooltipTitle;
    [SerializeField] Image tooltipIamge;
    [SerializeField] Button okButton;
    Tooltip currentTooltip;
    private void OnEnable()
    {int tooltipIndex = TooltipManager.instance.GetTooltipIndex();

        if (tooltipIndex==-1)
        {
            gameObject.SetActive(false);
        }else
        {
            currentTooltip = TooltipManager.instance.GetTooltip();
            if(currentTooltip.isNewFeature)
            {
                titleText.text= "New Feature";
                tooltipTitle.text = currentTooltip.title;
                descritptionText.text = currentTooltip.description;
                tooltipIamge.sprite = currentTooltip.icon;
            }else
            {
                titleText.text = "";
                tooltipTitle.text = currentTooltip.title;
                descritptionText.text = currentTooltip.description;
                tooltipIamge.sprite = currentTooltip.icon;
            }
        }
        okButton.onClick.AddListener(OnButtonClick);
    }
    private void OnDisable()
    {
        okButton.onClick.RemoveListener(OnButtonClick);
    }
    private void OnButtonClick()
    {
        gameObject.SetActive(false);
    }
}
