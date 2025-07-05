using System.Collections;
using UnityEngine;

public class HammerAnimator : MonoBehaviour
{
    [HideInInspector]
    public GameObject targetToDestroy;

    // BU METOD, ÇEKÝÇ ANÝMASYONUNUN 'VURMA' ANININ EN SONUNDA
    // BÝR 'ANIMATION EVENT' ÝLE ÇAÐRILACAKTIR.
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

        yield return new WaitForSeconds(2f); // Anlýk bir gecikme ekleyerek animasyonun tamamlanmasýný bekler
        if (targetToDestroy != null)
        {
            int slotIndex= targetToDestroy.transform.GetSiblingIndex();
            GameManager.instance.BreakSlotProses(slotIndex);
            
            targetToDestroy.SetActive(false);
            targetToDestroy = null;

        }

        // Animasyon bittiði için çekiç objesini de yok et
        gameObject.SetActive(false);
    }
    IEnumerator OnThrowCorrotune()
    {

        yield return new WaitForSeconds(2f); // Anlýk bir gecikme ekleyerek animasyonun tamamlanmasýný bekler
        if (targetToDestroy != null)
        {
          
            GameManager.instance.BreakEggProses(targetToDestroy);
            targetToDestroy.SetActive(false);
            targetToDestroy = null;
            

        }

        // Animasyon bittiði için çekiç objesini de yok et
        gameObject.SetActive(false);
    }
}