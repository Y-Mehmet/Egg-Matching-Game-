using System.Linq;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;
    [SerializeField] GameObject tooltipPanel;
    [SerializeField] TooltipDataHolder tooltipDataHolder;
    private void Awake()
    {
        if(instance!= null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }
    private void OnEnable()
    {
        

       
        if (GetTooltipIndex()!= -1)
        {
            tooltipPanel.SetActive(true);
        }else
        {
            tooltipPanel.SetActive(false);
        }

    }
    private void OnDisable()
    {
        tooltipPanel.SetActive(false);
    }
    public int GetTooltipIndex()
    {
        int currentLevelIndex = ResourceManager.Instance.GetResourceAmount(ResourceType.LevelIndex);
        return tooltipDataHolder.tooltips
            .FindIndex(tooltip => tooltip.reqLevelIndex == currentLevelIndex);
    }
    public Tooltip GetTooltip()
    {
        return tooltipDataHolder.tooltips[GetTooltipIndex()];
    }
}
