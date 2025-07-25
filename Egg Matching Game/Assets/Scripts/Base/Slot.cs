using Unity.Burst.CompilerServices;
using UnityEngine;

public class Slot : MonoBehaviour,ITrigger
{
    public int slotIndex;
   

    public void Triggered(GameObject triggerObject)
    {
        //GameManager.instance.AddEggListByIndex(slotIndex, triggerObject);
        

    }

    private void Start()
    {
       // slotIndex = transform.GetSiblingIndex();
        
    }

   public  void TriggerExit(GameObject triggerObject)
    {
        //GameManager.instance.RemoveEggListByIndex(slotIndex, triggerObject);
    }
}
