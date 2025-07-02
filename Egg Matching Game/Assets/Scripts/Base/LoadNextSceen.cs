using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadNextSceen : MonoBehaviour
{
    public TMP_Text levelText;
    private Button button;

    private void Awake()
    {
        // 4. GetComponent'i bir kere �a��r�p de�i�kende saklamak daha verimlidir.
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        // Listener'� burada ekliyoruz.
        button.onClick.AddListener(LoadNextScene);

        // Ayarlama i�lemini bir coroutine ile g�venli bir �ekilde ba�lat�yoruz.
        StartCoroutine(SetupButtonWhenReady());
    }

    private IEnumerator SetupButtonWhenReady()
    {
        // 5. SaveManager haz�r olana kadar bekle.
        //    Bu, script execution order (�al��ma s�ras�) sorunlar�n� ��zer.
        yield return new WaitUntil(() => SaveManager.Instance != null);

        // SaveManager haz�r oldu�unda metni g�venle ayarla.
        if (levelText != null)
        {
            levelText.text = "Level " + SaveManager.Instance.levelIndex;
            Debug.Log("Level text set to: " + levelText.text);
        }
    }
    private void OnDisable()
    {

        button.onClick.RemoveListener(LoadNextScene);
    }
    private void LoadNextScene()
    {
       
        SceeneManager.instance.LoadScene(SaveManager.Instance.levelIndex);
    }
}
