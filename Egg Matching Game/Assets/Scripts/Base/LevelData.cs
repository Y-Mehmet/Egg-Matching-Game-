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
        if (GameManager.instance.gameData.isTutorial)
        {
            return topEggColors;
        }
        List<EggColor> tempEggColorList = eggColors;
        tempEggColorList = tempEggColorList.OrderBy(x => Random.value).ToList();
        int perCount = GameManager.instance.GetLevelData().topEggPerCount;

        // ... (BURADAKÝ LÝSTE DOLDURMA KODUNUZ DEÐÝÞMEDEN AYNI KALIYOR) ...
        bool allColorsDistinct = eggColors.Distinct().Count() == eggColors.Count;
        if (!allColorsDistinct && !dualColor)
        {
            // ... (Bu blok ayný)
            var colorCounts = eggColors.GroupBy(color => color).Select(group => new { Color = group.Key, Count = group.Count() }).ToList();
            Debug.Log("eggColors'taki renklerin sayýsý:");
            foreach (var colorCount in colorCounts)
            {
                foreach (var color in tempEggColorList)
                {
                    if (colorCount.Color == color)
                    {
                        if (colorCount.Count > 1 && colorCount.Count <= perCount)
                        {
                            if (topEggColors.Contains(color))
                            {
                                EggColor randomEgg = ColorManager.instance.GetRandomColor();
                                for (int i = 0; i < perCount; i++) { topEggColors.Add(randomEgg); }
                            }
                            else
                            {
                                for (int i = 0; i < perCount; i++) { topEggColors.Add(color); }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < perCount; i++) { topEggColors.Add(color); }
                        }
                    }
                }
            }
        }
        else
        {
            foreach (var color in tempEggColorList)
            {
                for (int i = 0; i < perCount; i++)
                {
                    topEggColors.Add(color);
                }
            }
        }
        // ... (LÝSTE DOLDURMA KODU BURADA BÝTÝYOR) ...


        // --- YENÝ ÝSTEÐE GÖRE GÜNCELLENEN KONTROL VE KARIÞTIRMA DÖNGÜSÜ ---
        int attempts = 0;
        const int maxAttempts = 10;

        // Liste geçerli olana veya maksimum deneme sayýsýna ulaþana kadar döngüyü çalýþtýr.
        while (attempts < maxAttempts)
        {
            // Yardýmcý metodu çaðýr. Eðer 'true' dönerse liste geçerlidir ve döngüden çýkýlýr.
            if (ValidateAndShuffleTopEggs(eggColors, perCount))
            {
                break;
            }

            attempts++;
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning($"Liste {maxAttempts} denemeden sonra ideal duruma getirilemedi.");
        }

        return topEggColors;
    }/// <summary>
     /// topEggColors listesini kontrol eder. Çakýþan eleman sayýsý 1'den fazlaysa listeyi karýþtýrýr.
     /// </summary>
     /// <param name="originalEggColors">Karþýlaþtýrma yapýlacak orijinal liste.</param>
     /// <param name="currentPerCount">Renk tekrar sayýsý.</param>
     /// <returns>Liste geçerliyse (çakýþma <= 1) true, karýþtýrýldýysa false döner.</returns>
    private bool ValidateAndShuffleTopEggs(List<EggColor> originalEggColors, int currentPerCount)
    {
        int matchCount = 0;
        for (int i = 0; i < topEggColors.Count; i++)
        {
            int originalIndex = i / currentPerCount;
            if (originalIndex < originalEggColors.Count && topEggColors[i] == originalEggColors[originalIndex])
            {
                matchCount++;
            }
        }

        // Eðer çakýþma sayýsý 1'den fazlaysa listeyi gruplar halinde karýþtýr.
        if (matchCount > 0)
        {
            // topEggColors listesini gruplara ayýr.
            var groups = topEggColors
                .Select((item, index) => new { Item = item, Index = index })
                .GroupBy(x => x.Index / currentPerCount)
                .ToList();

            // Gruplarýn sýrasýný rastgele karýþtýr.
            var shuffledGroups = groups.OrderBy(x => UnityEngine.Random.value).ToList();

            // Karýþtýrýlmýþ gruplardan yeni bir liste oluþtur.
            List<EggColor> newTopEggColors = new List<EggColor>();
            foreach (var group in shuffledGroups)
            {
                foreach (var item in group)
                {
                    newTopEggColors.Add(item.Item);
                }
            }

            // topEggColors listesini güncellenmiþ liste ile deðiþtir.
            topEggColors = newTopEggColors;
            return false; // Liste karýþtýrýldý, henüz geçerli deðil.
        }

        // Çakýþma sayýsý 0 veya 1 ise liste geçerlidir.
        return true; // Liste geçerli.
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
