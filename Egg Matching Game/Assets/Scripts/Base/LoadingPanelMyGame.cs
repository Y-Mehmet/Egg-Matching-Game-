using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using Facebook.Unity;

public class LoadingPanelMyGame : MonoBehaviour
{
    [SerializeField] Image myGameLogo;

    private void Awake()
    {
   
    }

    private void OnEnable()
    {
        // Panel aktif hale geldiðinde yükleme iþlemini baþlat
        gameObject.SetActive(true);
        
    }
    void Start()
    {
        GameAnalyticsSDK.GameAnalytics.Initialize();
        FB.Init();
       
        StartCoroutine(ChangeAlfa());
    }

    private IEnumerator ChangeAlfa()
    {
        // Baþlangýçta belirlediðimiz süre kadar bekle.
        // Bu süre, videonun yüklenmeye baþlamasý için zaman tanýr.
        yield return new WaitForSeconds(5);

        float gecenZaman = 0f;
        Color baslangicRengi = myGameLogo.color;

        // Alfa deðeri 0 olana kadar bu döngü çalýþacak.
        while (gecenZaman < 1)
        {
            // Geçen süreyi her frame'de artýr.
            gecenZaman += Time.deltaTime;

            // Rengin alfa deðerini zamanla 1'den 0'a doðru düþür.
            float yeniAlfa = Mathf.Lerp(1f, 0f, gecenZaman / 1);
            myGameLogo.color = new Color(baslangicRengi.r, baslangicRengi.g, baslangicRengi.b, yeniAlfa);

            // Bir sonraki frame'e kadar bekle.
            yield return null;
        }

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
        

        // Yükleme tamamlanana kadar döngüyü sürdür
        while (!operation.isDone)
        {
            // Yükleme ilerlemesini 0-0.9 arasýnda bir deðere normalleþtir
            // Unity, sahne tamamen hazýr olmadan önce progress deðerini 0.9'da durdurur.
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // 0.9'a bölerek %0-100 aralýðýna getir

        

            // Eðer yükleme %90'ýn üzerine çýktýysa (yani sahne yüklendi ama henüz aktif deðil)
            if (operation.progress >= 0.9f) // %90'a ulaþtýðýnda
            {
             
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
