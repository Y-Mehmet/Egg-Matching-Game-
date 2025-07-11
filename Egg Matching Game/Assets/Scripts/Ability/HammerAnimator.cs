using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class HammerAnimator : MonoBehaviour
{
    [HideInInspector]
    public GameObject targetToDestroy;
    private float rotationDuration = 2; // Animasyonun s�resi
    private Vector3 startPos= new Vector3(0,-3, -0.5f); // Ba�lang�� pozisyonu
    private void OnEnable()
    {
        transform.localPosition = startPos; // Ba�lang�� pozisyonunu ayarla
       
        transform.localRotation= Quaternion.Euler(-70,0,0);
    }

    // BU METOD, �EK�� AN�MASYONUNUN 'VURMA' ANININ EN SONUNDA
    // B�R 'ANIMATION EVENT' �LE �A�RILACAKTIR.
    public void OnSmashAnimationComplete() // break slot
    {
        StartCoroutine(OnSmashCorrotune());
      
    }
    public void OnThrowBombAnimationComplated() // break egg
    {
        StartCoroutine(OnThrowCorrotune());
    }
    IEnumerator OnSmashCorrotune()
    {
        Debug.LogWarning("OnSmashCorrotune called. targetToDestroy: " + (targetToDestroy != null ? targetToDestroy.name : "null"));

        yield return new WaitForSeconds(2.2f); // Anl�k bir gecikme ekleyerek animasyonun tamamlanmas�n� bekler
        if (targetToDestroy != null)
        {
            
            transform.DOMove(targetToDestroy.transform.position , rotationDuration).SetEase(Ease.InOutQuad);
            // 1. Hedef Rotasyonu Belirle
            // X: -70, Y: 30, Z: 0 (veya mevcut Z de�eri)
            Vector3 targetRotation = new Vector3(30, 0, 0);

            // 2. Animasyonu Ba�lat
            // transform.DOLocalRotate(hedefRotasyon, s�re, d�nd�rmeModu);
            transform.DOLocalRotate(targetRotation, rotationDuration)
                     .SetEase(Ease.InOutQuad).OnComplete(() =>
                     {
                         Debug.LogWarning("Smash animation completed. targetToDestroy: " + targetToDestroy.name);
                         int slotIndex = targetToDestroy.transform.GetSiblingIndex();
                         GameManager.instance.BreakSlotProses(slotIndex);
                         GameObject brokenSlot = OneObjectPool.Instance.GetObjectWhitName(ObjectName.BrokenSlot);
                         brokenSlot.transform.position = targetToDestroy.transform.position;
                         targetToDestroy.SetActive(false);
                         targetToDestroy = null;
                         gameObject.SetActive(false);
                         

                     });
        }else
        {
            Debug.LogError(" targetToDestroy is null. Cannot perform smash animation.");
        }

        
    }
    IEnumerator OnThrowCorrotune()
    {

        yield return new WaitForSeconds(2.2f); // Anl�k bir gecikme ekleyerek animasyonun tamamlanmas�n� bekler
        if (targetToDestroy != null)
        {
           
            transform.DOMove(targetToDestroy.transform.position+new Vector3(0,0,-3), rotationDuration).SetEase(Ease.InOutQuad);
            // 1. Hedef Rotasyonu Belirle
            // X: -70, Y: 30, Z: 0 (veya mevcut Z de�eri)
            Vector3 targetRotation = new Vector3(30, 0, 0);

            // 2. Animasyonu Ba�lat
            // transform.DOLocalRotate(hedefRotasyon, s�re, d�nd�rmeModu);
            transform.DOLocalRotate(targetRotation, rotationDuration)
                     .SetEase(Ease.InOutQuad).OnComplete(() => {
                         GameManager.instance.BreakEggProses(targetToDestroy);
                         
                         targetToDestroy.SetActive(false);
                         targetToDestroy = null;
                         gameObject.SetActive(false);
                         
                     });
            
            

        }

       
        
    }
    private void OnDisable()
    {
        targetToDestroy = null;
        transform.DOKill(); // Animasyonlar� durdur
        
    }
}