using System;
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
    // ÖNCEKİ TÜM GetTopEggColorList METOTLARINI BU NİHAİ VE DOĞRU VERSİYONLA DEĞİŞTİRİN
    // ÖNCEKİ TÜM GetTopEggColorList METOTLARINI BU NİHAİ VE DOĞRU VERSİYONLA DEĞİŞTİRİN
    public List<EggColor> GetTopEggColorList()
    {
        if (GameManager.instance.gameData.isTutorial)
        {
            return topEggColors;
        }

        topEggColors.Clear();
        int perCount = this.topEggPerCount;
        if (perCount <= 0) perCount = 1;

        // DURUM A: perCount == 1 -> birebir kopya
        if (perCount == 1)
        {
            topEggColors.AddRange(eggColors);
        }
        else
        {
            // 1) Çözümdeki her rengin ihtiyacını say
            var colorCounts = new Dictionary<EggColor, int>();
            foreach (var color in eggColors)
            {
                if (!colorCounts.ContainsKey(color)) colorCounts[color] = 0;
                colorCounts[color]++;
            }

            // 2) Konsept slotları oluştur (her slot tamamen tek renkten olacak)
            List<List<EggColor>> conceptualSlots = new List<List<EggColor>>();

            foreach (var kvp in colorCounts)
            {
                EggColor color = kvp.Key;
                int needed = kvp.Value;

                // Her renk için gereken minimum slot sayısı
                int slotCount = Mathf.CeilToInt((float)needed / perCount);

                for (int s = 0; s < slotCount; s++)
                {
                    var slot = new List<EggColor>();
                    for (int i = 0; i < perCount; i++)
                        slot.Add(color); // *** TAMAMEN bu renkten doldur ***
                    conceptualSlots.Add(slot);
                }
            }

            // 3) Toplam gereken slot sayısı = eggColors.Count
            int requiredSlots = eggColors.Count;
            int currentSlots = conceptualSlots.Count;

            // 4) Eğer eksik slot varsa -> tamamen çeldirici (çözüm renkleri dışında) ile doldur
            while (currentSlots < requiredSlots)
            {
                EggColor distractor;
                do
                {
                    distractor = ColorManager.instance.GetRandomColor();
                } while (colorCounts.ContainsKey(distractor)); // çözüm renkleri olmasın

                var slot = new List<EggColor>();
                for (int i = 0; i < perCount; i++)
                    slot.Add(distractor);

                conceptualSlots.Add(slot);
                currentSlots++;
            }

            // 5) Slotları düz listeye aktar (her slot blok halinde eklenir)
            foreach (var slot in conceptualSlots)
            {
                topEggColors.AddRange(slot);
            }
        }

        // --- KARMA + KALİTE KONTROL (blokları bozmadan karıştıracak bir yardımcı bekleniyor) ---
        int attempts = 0;
        const int maxAttempts = 10;
        while (attempts < maxAttempts)
        {
            if (ValidateAndShuffleTopEggs(eggColors, perCount))
                break;
            attempts++;
        }
        if (attempts >= maxAttempts)
        {
            Debug.LogWarning($"Liste {maxAttempts} denemeden sonra ideal duruma getirilemedi.");
        }

        // Debug log
        string eggColorsString = string.Join(", ", eggColors);
        string topEggColorsString = string.Join(", ", topEggColors);
        Debug.Log($"--- GetTopEggColorList() Metodu Tamamlandı ---\n" +
                  $"<b>Girdi (eggColors):</b>   [{eggColorsString}]\n" +
                  $"<b>Çıktı (topEggColors):</b> [{topEggColorsString}]");

        return topEggColors;
    }



    private bool ValidateAndShuffleTopEggs(List<EggColor> originalEggColors, int currentPerCount)
    {
        int matchCount = 0;
        // Kalite kontrol için yumurta-slot çakışmalarını say (Bu kısım doğru çalışıyor)
        for (int i = 0; i < topEggColors.Count; i++)
        {
            int originalIndex = i / currentPerCount;
            if (originalIndex < originalEggColors.Count && topEggColors[i] == originalEggColors[originalIndex])
            {
                matchCount++;
            }
        }

        // Eğer çakışma sayısı kabul edilemez düzeydeyse listeyi yeniden düzenle
        if (matchCount > 0)
        {
            // Listeyi gruplar halinde karıştırmak için geçici bir yapı oluştur (Bu kısım doğru)
            var groups = topEggColors
                .Select((item, index) => new { Item = item, Index = index })
                .GroupBy(x => x.Index / currentPerCount)
                .ToList();

            var shuffledGroups = groups.OrderBy(x => UnityEngine.Random.value).ToList();

            // Karıştırılmış gruplardan geçici yeni bir liste oluştur (Bu kısım doğru)
            List<EggColor> newTopEggColors = new List<EggColor>();
            foreach (var group in shuffledGroups)
            {
                foreach (var item in group)
                {
                    newTopEggColors.Add(item.Item);
                }
            }

            // --- İŞTE KRİTİK DÜZELTME BURADA ---
            // Hatalı Kod: topEggColors = newTopEggColors; // Bu satır yeni bir referans atayarak hataya neden oluyordu.

            // DOĞRU KOD: Orijinal listenin içeriğini temizle ve yeni içerikle doldur.
            // Bu, referansın korunmasını ve oyunun diğer kısımlarının güncellemeyi görmesini sağlar.
            topEggColors.Clear();
            topEggColors.AddRange(newTopEggColors);

            return false; // Liste yeniden düzenlendi, bir sonraki döngüde tekrar kontrol edilecek.
        }

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
