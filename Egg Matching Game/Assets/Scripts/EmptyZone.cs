using UnityEngine;

public class EmptyZone :MonoBehaviour, ITrigger
{
    void ITrigger.Triggered(GameObject triggerObject)
    {
        GameManager.instance.RemoveEggListByIndex(-1, triggerObject);
    }

    void ITrigger.TriggerExit(GameObject triggerObject)
    {
       
    }
}
