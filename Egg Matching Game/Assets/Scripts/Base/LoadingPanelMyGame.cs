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
        // Y�klenecek sahneyi asenkron bir i�lem olarak ba�lat
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
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            

            // E�er y�kleme %90'�n �zerine ��kt�ysa (yani sahne y�klendi ama hen�z aktif de�il)
            if (operation.progress >= 0.9f)
            {
             
                // Burada isterseniz, belirli bir s�re bekleyebilir
                // veya kullan�c� bir tu�a basana/ekrana dokunana kadar bekleyebilirsiniz.
                // �rne�in, 2 saniye bekleyip sahneyi aktifle�tirelim:
                 yield return new WaitForSeconds(4f);

                // Sahnenin aktivasyonuna izin ver
                operation.allowSceneActivation = true;
            }

            // Bir sonraki frame'e ge�mesini bekle
            yield return null;
        }

       
    }
    private void OnDisable()
    {
        StopCoroutine(LoadingPanelSetActiveFalse());
    }
}
