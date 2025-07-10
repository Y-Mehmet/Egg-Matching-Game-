using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class HammerAnimator : MonoBehaviour
{
    [HideInInspector]
    public GameObject targetToDestroy;
    private float rotationDuration = 2; // Animasyonun süresi
    private Vector3 startPos= new Vector3(0,-3, -0.5f); // Baþlangýç pozisyonu

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

        yield return new WaitForSeconds(.2f); // Anlýk bir gecikme ekleyerek animasyonun tamamlanmasýný bekler
        if (targetToDestroy != null)
        {
            transform.DOMove(targetToDestroy.transform.position, rotationDuration).SetEase(Ease.InOutQuad);
            // 1. Hedef Rotasyonu Belirle
            // X: -70, Y: 30, Z: 0 (veya mevcut Z deðeri)
            Vector3 targetRotation = new Vector3(30, 0, 0);

            // 2. Animasyonu Baþlat
            // transform.DOLocalRotate(hedefRotasyon, süre, döndürmeModu);
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

        // Animasyon bittiði için çekiç objesini de yok et
        gameObject.SetActive(false);
    }
    IEnumerator OnThrowCorrotune()
    {

        yield return new WaitForSeconds(.2f); // Anlýk bir gecikme ekleyerek animasyonun tamamlanmasýný bekler
        if (targetToDestroy != null)
        {
            transform.localRotation = new Quaternion(-70,0,0,0); // Rotasyonu sýfýrla
            transform.DOMove(targetToDestroy.transform.position, rotationDuration).SetEase(Ease.InOutQuad);
            // 1. Hedef Rotasyonu Belirle
            // X: -70, Y: 30, Z: 0 (veya mevcut Z deðeri)
            Vector3 targetRotation = new Vector3(30, 0, 0);

            // 2. Animasyonu Baþlat
            // transform.DOLocalRotate(hedefRotasyon, süre, döndürmeModu);
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
        transform.DOKill(); // Animasyonlarý durdur
        transform.localRotation= Quaternion.identity; // Rotasyonu sýfýrla
        transform.localPosition = startPos; // Pozisyonu sýfýrla
    }
}