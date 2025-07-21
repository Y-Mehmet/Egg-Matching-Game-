

using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{
   
    public void Purchased(Product product)
    {
        switch(product.definition.id)
        {
            case "gem80":
                ResourceManager.Instance.AddResource(ResourceType.Gem, 80);// 1.99
                Debug.Log("Purchased 80 Gems for $1.99");
                break;
            case "gem240":
                ResourceManager.Instance.AddResource(ResourceType.Gem, 240);// 4.99
                break;
            case "gem500":
                ResourceManager.Instance.AddResource(ResourceType.Gem, 500);// 9.99
                break;
            case "gem1k":
                ResourceManager.Instance.AddResource(ResourceType.Gem, 1000);// 19.99
                break;
            case "gem2k100":
                ResourceManager.Instance.AddResource(ResourceType.Gem, 2100);// 29.99
                break;
        }
    }
}
