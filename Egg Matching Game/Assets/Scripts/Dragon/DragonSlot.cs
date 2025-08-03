using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragonSlot : MonoBehaviour
{
    [SerializeField] TMP_Text dragonNameText, dragonIndexText, addTime;
    [SerializeField] Button wiewBtn;
    [SerializeField] Image dragonImage, selectedIcon;
    int slotIndex;
    private void OnEnable()
    {
        
        slotIndex = transform.parent.GetSiblingIndex();
        if ( ResourceManager.Instance.SelectedDragonIndex!= slotIndex)
            selectedIcon.gameObject.SetActive(false);
        else
            selectedIcon.gameObject.SetActive(true);

        addTime.text ="+ "+ DragonManager.Instance.dragonHolder.dragonSOList[slotIndex].addTime.ToString() + "s";
        dragonIndexText.text = "Dragon " + (slotIndex + 1).ToString();
        dragonNameText.text = DragonManager.Instance.dragonHolder.dragonSOList[slotIndex].DragonName.ToString();
        dragonImage.sprite = DragonManager.Instance.dragonHolder.dragonSOList[slotIndex].DragonSprite;
        wiewBtn.onClick.AddListener(OnViewBtnClick);
        ResourceManager.Instance.onSelectedDragonIndexChanged += UpdateSelectedIcon;
    }
    private void OnDisable()
    {
        wiewBtn.onClick.RemoveListener(OnViewBtnClick);
        ResourceManager.Instance.onSelectedDragonIndexChanged -= UpdateSelectedIcon;
    }
    private void OnViewBtnClick()
    {
        
            ResourceManager.Instance.SelectedDragonIndex = slotIndex;
        ResourceManager.Instance.onSelectedDragonIndexChanged?.Invoke();

        SoundManager.instance.PlaySfx(SoundType.btnClick);
    }
    private void UpdateSelectedIcon()
    {
     if (ResourceManager.Instance.SelectedDragonIndex == slotIndex)
            selectedIcon.gameObject.SetActive(true);
        else
            selectedIcon.gameObject.SetActive(false);
    }

}
