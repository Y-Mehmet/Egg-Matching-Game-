using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnProgresDragonSlot : MonoBehaviour
{
    [SerializeField] TMP_Text dragonNameText, dragonIndexText, sliderValueText;
    [SerializeField] Slider slider;
    [SerializeField] Image dragonImage;
    [SerializeField] Button wiewBtn;
    int slotIndex;
    private void OnEnable()
    {
        slider.maxValue = DragonManager.Instance.GetMissionGemAmount();
        slider.interactable = false;
         slotIndex = transform.parent.GetSiblingIndex();
        dragonIndexText.text = (slotIndex + 1).ToString();
        dragonNameText.text = DragonManager.Instance.dragonHolder.dragonSOList[slotIndex].DragonName.ToString();
        dragonImage.sprite = DragonManager.Instance.dragonHolder.dragonSOList[slotIndex].DragonSprite;
        OnSliderValueChanged(DragonManager.Instance.GetCurrentDragonSO().DragonGemAmount);
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        wiewBtn.onClick.AddListener(OnViewBtnClick);
    }
        private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        wiewBtn.onClick.RemoveListener(OnViewBtnClick);
    }
   private void OnSliderValueChanged(float value)
    {
        sliderValueText.text = DragonManager.Instance.dragonHolder.dragonSOList[slotIndex].DragonGemAmount +"/" + DragonManager.Instance.dragonHolder.dragonSOList[slotIndex].dragonMissionGemValue;
        slider.value = DragonManager.Instance.dragonHolder.dragonSOList[slotIndex].DragonGemAmount;
    }
    private void OnViewBtnClick()
    {
        PanelManager.Instance.ShowPanel(PanelID.DragonPaint,  PanelShowBehavior.HIDE_PREVISE);
    }
}
