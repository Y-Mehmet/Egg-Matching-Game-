using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    public int startTime = 180;
    public bool mixColor = false;
    public bool dualColor = false;
    public void RestartLevelData()
    {
        if(tempTopEggColors.Count > 0)
        {
            tempTopEggColors.Clear();
        }
        SaveGameData gameData = SaveSystem.Load();

        if (topEggColors.Count > 0 &&  !gameData.isTutorial)
        {
            topEggColors.Clear();
        }
        brokenEggCount = 0;
        brokenSlotCount = 0;
        GetTopEggColorList();
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
    public List<EggColor> GetTopEggColorList()
    {
        if(GameManager.instance.gameData.isTutorial)
        {
            return topEggColors;
        }
        List<EggColor> tempEggColorList = eggColors;
        tempEggColorList = tempEggColorList.OrderBy(x => Random.value).ToList(); // Listeyi rastgele sýralayarak karýþtýr
        int perCount = GameManager.instance.GetLevelData().topEggPerCount;
        // Renklerin birbirinden farklý olup olmadýðýný kontrol et
        bool allColorsDistinct = eggColors.Distinct().Count() == eggColors.Count;
        
        if (!allColorsDistinct && !dualColor)
        {
            // Renkler birbirinden farklý deðilse ve dualColor false ise
            // Hangi renkten kaç tane olduðunu bul
            var colorCounts = eggColors
                .GroupBy(color => color)
                .Select(group => new { Color = group.Key, Count = group.Count() })
                .ToList();

            Debug.Log("eggColors'taki renklerin sayýsý:");
            foreach (var colorCount in colorCounts)
            {
                foreach (var color in tempEggColorList)
                {
                    if(colorCount.Color == color )
                    {
                        if(colorCount.Count > 1 && colorCount.Count <= perCount)
                        {
                            if (topEggColors.Contains(color))
                            {
                                EggColor randomEgg = ColorManager.instance.GetRandomColor();
                                for (int i = 0; i < perCount; i++)
                                {
                                   
                                    topEggColors.Add(randomEgg);

                                }
                            }
                            else
                            {
                                for (int i = 0; i < perCount; i++)
                                {

                                    topEggColors.Add(color);

                                }

                            }
                        }
                        else
                        {
                            for (int i = 0; i < perCount; i++)
                            {

                                topEggColors.Add(color);

                            }
                        }

                    }

                   
                }
            }
            
        }else
        {
            foreach (var color in tempEggColorList)
            {
                for (int i = 0; i < perCount; i++)
                {

                    topEggColors.Add(color);

                }
            }
               
        }

           
       
        return topEggColors;
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
