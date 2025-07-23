using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MissionList : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TMP_Text dragonNameText, desctriptionText, missionGemCountText;
    [SerializeField] Image colorImage;
    int requireGemAmount=0;
    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        UpdateMissionDetails();
    }
    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
    private void OnButtonClick()
    {
        int tempGem = ResourceManager.Instance.GetResourceAmount(ResourceType.Gem);
        if (DragonManager.Instance.GetRequiredGemAmount()<= tempGem)
        {
            DragonManager.Instance.OnDragonGemAmountChange?.Invoke(DragonManager.Instance.GetRequiredGemAmount());
            
            UpdateMissionDetails();
        }
        else
        {
           
            DragonManager.Instance.OnDragonGemAmountChange?.Invoke(tempGem);
           
            UpdateMissionDetails();
        }
       
    }
    private void UpdateMissionDetails()
    {
        requireGemAmount = DragonManager.Instance.GetRequiredGemAmount();
        dragonNameText.text = DragonManager.Instance.GetCurrentDragonSO().DragonName.ToString();
        desctriptionText.text = "Paint the "+DragonManager.Instance.GetCurrentDragonSO().color.ToString().FirstCharacterToLower() +" parts";
        missionGemCountText.text = requireGemAmount.ToString();
        colorImage.color = ColorManager.instance.GetEggColor(DragonManager.Instance.GetCurrentDragonSO().color);

    }
}
