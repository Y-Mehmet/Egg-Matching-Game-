using UnityEngine;
using DG.Tweening; // DOTween k�t�phanesini kullanmak i�in bu sat�r gerekli
using System.Collections.Generic; // List kullanabilmek i�in

public class FragmentController : MonoBehaviour
{
    [Header("Animasyon Ayarlar�")]
    [Tooltip("Par�alar�n ne kadar uza�a f�rlayaca��n� belirler.")]
    public float explosionForce = 2f;

    [Tooltip("Fizik kullanmadan yer�ekimi hissi vermek i�in a�a�� y�nl� kuvvet.")]
    public float gravityForce = 3f;

    [Tooltip("Par�alar�n ne kadar d�nece�ini belirler.")]
    public float rotationStrength = 360f;

    [Tooltip("Animasyonun toplam s�resi. Bu s�re sonunda par�alar kaybolur.")]
    public float animationDuration = 3f;


    // Her par�an�n ba�lang�� pozisyonunu ve rotasyonunu saklamak i�in bir liste
    private List<Transform> fragments = new List<Transform>();
    private List<Vector3> initialPositions = new List<Vector3>();
    private List<Quaternion> initialRotations = new List<Quaternion>();

    private bool isInitialized = false;

    // Awake fonksiyonu, obje ilk yarat�ld���nda sadece bir kez �al���r.
    // Ba�lang�� durumlar�n� kaydetmek i�in en iyi yerdir.
    void Awake()
    {
        InitializeFragments();
    }

    // Bu fonksiyon, �ocuk objelerin ba�lang�� durumlar�n� kaydeder.
    private void InitializeFragments()
    {
        if (isInitialized) return; // E�er daha �nce y�klendiyse tekrar yapma

        // Bu objenin alt�ndaki t�m �ocuklar� (par�alar�) bul ve listeye ekle
        foreach (Transform child in transform)
        {
            fragments.Add(child);
            initialPositions.Add(child.localPosition); // Parent'a g�re yerel pozisyonu kaydet
            initialRotations.Add(child.localRotation); // Parent'a g�re yerel rotasyonu kaydet
        }

        isInitialized = true;
    }

    // Bu script'in ba�l� oldu�u GameObject aktif oldu�unda �al���r.
    void OnEnable()
    {
        // E�er ba�lang�� durumu al�nmad�ysa, �imdi al.
        if (!isInitialized)
        {
            InitializeFragments();
        }

        // Par�alanma animasyonunu ba�lat
        Shatter();
    }

    // Bu script'in ba�l� oldu�u GameObject pasif oldu�unda �al���r.
    void OnDisable()
    {
        // T�m animasyonlar� durdur ve par�alar� eski haline getir
        ResetFragments();
    }

    private void Shatter()
    {
        for (int i = 0; i < fragments.Count; i++)
        {
            Transform fragment = fragments[i];

            // Animasyona ba�lamadan �nce par�an�n aktif oldu�undan emin ol
            fragment.gameObject.SetActive(true);

            // Rastgele bir patlama y�n� belirle (sa�a, sola, �ne, arkaya)
            Vector3 explosionDirection = Random.insideUnitSphere * explosionForce;

            // Yer�ekimi etkisi i�in a�a�� y�nl� bir kuvvet ekle
            Vector3 gravityEffect = Vector3.down * gravityForce;

            // Hedef pozisyonu, ba�lang�� pozisyonuna bu iki kuvveti ekleyerek bul
            Vector3 targetPosition = fragment.localPosition + explosionDirection + gravityEffect;

            // Rastgele bir d�nme hedefi belirle
            Vector3 targetRotation = Random.insideUnitSphere * rotationStrength;

            // DOTween ile hareket animasyonunu ba�lat
            fragment.DOLocalMove(targetPosition, animationDuration)
                .SetEase(Ease.OutQuad); // Ba�lang��ta h�zl�, sonra yava�layan bir hareket

            // DOTween ile rotasyon animasyonunu ba�lat
            fragment.DOLocalRotate(targetRotation, animationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    // Animasyon bitti�inde par�ay� pasif hale getir (Destroy etme)
                    fragment.gameObject.SetActive(false);
                });
        }
    }

    private void ResetFragments()
    {
        for (int i = 0; i < fragments.Count; i++)
        {
            Transform fragment = fragments[i];

            // Par�a �zerinde �al��an t�m DOTween animasyonlar�n� an�nda durdur.
            // Bu, OnDisable oldu�unda animasyonun arka planda devam etmesini engeller.
            fragment.DOKill();

            // Par�ay� ba�lang�� pozisyonuna ve rotasyonuna geri getir
            fragment.localPosition = initialPositions[i];
            fragment.localRotation = initialRotations[i];

            // Par�ay� g�r�n�r/aktif yap ki bir sonraki OnEnable'da haz�r olsun
            fragment.gameObject.SetActive(true);
        }
    }
}