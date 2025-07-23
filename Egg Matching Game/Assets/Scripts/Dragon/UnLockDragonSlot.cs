using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnLockDragonSlot : MonoBehaviour
{
    [SerializeField] private Image dragonImage;
    [SerializeField] private TMP_Text dragonNameText, dragonIndexText;
    private void OnEnable()
    {
        int slotIndex = transform.parent.GetSiblingIndex();
        dragonImage.sprite= DragonManager.Instance.dragonHolder.dragonSOList[slotIndex].DragonSprite;
        dragonNameText.text= DragonManager.Instance.dragonHolder.dragonSOList[slotIndex].DragonName.ToString();
        dragonIndexText.text = "Dragon " + (slotIndex + 1).ToString();

    }
}
