using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public static TradeManager insrance;

    private void Awake()
    {
        if (insrance == null)
        {
            insrance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
   
}
