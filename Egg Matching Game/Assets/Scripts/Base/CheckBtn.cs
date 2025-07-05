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
        
    }
}
