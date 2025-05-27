using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class Egg : MonoBehaviour
{
    public EggColor eggColor;
    
    public Vector3 startPos = new();
    public int startTopStackIndex = 0;
    public List<IEggProperty> properties = new();

    private void OnEnable()
    {
        GameManager.instance.onSlotIndexChange += OnSlotIndexChange;

    }
    private void OnDisable()
    {
        GameManager.instance.onSlotIndexChange -= OnSlotIndexChange;
    }
    private void OnSlotIndexChange(int slotIndex, GameObject egg)
    {
        if (egg== gameObject)
        {
           // Debug.Log("slot index " + slotIndex+" list count "+ GameManager.instance.SlotPositionList.Count);
          
            if (slotIndex == -1)
            {
                transform.DOMove(startPos, 0.5f).SetEase(Ease.OutBack);
            }
            else
            {
                transform.DOMove(GameManager.instance.SlotPositionList[slotIndex], 0.5f).SetEase(Ease.OutBack);
            }
            
        }
    }
   

    private void OnTriggerEnter(Collider other)
    {
        ITrigger iTrigger = other.GetComponent<ITrigger>();
        if (iTrigger != null)
        {
            iTrigger.Triggered(gameObject);
        }
    }
  
   
    public bool IsCorrect(EggColor expectedColor)
    {
        foreach (var prop in properties)
        {
            var result = prop.Evaluate(expectedColor, eggColor);
            if (result.HasValue) return result.Value;
        }

        // default davranýþ: sadece renk eþleþirse doðru
        return eggColor == expectedColor;
    }


}

