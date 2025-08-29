// Bu kod, UI panelinin kendisine baðlý bir script olarak kullanýlabilir.
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class FinalCountUI : MonoBehaviour
{
    public static FinalCountUI instance;
    public TextMeshProUGUI eggCountText;
    public Image panelImage; // UI panelinin Image bileþeni
    public int totalEggCount ;
    public ParticleSystem fireParticles; // Alev partikül sistemi
    public Color correctColor = Color.green, drawColor= Color.white, incorrectColor= Color.red, originalColor=Color.white;
    public RectTransform glowEffect;

    private int currentCount = 0;
    private bool isFlashing = false;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        GameManager.instance.trueEggCountChanged += UpdateEggCount;
    }
    private void OnDisable()
    {
        GameManager.instance.trueEggCountChanged -= UpdateEggCount;
    }
    public void UpdateEggCount(int newCount)
    {
        totalEggCount = GameManager.instance.GetCheckedEggCount();
        eggCountText.text = newCount.ToString();
      

        if (currentCount >= totalEggCount - 1 && !isFlashing)
        {
            // Hedefe yaklaþýnca animasyon baþlasýn (örneðin 4'te)
            isFlashing = true;

            // Renk ve parlaklýk animasyonu
           // panelImage.DOColor(Color.red, 0.5f).SetLoops(-1, LoopType.Yoyo);
           // eggCountText.DOColor(Color.yellow, 0.5f).SetLoops(-1, LoopType.Yoyo);

            // Titreme efekti
           // panelImage.transform.DOShakePosition(1f, 5, 20).SetLoops(-1);

            // Alev partiküllerini oynat
            if (fireParticles != null)
            {
                fireParticles.Play();
            }
        }else if(currentCount < totalEggCount - 1 )
        {
            if (newCount > currentCount)
            {
              

                // Yaylanma animasyonu
                eggCountText.transform.DOPunchScale(Vector3.one * 0.5f, 1f, 2, .3f);
                eggCountText.DOColor(correctColor, 0.5f).OnComplete(() => { eggCountText.color = originalColor; });

                // Parýltý animasyonu (eðer varsa)
                if (glowEffect != null)
                {
                    glowEffect.DOScale(1f, 0.2f).From(0).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        glowEffect.DOScale(0, 0.3f);
                    });
                }
            }
            else if(newCount < currentCount)
            {
                // Yaylanma animasyonu
                eggCountText.transform.DOPunchScale(Vector3.one * 0.5f, 1f, 2, .3f);
                eggCountText.DOColor(incorrectColor, 0.5f).OnComplete(() => { eggCountText.color = originalColor; });

                // Parýltý animasyonu (eðer varsa)
                if (glowEffect != null)
                {
                    glowEffect.DOScale(1f, 0.2f).From(0).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        glowEffect.DOScale(0, 0.3f);
                    });
                }
            }
            else
            {
                // Beraberlik durumu için farklý bir animasyon
                eggCountText.transform.DOPunchScale(Vector3.one * 0.5f, 1f, 2, .3f);
                eggCountText.DOColor(drawColor, 0.5f).OnComplete(()=> { eggCountText.color = originalColor; });

                // Parýltý animasyonu (eðer varsa)
                if (glowEffect != null)
                {
                    glowEffect.DOScale(1f, 0.2f).From(0).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        glowEffect.DOScale(0, 0.3f);
                    });
                }
            }
            currentCount = newCount;
        }
    
    }
}