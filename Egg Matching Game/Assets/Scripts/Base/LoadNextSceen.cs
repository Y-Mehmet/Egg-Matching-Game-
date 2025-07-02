using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadNextSceen : MonoBehaviour
{
    public TMP_Text levelText;
    private Button button;
    private SaveGameData gameData;

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
        if (levelText != null)
        {
            gameData = SaveSystem.Load();
            levelText.text = (gameData.levelIndex+1).ToString();
            Debug.Log("Level text set to: " + levelText.text);
        }
    }

   
    private void OnDisable()
    {

        button.onClick.RemoveListener(LoadNextScene);
    }
    private void LoadNextScene()
    {
       
        SceeneManager.instance.LoadScene(1);
    }
}
