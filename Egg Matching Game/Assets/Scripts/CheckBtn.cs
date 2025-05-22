using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBtn : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveListener(OnClick);
    }
    private void OnClick()
    {
        GameManager.instance.Check();
        foreach( KeyValuePair<int, GameObject> kvp in GameManager.instance.eggSlotDic)
        {
            Debug.Log($"slot index : {kvp.Key} color {kvp.Value.GetComponent<Egg>().eggColor.ToString()}");
        }
    }
}
