using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Bu script'i, ekran� kaplayan ve yava��a yok olmas�n� istedi�iniz
// siyah UI Image nesnesine ekleyin.
[RequireComponent(typeof(Image))]
public class Load : MonoBehaviour
{
    [Tooltip("Panelin tamamen g�r�nmez hale gelmesi ne kadar s�rs�n (saniye)?")]
    public float fadeSuresi = 1.5f;

    [Tooltip("Kararma ba�lamadan �nce ne kadar beklensin (saniye)?")]
    public float baslangicGecikmesi = 1.0f;

    private Image panelImage;

    void Awake()
    {
        gameObject.SetActive(true);
        panelImage = GetComponent<Image>();
    } 


    void Start()
    {
        // Paneli aktif hale getiriyoruz.
        // Alfa de�erini de�i�tirme i�lemini ba�lat�yoruz.
        StartCoroutine(ChangeAlfa());
    }

    private IEnumerator ChangeAlfa()
    {
        // Ba�lang��ta belirledi�imiz s�re kadar bekle.
        // Bu s�re, videonun y�klenmeye ba�lamas� i�in zaman tan�r.
        yield return new WaitForSeconds(baslangicGecikmesi);

        float gecenZaman = 0f;
        Color baslangicRengi = panelImage.color;

        // Alfa de�eri 0 olana kadar bu d�ng� �al��acak.
        while (gecenZaman < fadeSuresi)
        {
            // Ge�en s�reyi her frame'de art�r.
            gecenZaman += Time.deltaTime;

            // Rengin alfa de�erini zamanla 1'den 0'a do�ru d���r.
            float yeniAlfa = Mathf.Lerp(1f, 0f, gecenZaman / fadeSuresi);
            panelImage.color = new Color(baslangicRengi.r, baslangicRengi.g, baslangicRengi.b, yeniAlfa);

            // Bir sonraki frame'e kadar bekle.
            yield return null;
        }

        // ��lem bitti�inde panelin GameObject'ini tamamen devre d��� b�rak.
        gameObject.SetActive(false);
    }
}