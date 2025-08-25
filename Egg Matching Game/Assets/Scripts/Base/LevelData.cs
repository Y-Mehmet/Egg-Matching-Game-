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

        // ... (BURADAK� L�STE DOLDURMA KODUNUZ DE���MEDEN AYNI KALIYOR) ...
        bool allColorsDistinct = eggColors.Distinct().Count() == eggColors.Count;
        if (!allColorsDistinct && !dualColor)
        {
            // ... (Bu blok ayn�)
            var colorCounts = eggColors.GroupBy(color => color).Select(group => new { Color = group.Key, Count = group.Count() }).ToList();
            Debug.Log("eggColors'taki renklerin say�s�:");
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
        // ... (L�STE DOLDURMA KODU BURADA B�T�YOR) ...


        // --- YEN� �STE�E G�RE G�NCELLENEN KONTROL VE KARI�TIRMA D�NG�S� ---
        int attempts = 0;
        const int maxAttempts = 10;

        // Liste ge�erli olana veya maksimum deneme say�s�na ula�ana kadar d�ng�y� �al��t�r.
        while (attempts < maxAttempts)
        {
            // Yard�mc� metodu �a��r. E�er 'true' d�nerse liste ge�erlidir ve d�ng�den ��k�l�r.
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
     /// topEggColors listesini kontrol eder. �ak��an eleman say�s� 1'den fazlaysa listeyi kar��t�r�r.
     /// </summary>
     /// <param name="originalEggColors">Kar��la�t�rma yap�lacak orijinal liste.</param>
     /// <param name="currentPerCount">Renk tekrar say�s�.</param>
     /// <returns>Liste ge�erliyse (�ak��ma <= 1) true, kar��t�r�ld�ysa false d�ner.</returns>
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

        // E�er �ak��ma say�s� 1'den fazlaysa listeyi gruplar halinde kar��t�r.
        if (matchCount > 0)
        {
            // topEggColors listesini gruplara ay�r.
            var groups = topEggColors
                .Select((item, index) => new { Item = item, Index = index })
                .GroupBy(x => x.Index / currentPerCount)
                .ToList();

            // Gruplar�n s�ras�n� rastgele kar��t�r.
            var shuffledGroups = groups.OrderBy(x => UnityEngine.Random.value).ToList();

            // Kar��t�r�lm�� gruplardan yeni bir liste olu�tur.
            List<EggColor> newTopEggColors = new List<EggColor>();
            foreach (var group in shuffledGroups)
            {
                foreach (var item in group)
                {
                    newTopEggColors.Add(item.Item);
                }
            }

            // topEggColors listesini g�ncellenmi� liste ile de�i�tir.
            topEggColors = newTopEggColors;
            return false; // Liste kar��t�r�ld�, hen�z ge�erli de�il.
        }

        // �ak��ma say�s� 0 veya 1 ise liste ge�erlidir.
        return true; // Liste ge�erli.
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
