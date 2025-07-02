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
        // 4. GetComponent'i bir kere çaðýrýp deðiþkende saklamak daha verimlidir.
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        // Listener'ý burada ekliyoruz.
        button.onClick.AddListener(LoadNextScene);

        // Ayarlama iþlemini bir coroutine ile güvenli bir þekilde baþlatýyoruz.
        StartCoroutine(SetupButtonWhenReady());
    }

    private IEnumerator SetupButtonWhenReady()
    {
        // 5. SaveManager hazýr olana kadar bekle.
        //    Bu, script execution order (çalýþma sýrasý) sorunlarýný çözer.
        yield return new WaitUntil(() => SaveManager.Instance != null);

        // SaveManager hazýr olduðunda metni güvenle ayarla.
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
