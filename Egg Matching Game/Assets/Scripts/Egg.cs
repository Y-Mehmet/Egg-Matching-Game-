using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class Egg : MonoBehaviour
{
    public EggColor eggColor;
    
    public Vector3 startPos = new();

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
            Debug.Log("slot index " + slotIndex+" list count "+ GameManager.instance.SlotPositionList.Count);
          
            if (slotIndex == -1)
            {
                transform.DOMove(startPos, 0.5f).SetEase(Ease.OutBack);
            }
            else
            {
                transform.DOMove(GameManager.instance.SlotPositionList[slotIndex], 0.5f).SetEase(Ease.OutBack);
            }
            
        }else
        {
            
        }
    }
   

    private void OnTriggerEnter(Collider other)
    {
        ITrigger iTrigger = other.GetComponent<ITrigger>();
        if (iTrigger != null)
        {
            iTrigger.Triggered(gameObject);
        }else
        {
           
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //ITrigger iTrigger = other.GetComponent<ITrigger>();
        //if (iTrigger != null)
        //{
        //    iTrigger.TriggerExit(gameObject);
        //}
    }

    
}

