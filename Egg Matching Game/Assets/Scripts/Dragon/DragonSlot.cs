using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragonSlot : MonoBehaviour
{
    [SerializeField] TMP_Text dragonNameText, dragonIndexText;
    [SerializeField] Button wiewBtn;
    [SerializeField] Image dragonImage;
    int slotIndex;
    private void OnEnable()
    {
        slotIndex = transform.parent.GetSiblingIndex();
        dragonIndexText.text = "Dragon " + (slotIndex + 1).ToString();
        dragonNameText.text = DragonManager.Instance.dragonHolder.dragonSOList[slotIndex].DragonName.ToString();
        dragonImage.sprite = DragonManager.Instance.dragonHolder.dragonSOList[slotIndex].DragonSprite;
        wiewBtn.onClick.AddListener(OnViewBtnClick);
    }
    private void OnDisable()
    {
        wiewBtn.onClick.RemoveListener(OnViewBtnClick);
    }
    private void OnViewBtnClick()
    {
        ResourceManager.Instance.SelectedDragonIndex = slotIndex;
        SoundManager.instance.PlaySfx(SoundType.btnClick);
    }

}
