using UnityEngine;
using TMPro;
using DG.Tweening; // DoTween k�t�phanesini ekliyoruz
using System.Collections;
using System; // Action kullanmak i�in

public class CountdownController : MonoBehaviour
{
    [Header("UI Referanslar�")]
      // Ba�lang�� paneli (i�inde buton olan)
    public TMP_Text countdownText;       // 3, 2, 1 yaz�s�n� g�sterecek text

    [Header("Animasyon Ayarlar�")]
    public float moveDistance = 1000f;    // Metnin ne kadar yukar�dan/a�a��ya gidece�i
    public float moveInDuration = 0.4f;  // Ekrana giri� animasyon s�resi
    public float pauseDuration = 0.5f;   // Metnin ekranda kalma s�resi
    public float moveOutDuration = 0.3f; // Ekrandan ��k�� animasyon s�resi

    
    public static CountdownController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void OnEnable()
    {
       GameManager.instance.gameReStart += StartCountdown; // Oyun yeniden ba�lat�ld���nda geri say�m� ba�lat
    }
    private void OnDisable()
    {
        GameManager.instance.gameReStart -= StartCountdown;
    }

    private void Start()
    {
        // Oyun ba��nda geri say�m metni ve ba�lang�� paneli ayarlar�
        countdownText.gameObject.SetActive(false);
        PanelManager.Instance.ShowPanel(PanelID.TopToStart_Panel, PanelShowBehavior.HIDE_PREVISE); // "Tap to Start" panelini g�steriyoruz
    }

    // Bu metodu "Tap to Start" butonunun OnClick event'ine ba�layaca��z.
    public void StartCountdown()
    {
        PanelManager.Instance.HideLastPanel();
        SoundManager.instance.PlaySfx(SoundType.Temp);
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        countdownText.gameObject.SetActive(true);

        // --- Geri Say�m D�ng�s� ---

        // "3" i�in animasyon
        yield return AnimateText("3").WaitForCompletion();

        // "2" i�in animasyon
        yield return AnimateText("2").WaitForCompletion();

        // "1" i�in animasyon
        yield return AnimateText("1").WaitForCompletion();

        
        yield return AnimateText("START!").WaitForCompletion();

        // --- Geri Say�m Bitti ---

        countdownText.gameObject.SetActive(false);
        Debug.Log("Geri say�m bitti! Oyun ba�l�yor!");

        // Oyunun ba�lad���n� bildiren event'i tetikle
        GameManager.instance.gameStart?.Invoke();
    }

    /// <summary>
    /// Verilen metni yukar�dan a�a��ya animasyonla getirip g�t�r�r.
    /// </summary>
    /// <param name="textToShow">Ekranda g�sterilecek metin.</param>
    /// <returns>Olu�turulan DoTween Sequence'i.</returns>
    private Sequence AnimateText(string textToShow)
    {
        // Metni ve ba�lang�� pozisyonunu ayarla
        countdownText.text = textToShow;
        RectTransform rectTransform = countdownText.rectTransform;

        // Ba�lang�� pozisyonu (ekran�n �st�) ve alfa de�eri (g�r�n�r)
        rectTransform.anchoredPosition = new Vector2(0, moveDistance);
        countdownText.alpha = 1f;

        // DoTween Sequence olu�turuyoruz. Bu, animasyonlar� ard���k �al��t�rmam�z� sa�lar.
        Sequence sequence = DOTween.Sequence();

        // 1. Animasyon: Metni yukar�dan ekran�n ortas�na getir (Y=0)
        sequence.Append(rectTransform.DOAnchorPosY(0, moveInDuration).SetEase(Ease.OutQuad));

        // 2. Bekleme: Metin ekranda bir s�re bekler
        if(textToShow != "START!")
        {
            sequence.AppendInterval(pauseDuration);
            // 3. Animasyon: Metni ortadan a�a��ya do�ru g�t�r�rken ayn� anda saydamla�t�r
            // Append: S�raya yeni animasyon ekler (a�a�� hareket)
            // Join: Son eklenen animasyonla AYNI ANDA �al��acak yeni bir animasyon ekler (saydamla�ma)
            sequence.Append(rectTransform.DOAnchorPosY(-moveDistance, moveOutDuration).SetEase(Ease.InQuad))
                     .Join(countdownText.DOFade(0, moveOutDuration));
        }
        else
        {
            sequence.Append(rectTransform.DOAnchorPosX(moveDistance, moveOutDuration).SetEase(Ease.InQuad))
                    .Join(countdownText.DOFade(0, moveOutDuration));
            
        }
        
        

        

        return sequence;
    }
}