using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;


public class LoadingPanelMyGame : MonoBehaviour
{
    [SerializeField] Image myGameLogo;

    private void Awake()
    {
   
    }

    private void OnEnable()
    {
        // Panel aktif hale geldi�inde y�kleme i�lemini ba�lat
        gameObject.SetActive(true);
        
    }
    void Start()
    {
        GameAnalyticsSDK.GameAnalytics.Initialize();
       // FB.Init();
       
        StartCoroutine(ChangeAlfa());
    }

    private IEnumerator ChangeAlfa()
    {
        // Ba�lang��ta belirledi�imiz s�re kadar bekle.
        // Bu s�re, videonun y�klenmeye ba�lamas� i�in zaman tan�r.
        yield return new WaitForSeconds(5);

        float gecenZaman = 0f;
        Color baslangicRengi = myGameLogo.color;

        // Alfa de�eri 0 olana kadar bu d�ng� �al��acak.
        while (gecenZaman < 1)
        {
            // Ge�en s�reyi her frame'de art�r.
            gecenZaman += Time.deltaTime;

            // Rengin alfa de�erini zamanla 1'den 0'a do�ru d���r.
            float yeniAlfa = Mathf.Lerp(1f, 0f, gecenZaman / 1);
            myGameLogo.color = new Color(baslangicRengi.r, baslangicRengi.g, baslangicRengi.b, yeniAlfa);

            // Bir sonraki frame'e kadar bekle.
            yield return null;
        }

        StartCoroutine(LoadSceneAsyncAndDeactivatePanel());

    }

    IEnumerator LoadSceneAsyncAndDeactivatePanel()
    {
        // Y�klenecek sahneyi asenkron bir i�lem olarak ba�lat
        // Sahne 1'i y�kleyecek (Build Settings'deki sahne s�ras�)
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);

        // Sahne haz�r oldu�unda otomatik aktivasyonu kapat
        // Bu, y�kleme %90'a geldi�inde bile sahnenin hemen ge�i� yapmamas�n� sa�lar.
        // Biz izin verene kadar bekler.
        operation.allowSceneActivation = false;
        

        // Y�kleme tamamlanana kadar d�ng�y� s�rd�r
        while (!operation.isDone)
        {
            // Y�kleme ilerlemesini 0-0.9 aras�nda bir de�ere normalle�tir
            // Unity, sahne tamamen haz�r olmadan �nce progress de�erini 0.9'da durdurur.
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // 0.9'a b�lerek %0-100 aral���na getir

        

            // E�er y�kleme %90'�n �zerine ��kt�ysa (yani sahne y�klendi ama hen�z aktif de�il)
            if (operation.progress >= 0.9f) // %90'a ula�t���nda
            {
             
                operation.allowSceneActivation = true;
            }

            // Bir sonraki frame'e ge�mesini bekle
            yield return null;
        }

        // Yeni sahne tamamen y�klendi�inde ve aktif hale geldi�inde,
        // bu y�kleme panelini devre d��� b�rak veya yok et.
        // Yok etmek daha temiz bir ��z�md�r, ��nk� DontDestroyOnLoad ile i�aretlendi.
        Destroy(gameObject);
    }

    // OnDisable metodu, bu GameObject manuel olarak devre d��� b�rak�ld���nda veya yok edildi�inde �a�r�l�r.
    // Ancak DontDestroyOnLoad kullan�ld���nda, sahne ge�i�inde otomatik olarak �a�r�lmaz.
    // Bu y�zden Destroy(gameObject) ile manuel olarak yok etmeliyiz.
    private void OnDisable()
    {
        // Coroutine'i durdurmak genellikle iyi bir uygulamad�r,
        // ancak Destroy(gameObject) �a�r�ld���nda zaten duracakt�r.
        StopAllCoroutines();
    }
}
