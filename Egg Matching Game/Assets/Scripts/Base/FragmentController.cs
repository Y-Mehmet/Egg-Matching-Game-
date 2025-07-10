using UnityEngine;
using DG.Tweening; // DOTween kütüphanesini kullanmak için bu satýr gerekli
using System.Collections.Generic; // List kullanabilmek için

public class FragmentController : MonoBehaviour
{
    [Header("Animasyon Ayarlarý")]
    [Tooltip("Parçalarýn ne kadar uzaða fýrlayacaðýný belirler.")]
    public float explosionForce = 2f;

    [Tooltip("Fizik kullanmadan yerçekimi hissi vermek için aþaðý yönlü kuvvet.")]
    public float gravityForce = 3f;

    [Tooltip("Parçalarýn ne kadar döneceðini belirler.")]
    public float rotationStrength = 360f;

    [Tooltip("Animasyonun toplam süresi. Bu süre sonunda parçalar kaybolur.")]
    public float animationDuration = 3f;


    // Her parçanýn baþlangýç pozisyonunu ve rotasyonunu saklamak için bir liste
    private List<Transform> fragments = new List<Transform>();
    private List<Vector3> initialPositions = new List<Vector3>();
    private List<Quaternion> initialRotations = new List<Quaternion>();

    private bool isInitialized = false;

    // Awake fonksiyonu, obje ilk yaratýldýðýnda sadece bir kez çalýþýr.
    // Baþlangýç durumlarýný kaydetmek için en iyi yerdir.
    void Awake()
    {
        InitializeFragments();
    }

    // Bu fonksiyon, çocuk objelerin baþlangýç durumlarýný kaydeder.
    private void InitializeFragments()
    {
        if (isInitialized) return; // Eðer daha önce yüklendiyse tekrar yapma

        // Bu objenin altýndaki tüm çocuklarý (parçalarý) bul ve listeye ekle
        foreach (Transform child in transform)
        {
            fragments.Add(child);
            initialPositions.Add(child.localPosition); // Parent'a göre yerel pozisyonu kaydet
            initialRotations.Add(child.localRotation); // Parent'a göre yerel rotasyonu kaydet
        }

        isInitialized = true;
    }

    // Bu script'in baðlý olduðu GameObject aktif olduðunda çalýþýr.
    void OnEnable()
    {
        // Eðer baþlangýç durumu alýnmadýysa, þimdi al.
        if (!isInitialized)
        {
            InitializeFragments();
        }

        // Parçalanma animasyonunu baþlat
        Shatter();
    }

    // Bu script'in baðlý olduðu GameObject pasif olduðunda çalýþýr.
    void OnDisable()
    {
        // Tüm animasyonlarý durdur ve parçalarý eski haline getir
        ResetFragments();
    }

    private void Shatter()
    {
        for (int i = 0; i < fragments.Count; i++)
        {
            Transform fragment = fragments[i];

            // Animasyona baþlamadan önce parçanýn aktif olduðundan emin ol
            fragment.gameObject.SetActive(true);

            // Rastgele bir patlama yönü belirle (saða, sola, öne, arkaya)
            Vector3 explosionDirection = Random.insideUnitSphere * explosionForce;

            // Yerçekimi etkisi için aþaðý yönlü bir kuvvet ekle
            Vector3 gravityEffect = Vector3.down * gravityForce;

            // Hedef pozisyonu, baþlangýç pozisyonuna bu iki kuvveti ekleyerek bul
            Vector3 targetPosition = fragment.localPosition + explosionDirection + gravityEffect;

            // Rastgele bir dönme hedefi belirle
            Vector3 targetRotation = Random.insideUnitSphere * rotationStrength;

            // DOTween ile hareket animasyonunu baþlat
            fragment.DOLocalMove(targetPosition, animationDuration)
                .SetEase(Ease.OutQuad); // Baþlangýçta hýzlý, sonra yavaþlayan bir hareket

            // DOTween ile rotasyon animasyonunu baþlat
            fragment.DOLocalRotate(targetRotation, animationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    // Animasyon bittiðinde parçayý pasif hale getir (Destroy etme)
                    fragment.gameObject.SetActive(false);
                });
        }
    }

    private void ResetFragments()
    {
        for (int i = 0; i < fragments.Count; i++)
        {
            Transform fragment = fragments[i];

            // Parça üzerinde çalýþan tüm DOTween animasyonlarýný anýnda durdur.
            // Bu, OnDisable olduðunda animasyonun arka planda devam etmesini engeller.
            fragment.DOKill();

            // Parçayý baþlangýç pozisyonuna ve rotasyonuna geri getir
            fragment.localPosition = initialPositions[i];
            fragment.localRotation = initialRotations[i];

            // Parçayý görünür/aktif yap ki bir sonraki OnEnable'da hazýr olsun
            fragment.gameObject.SetActive(true);
        }
    }
}