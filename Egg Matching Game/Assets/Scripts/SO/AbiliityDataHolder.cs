using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbiliityDataHolder", menuName = "AbilityData/AbiliityDataHolder")]
public class AbiliityDataHolder : ScriptableObject
{
    public List<AbilityData> abilities = new List<AbilityData>();
}
