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
        
            UnityEngine.SceneManagement.SceneManager.LoadScene(level);
       
    }
}
