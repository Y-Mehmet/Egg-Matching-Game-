using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DragonHolder", menuName = "Dragon/DragonHolder")]
public class DragonHolder : ScriptableObject
{
    public List<DragonSO> dragonSOList= new List<DragonSO>();
}
