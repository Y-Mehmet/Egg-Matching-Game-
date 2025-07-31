using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingPanelMyGame : MonoBehaviour
{
    // Yükleme ilerlemesini göstermek için bir UI Text veya Slider referansý eklenebilir
    // public Text progressText;
    // public Slider progressBar;

    private void Awake()
    {
        // Bu GameObject'in sahne geçiþlerinde yok edilmemesini saðla
        // Bu, Coroutine'in yeni sahne yüklendiðinde durmamasýný saðlar.
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Panel aktif hale geldiðinde yükleme iþlemini baþlat
        gameObject.SetActive(true);
        StartCoroutine(LoadSceneAsyncAndDeactivatePanel());
    }

    IEnumerator LoadSceneAsyncAndDeactivatePanel()
    {
        // Yüklenecek sahneyi asenkron bir iþlem olarak baþlat
        // Sahne 1'i yükleyecek (Build Settings'deki sahne sýrasý)
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);

        // Sahne hazýr olduðunda otomatik aktivasyonu kapat
        // Bu, yükleme %90'a geldiðinde bile sahnenin hemen geçiþ yapmamasýný saðlar.
        // Biz izin verene kadar bekler.
        operation.allowSceneActivation = false;
        yield return new WaitForSeconds(10f);

        // Yükleme tamamlanana kadar döngüyü sürdür
        while (!operation.isDone)
        {
            // Yükleme ilerlemesini 0-0.9 arasýnda bir deðere normalleþtir
            // Unity, sahne tamamen hazýr olmadan önce progress deðerini 0.9'da durdurur.
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // 0.9'a bölerek %0-100 aralýðýna getir

            // Ýlerleme çubuðu veya metin güncellemeleri burada yapýlabilir
            // if (progressBar != null)
            // {
            //     progressBar.value = progress;
            // }
            // if (progressText != null)
            // {
            //     progressText.text = "Loading: " + (progress * 100f).ToString("F0") + "%";
            // }

            // Eðer yükleme %90'ýn üzerine çýktýysa (yani sahne yüklendi ama henüz aktif deðil)
            if (operation.progress >= 0.9f) // %90'a ulaþtýðýnda
            {
                // Burada isterseniz, belirli bir süre bekleyebilir
                // veya kullanýcý bir tuþa basana/ekrana dokunana kadar bekleyebilirsiniz.
                // Örneðin, 4 saniye bekleyip sahneyi aktifleþtirelim:
               // BURADA 4 SANÝYE BEKLEME SAÐLANACAK

                // Sahnenin aktivasyonuna izin ver
                operation.allowSceneActivation = true;
            }

            // Bir sonraki frame'e geçmesini bekle
            yield return null;
        }

        // Yeni sahne tamamen yüklendiðinde ve aktif hale geldiðinde,
        // bu yükleme panelini devre dýþý býrak veya yok et.
        // Yok etmek daha temiz bir çözümdür, çünkü DontDestroyOnLoad ile iþaretlendi.
        Destroy(gameObject);
    }

    // OnDisable metodu, bu GameObject manuel olarak devre dýþý býrakýldýðýnda veya yok edildiðinde çaðrýlýr.
    // Ancak DontDestroyOnLoad kullanýldýðýnda, sahne geçiþinde otomatik olarak çaðrýlmaz.
    // Bu yüzden Destroy(gameObject) ile manuel olarak yok etmeliyiz.
    private void OnDisable()
    {
        // Coroutine'i durdurmak genellikle iyi bir uygulamadýr,
        // ancak Destroy(gameObject) çaðrýldýðýnda zaten duracaktýr.
        StopAllCoroutines();
    }
}
