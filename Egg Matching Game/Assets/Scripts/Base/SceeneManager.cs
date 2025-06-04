using UnityEngine;

public class SceeneManager : MonoBehaviour
{
    public static SceeneManager instance;
    //public int level = 1;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LoadScene(int level=1)
    {
        if(level<= 4)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(level-1);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(5);
        }
    }
}
