using UnityEngine;

public interface ITrigger 
{
    public void Triggered(GameObject triggerObject);
    public void TriggerExit(GameObject triggerObject);
}
