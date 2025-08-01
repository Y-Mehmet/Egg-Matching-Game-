using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Bu script'i, ekraný kaplayan ve yavaþça yok olmasýný istediðiniz
// siyah UI Image nesnesine ekleyin.
[RequireComponent(typeof(Image))]
public class Load : MonoBehaviour
{
    [Tooltip("Panelin tamamen görünmez hale gelmesi ne kadar sürsün (saniye)?")]
    public float fadeSuresi = 1.5f;

    [Tooltip("Kararma baþlamadan önce ne kadar beklensin (saniye)?")]
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
        // Alfa deðerini deðiþtirme iþlemini baþlatýyoruz.
        StartCoroutine(ChangeAlfa());
    }

    private IEnumerator ChangeAlfa()
    {
        // Baþlangýçta belirlediðimiz süre kadar bekle.
        // Bu süre, videonun yüklenmeye baþlamasý için zaman tanýr.
        yield return new WaitForSeconds(baslangicGecikmesi);

        float gecenZaman = 0f;
        Color baslangicRengi = panelImage.color;

        // Alfa deðeri 0 olana kadar bu döngü çalýþacak.
        while (gecenZaman < fadeSuresi)
        {
            // Geçen süreyi her frame'de artýr.
            gecenZaman += Time.deltaTime;

            // Rengin alfa deðerini zamanla 1'den 0'a doðru düþür.
            float yeniAlfa = Mathf.Lerp(1f, 0f, gecenZaman / fadeSuresi);
            panelImage.color = new Color(baslangicRengi.r, baslangicRengi.g, baslangicRengi.b, yeniAlfa);

            // Bir sonraki frame'e kadar bekle.
            yield return null;
        }

        // Ýþlem bittiðinde panelin GameObject'ini tamamen devre dýþý býrak.
        gameObject.SetActive(false);
    }
}