using UnityEngine;

public class HammerAnimator : MonoBehaviour
{
    [HideInInspector]
    public GameObject targetToDestroy;

    // BU METOD, ÇEKÝÇ ANÝMASYONUNUN 'VURMA' ANININ EN SONUNDA
    // BÝR 'ANIMATION EVENT' ÝLE ÇAÐRILACAKTIR.
    public void OnSmashAnimationComplete()
    {
        // Hedef objeyi yok et
        if (targetToDestroy != null)
        {
            // Ýsteðe baðlý: Patlama efekti, ses vb. burada eklenebilir.
            Destroy(targetToDestroy);
        }

        // Animasyon bittiði için çekiç objesini de yok et
        Destroy(gameObject);
    }
}