using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class CurrentDragonProssesPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text dragonClorText, sliderValueText;
    [SerializeField] private Slider slider;
    private void OnEnable()
    {
        UpdateUI(0);
        slider.maxValue = DragonManager.Instance.GetMissionGemAmount();
        slider.interactable = false;
        DragonManager.Instance.OnDragonIndexChange += UpdateUI;
        DragonManager.Instance.OnDragonGemAmountChange += UpdateUI;
    }
    private void OnDisable()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        DragonManager.Instance.OnDragonGemAmountChange-= UpdateUI;
        DragonManager.Instance.OnDragonIndexChange -= UpdateUI;
    }
    private void OnSliderValueChanged(float value)
    {
        slider.value = value;
    }
    private void UpdateUI(int d)
    {
        dragonClorText.text = DragonManager.Instance.dragonHolder.dragonSOList[DragonManager.Instance.GetDragonIndex()].color.ToString();
        sliderValueText.text = DragonManager.Instance.dragonHolder.dragonSOList[DragonManager.Instance.GetDragonIndex()].DragonGemAmount + "/" + DragonManager.Instance.dragonHolder.dragonSOList[DragonManager.Instance.GetDragonIndex()].dragonMissionGemValue;
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        OnSliderValueChanged(DragonManager.Instance.GetCurrentDragonSO().DragonGemAmount);
    }

}
