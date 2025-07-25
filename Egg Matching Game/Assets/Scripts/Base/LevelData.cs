using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("slot")]
    public List<EggColor> eggColors;// slot egg 
    public List<EggColor> topEggColors;
    public List<EggColor> tempTopEggColors;
    public int brokenEggCount = 0;
    public int brokenSlotCount = 0;
    public int topEggPerCount = 1;
    public void RestartLevelData()
    {
        if(tempTopEggColors.Count > 0)
        {
            tempTopEggColors.Clear();
        }
        brokenEggCount = 0;
        brokenSlotCount = 0;
    }
    public int GetSlotCount()
    {
        return eggColors.Count;
    }
    public int GetTopEggCount()
    {
        return topEggColors.Count;
    }
    public int GetBrokenEggCount()
    {
        return brokenEggCount;
    }
    public int GetBrokenSlotCount()
    {
        return brokenSlotCount;
    }
    public int GetTopEggPerCount()
    {
        return topEggPerCount;
    }
    public List<EggColor> GetTempTopEggColorList()
    {
        if (tempTopEggColors == null || tempTopEggColors.Count == 0)
        {
            foreach (var item in topEggColors)
            {
                tempTopEggColors.Add(item);
            }
        }
        return tempTopEggColors;
    }





    public void RemoveEggByEggColor(EggColor color)
    {
        GetTempTopEggColorList();
        if (tempTopEggColors == null || tempTopEggColors.Count==0)
        {
           foreach(var item in topEggColors)
            {
                tempTopEggColors.Add(item);
            }
        }
        Debug.LogWarning(" level data remove egg by color  " + color +" temp eggg count "+tempTopEggColors.Count);
        foreach (EggColor item in tempTopEggColors)
        {
            
            if (item.ToString() == color.ToString())
            {
                tempTopEggColors.Remove(item);
                brokenEggCount++;
                Debug.Log("Removed egg color: " + item + ", Broken count: " + brokenEggCount);
                break;
            }else
            {
                Debug.LogWarning(item + " does not match " + color + ", not removed.");
            }
        }
    }
    public void RemoveSlotByEggColor()
    {
        brokenSlotCount++;
    }
}
