using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AbilityButton : MonoBehaviour
{
    [Header("Yetenek Verisi")]
    public AbilityData abilityData;

    private Button _button;
    private Image _iconImage;

    void Start()
    {
        _button = GetComponent<Button>();
        _iconImage = GetComponent<Image>();

        if (abilityData != null)
        {
            // Butonun g�rselini ve g�revini ayarla
            _iconImage.sprite = abilityData.Icon;
            _button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Bu butona bir AbilityData atanmam��!", this);
        }
    }

    private void OnButtonClick()
    {
        AbilityManager.Instance.SelectAbility(abilityData);
    }
}