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
    private void OnEnable()
    {
        transform.localPosition = startPos; // Baþlangýç pozisyonunu ayarla
       
        transform.localRotation= Quaternion.Euler(-70,0,0);
    }

    // BU METOD, ÇEKÝÇ ANÝMASYONUNUN 'VURMA' ANININ EN SONUNDA
    // BÝR 'ANIMATION EVENT' ÝLE ÇAÐRILACAKTIR.
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

        yield return new WaitForSeconds(2.2f); // Anlýk bir gecikme ekleyerek animasyonun tamamlanmasýný bekler
        if (targetToDestroy != null)
        {
            
            transform.DOMove(targetToDestroy.transform.position , rotationDuration).SetEase(Ease.InOutQuad);
            // 1. Hedef Rotasyonu Belirle
            // X: -70, Y: 30, Z: 0 (veya mevcut Z deðeri)
            Vector3 targetRotation = new Vector3(30, 0, 0);

            // 2. Animasyonu Baþlat
            // transform.DOLocalRotate(hedefRotasyon, süre, döndürmeModu);
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

        yield return new WaitForSeconds(2.2f); // Anlýk bir gecikme ekleyerek animasyonun tamamlanmasýný bekler
        if (targetToDestroy != null)
        {
           
            transform.DOMove(targetToDestroy.transform.position+new Vector3(0,0,-3), rotationDuration).SetEase(Ease.InOutQuad);
            // 1. Hedef Rotasyonu Belirle
            // X: -70, Y: 30, Z: 0 (veya mevcut Z deðeri)
            Vector3 targetRotation = new Vector3(30, 0, 0);

            // 2. Animasyonu Baþlat
            // transform.DOLocalRotate(hedefRotasyon, süre, döndürmeModu);
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
        transform.DOKill(); // Animasyonlarý durdur
        
    }
}