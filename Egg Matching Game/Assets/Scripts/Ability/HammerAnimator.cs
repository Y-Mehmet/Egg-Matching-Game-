using System.Collections;
using UnityEngine;

public class HammerAnimator : MonoBehaviour
{
    [HideInInspector]
    public GameObject targetToDestroy;

    // BU METOD, �EK�� AN�MASYONUNUN 'VURMA' ANININ EN SONUNDA
    // B�R 'ANIMATION EVENT' �LE �A�RILACAKTIR.
    public void OnSmashAnimationComplete()
    {
        StartCoroutine(OnSmashCorrotune());
      
    }
    public void OnThrowBombAnimationComplated()
    {
        StartCoroutine(OnThrowCorrotune());
    }
    IEnumerator OnSmashCorrotune()
    {

        yield return new WaitForSeconds(2f); // Anl�k bir gecikme ekleyerek animasyonun tamamlanmas�n� bekler
        if (targetToDestroy != null)
        {
            int slotIndex= targetToDestroy.transform.GetSiblingIndex();
            GameManager.instance.BreakSlotProses(slotIndex);
            
            targetToDestroy.SetActive(false);
            targetToDestroy = null;

        }

        // Animasyon bitti�i i�in �eki� objesini de yok et
        gameObject.SetActive(false);
    }
    IEnumerator OnThrowCorrotune()
    {

        yield return new WaitForSeconds(2f); // Anl�k bir gecikme ekleyerek animasyonun tamamlanmas�n� bekler
        if (targetToDestroy != null)
        {
          
            GameManager.instance.BreakEggProses(targetToDestroy);
            targetToDestroy.SetActive(false);
            targetToDestroy = null;
            

        }

        // Animasyon bitti�i i�in �eki� objesini de yok et
        gameObject.SetActive(false);
    }
}