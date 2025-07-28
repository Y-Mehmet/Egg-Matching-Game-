using UnityEngine;

public class SceeneManager : MonoBehaviour
{
    public static SceeneManager instance;
   
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LoadScene(int level=1)
    {
        level++;
            UnityEngine.SceneManagement.SceneManager.LoadScene(level);
       
    }
    public void LoadSceneAndEnergyPanel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        if(PanelManager.Instance!= null)
        {
            PanelManager.Instance.ShowPanel(PanelID.RefillEnergyPanel);
        }
        else
        {
            Debug.LogWarning("PanelManager instance is null. Cannot show RefillEnergyPanel.");
        }


    }
}
