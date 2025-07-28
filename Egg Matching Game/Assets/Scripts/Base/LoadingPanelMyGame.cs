using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class LoadingPanelMyGame : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.SetActive(true);
        StartCoroutine(LoadingPanelSetActiveFalse());
    }
   

    IEnumerator LoadingPanelSetActiveFalse()
    {
        // Yüklenecek sahneyi asenkron bir iþlem olarak baþlat
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
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            

            // Eðer yükleme %90'ýn üzerine çýktýysa (yani sahne yüklendi ama henüz aktif deðil)
            if (operation.progress >= 0.9f)
            {
             
                // Burada isterseniz, belirli bir süre bekleyebilir
                // veya kullanýcý bir tuþa basana/ekrana dokunana kadar bekleyebilirsiniz.
                // Örneðin, 2 saniye bekleyip sahneyi aktifleþtirelim:
                 yield return new WaitForSeconds(4f);

                // Sahnenin aktivasyonuna izin ver
                operation.allowSceneActivation = true;
            }

            // Bir sonraki frame'e geçmesini bekle
            yield return null;
        }

       
    }
    private void OnDisable()
    {
        StopCoroutine(LoadingPanelSetActiveFalse());
    }
}
