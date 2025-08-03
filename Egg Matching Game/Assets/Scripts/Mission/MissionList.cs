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
        if(tempGem==0)
        {
            PanelManager.Instance.ShowPanel(PanelID.InUpPanel);
        }
        else if (DragonManager.Instance.GetRequiredGemAmount()<= tempGem)
        {
            DragonManager.Instance.OnDragonGemAmountChange?.Invoke(DragonManager.Instance.GetRequiredGemAmount());
            SoundManager.instance.PlaySfx(SoundType.Gem);
            
            UpdateMissionDetails();
        }
        else
        {
           
            DragonManager.Instance.OnDragonGemAmountChange?.Invoke(tempGem);
            SoundManager.instance.PlaySfx(SoundType.Gem);
            UpdateMissionDetails();
        }
       
    }
    private void UpdateMissionDetails()
    {
        requireGemAmount = DragonManager.Instance.GetRequiredGemAmount();
        dragonNameText.text = DragonManager.Instance.GetCurrentDragonSO().DragonName.ToString();
        desctriptionText.text = "Paint the "+DragonManager.Instance.GetCurrentDragonSO().color.ToString().FirstCharacterToLower() +" parts";
        missionGemCountText.text = requireGemAmount.ToString();
        if(ResourceManager.Instance.GetResourceAmount(ResourceType.Gem)<=0)
            missionGemCountText.color = Color.red;
        else
            missionGemCountText.color= Color.white; 
        colorImage.color = ColorManager.instance.GetEggColor(DragonManager.Instance.GetCurrentDragonSO().color);

    }
}
