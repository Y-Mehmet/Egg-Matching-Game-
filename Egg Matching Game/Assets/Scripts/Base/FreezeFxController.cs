using System.Collections;
using UnityEngine;

public class FreezeFxController : MonoBehaviour
{
    
    private void OnEnable()
    {
        StartCoroutine(enumerator());
    }
    IEnumerator enumerator()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        StopCoroutine(enumerator());
    }
}
