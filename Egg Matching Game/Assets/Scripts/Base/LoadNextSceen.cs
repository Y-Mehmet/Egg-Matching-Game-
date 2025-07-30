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
        if (levelText != null)
        {
            
            levelText.text = "Level "+(ResourceManager.Instance.GetResourceAmount(ResourceType.LevelIndex)+1);
            Debug.Log("Level text set to: " + levelText.text);
        }
    }

   
    private void OnDisable()
    {

        button.onClick.RemoveListener(LoadNextScene);
    }
    private void LoadNextScene()
    {
        SoundManager.instance.StopBG();
        SoundManager.instance.PlaySfx(SoundType.btnClick);
        SceeneManager.instance.LoadScene(1);
    }
    
}
