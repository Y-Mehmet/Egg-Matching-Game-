using UnityEngine;

public class HammerAnimator : MonoBehaviour
{
    [HideInInspector]
    public GameObject targetToDestroy;

    // BU METOD, �EK�� AN�MASYONUNUN 'VURMA' ANININ EN SONUNDA
    // B�R 'ANIMATION EVENT' �LE �A�RILACAKTIR.
    public void OnSmashAnimationComplete()
    {
        // Hedef objeyi yok et
        if (targetToDestroy != null)
        {
            // �ste�e ba�l�: Patlama efekti, ses vb. burada eklenebilir.
            Destroy(targetToDestroy);
        }

        // Animasyon bitti�i i�in �eki� objesini de yok et
        Destroy(gameObject);
    }
}