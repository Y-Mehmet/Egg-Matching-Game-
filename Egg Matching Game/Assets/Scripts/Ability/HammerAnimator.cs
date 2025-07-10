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

        yield return new WaitForSeconds(.2f); // Anl�k bir gecikme ekleyerek animasyonun tamamlanmas�n� bekler
        if (targetToDestroy != null)
        {
            transform.DOMove(targetToDestroy.transform.position, rotationDuration).SetEase(Ease.InOutQuad);
            // 1. Hedef Rotasyonu Belirle
            // X: -70, Y: 30, Z: 0 (veya mevcut Z de�eri)
            Vector3 targetRotation = new Vector3(30, 0, 0);

            // 2. Animasyonu Ba�lat
            // transform.DOLocalRotate(hedefRotasyon, s�re, d�nd�rmeModu);
            transform.DOLocalRotate(targetRotation, rotationDuration, RotateMode.Fast)
                     .SetEase(Ease.InOutQuad).OnComplete(() =>
                     {
                         int slotIndex = targetToDestroy.transform.GetSiblingIndex();
                         GameManager.instance.BreakSlotProses(slotIndex);

                         targetToDestroy.SetActive(false);
                         gameObject.SetActive(false);
                         targetToDestroy = null;

                     });
        }

        // Animasyon bitti�i i�in �eki� objesini de yok et
        gameObject.SetActive(false);
    }
    IEnumerator OnThrowCorrotune()
    {

        yield return new WaitForSeconds(.2f); // Anl�k bir gecikme ekleyerek animasyonun tamamlanmas�n� bekler
        if (targetToDestroy != null)
        {
            transform.localRotation = new Quaternion(-70,0,0,0); // Rotasyonu s�f�rla
            transform.DOMove(targetToDestroy.transform.position, rotationDuration).SetEase(Ease.InOutQuad);
            // 1. Hedef Rotasyonu Belirle
            // X: -70, Y: 30, Z: 0 (veya mevcut Z de�eri)
            Vector3 targetRotation = new Vector3(30, 0, 0);

            // 2. Animasyonu Ba�lat
            // transform.DOLocalRotate(hedefRotasyon, s�re, d�nd�rmeModu);
            transform.DOLocalRotate(targetRotation, rotationDuration, RotateMode.Fast)
                     .SetEase(Ease.InOutQuad).OnComplete(() => {
                         GameManager.instance.BreakEggProses(targetToDestroy);
                         targetToDestroy.SetActive(false);
                         gameObject.SetActive(false);
                         targetToDestroy = null;
                     });
            
            

        }

       
        
    }
    private void OnDisable()
    {
        targetToDestroy = null;
        transform.DOKill(); // Animasyonlar� durdur
        transform.localRotation= Quaternion.identity; // Rotasyonu s�f�rla
        transform.localPosition = startPos; // Pozisyonu s�f�rla
    }
}