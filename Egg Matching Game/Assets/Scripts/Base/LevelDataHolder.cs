using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataHolder", menuName = "Level/LevelDataHolder")]
public class LevelDataHolder : ScriptableObject
{
    public List<LevelData> levels = new List<LevelData>();
}
