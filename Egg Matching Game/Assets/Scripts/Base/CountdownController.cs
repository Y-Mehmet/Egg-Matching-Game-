using UnityEngine;
using TMPro;
using DG.Tweening; // DoTween kütüphanesini ekliyoruz
using System.Collections;
using System; // Action kullanmak için

public class CountdownController : MonoBehaviour
{
    [Header("UI Referanslarý")]
      // Baþlangýç paneli (içinde buton olan)
    public TMP_Text countdownText;       // 3, 2, 1 yazýsýný gösterecek text

    [Header("Animasyon Ayarlarý")]
    public float moveDistance = 1000f;    // Metnin ne kadar yukarýdan/aþaðýya gideceði
    public float moveInDuration = 0.4f;  // Ekrana giriþ animasyon süresi
    public float pauseDuration = 0.5f;   // Metnin ekranda kalma süresi
    public float moveOutDuration = 0.3f; // Ekrandan çýkýþ animasyon süresi

    
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
       GameManager.instance.gameReStart += StartCountdown; // Oyun yeniden baþlatýldýðýnda geri sayýmý baþlat
    }
    private void OnDisable()
    {
        GameManager.instance.gameReStart -= StartCountdown;
    }

    private void Start()
    {
        // Oyun baþýnda geri sayým metni ve baþlangýç paneli ayarlarý
        countdownText.gameObject.SetActive(false);
        PanelManager.Instance.ShowPanel(PanelID.TopToStart_Panel, PanelShowBehavior.HIDE_PREVISE); // "Tap to Start" panelini gösteriyoruz
    }

    // Bu metodu "Tap to Start" butonunun OnClick event'ine baðlayacaðýz.
    public void StartCountdown()
    {
        PanelManager.Instance.HideLastPanel();
        SoundManager.instance.PlaySfx(SoundType.Temp);
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        countdownText.gameObject.SetActive(true);

        // --- Geri Sayým Döngüsü ---

        // "3" için animasyon
        yield return AnimateText("3").WaitForCompletion();

        // "2" için animasyon
        yield return AnimateText("2").WaitForCompletion();

        // "1" için animasyon
        yield return AnimateText("1").WaitForCompletion();

        
        yield return AnimateText("START!").WaitForCompletion();

        // --- Geri Sayým Bitti ---

        countdownText.gameObject.SetActive(false);
        Debug.Log("Geri sayým bitti! Oyun baþlýyor!");

        // Oyunun baþladýðýný bildiren event'i tetikle
        GameManager.instance.gameStart?.Invoke();
    }

    /// <summary>
    /// Verilen metni yukarýdan aþaðýya animasyonla getirip götürür.
    /// </summary>
    /// <param name="textToShow">Ekranda gösterilecek metin.</param>
    /// <returns>Oluþturulan DoTween Sequence'i.</returns>
    private Sequence AnimateText(string textToShow)
    {
        // Metni ve baþlangýç pozisyonunu ayarla
        countdownText.text = textToShow;
        RectTransform rectTransform = countdownText.rectTransform;

        // Baþlangýç pozisyonu (ekranýn üstü) ve alfa deðeri (görünür)
        rectTransform.anchoredPosition = new Vector2(0, moveDistance);
        countdownText.alpha = 1f;

        // DoTween Sequence oluþturuyoruz. Bu, animasyonlarý ardýþýk çalýþtýrmamýzý saðlar.
        Sequence sequence = DOTween.Sequence();

        // 1. Animasyon: Metni yukarýdan ekranýn ortasýna getir (Y=0)
        sequence.Append(rectTransform.DOAnchorPosY(0, moveInDuration).SetEase(Ease.OutQuad));

        // 2. Bekleme: Metin ekranda bir süre bekler
        if(textToShow != "START!")
        {
            sequence.AppendInterval(pauseDuration);
            // 3. Animasyon: Metni ortadan aþaðýya doðru götürürken ayný anda saydamlaþtýr
            // Append: Sýraya yeni animasyon ekler (aþaðý hareket)
            // Join: Son eklenen animasyonla AYNI ANDA çalýþacak yeni bir animasyon ekler (saydamlaþma)
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