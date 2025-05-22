using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadNextSceen : MonoBehaviour
{
    public TMP_Text levelText;
    
    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener( LoadNextScene);
        levelText.text = "Level " + SceeneManager.instance.level.ToString();

    }
    private void OnDisable()
    {

        GetComponent<Button>().onClick.RemoveListener(LoadNextScene);
    }
    private void LoadNextScene()
    {
        SceeneManager.instance.level++;
        SceeneManager.instance.LoadScene(SceeneManager.instance.level);
    }
}
