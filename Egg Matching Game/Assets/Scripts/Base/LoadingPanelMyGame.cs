using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingPanelMyGame : MonoBehaviour
{
    // Y�kleme ilerlemesini g�stermek i�in bir UI Text veya Slider referans� eklenebilir
    // public Text progressText;
    // public Slider progressBar;

    private void Awake()
    {
        // Bu GameObject'in sahne ge�i�lerinde yok edilmemesini sa�la
        // Bu, Coroutine'in yeni sahne y�klendi�inde durmamas�n� sa�lar.
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Panel aktif hale geldi�inde y�kleme i�lemini ba�lat
        gameObject.SetActive(true);
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
        yield return new WaitForSeconds(10f);

        // Y�kleme tamamlanana kadar d�ng�y� s�rd�r
        while (!operation.isDone)
        {
            // Y�kleme ilerlemesini 0-0.9 aras�nda bir de�ere normalle�tir
            // Unity, sahne tamamen haz�r olmadan �nce progress de�erini 0.9'da durdurur.
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // 0.9'a b�lerek %0-100 aral���na getir

            // �lerleme �ubu�u veya metin g�ncellemeleri burada yap�labilir
            // if (progressBar != null)
            // {
            //     progressBar.value = progress;
            // }
            // if (progressText != null)
            // {
            //     progressText.text = "Loading: " + (progress * 100f).ToString("F0") + "%";
            // }

            // E�er y�kleme %90'�n �zerine ��kt�ysa (yani sahne y�klendi ama hen�z aktif de�il)
            if (operation.progress >= 0.9f) // %90'a ula�t���nda
            {
                // Burada isterseniz, belirli bir s�re bekleyebilir
                // veya kullan�c� bir tu�a basana/ekrana dokunana kadar bekleyebilirsiniz.
                // �rne�in, 4 saniye bekleyip sahneyi aktifle�tirelim:
               // BURADA 4 SAN�YE BEKLEME SA�LANACAK

                // Sahnenin aktivasyonuna izin ver
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
