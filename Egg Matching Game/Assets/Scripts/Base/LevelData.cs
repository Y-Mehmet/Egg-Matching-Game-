using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/LevelData")]
public class LevelData : ScriptableObject
{
    
    public List<EggColor> eggColors;
    public List<EggColor> topEggColors;
    
    public int GetSlotCount()
    {
        return eggColors.Count;
    }
    public int GetTopEggCount()
    {
        return topEggColors.Count;
    }

    public float levelDuration = 60f;
    public int enemyCount = 5;
    public float spawnRate = 1.5f;
}
