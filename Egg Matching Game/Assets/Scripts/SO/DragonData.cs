using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DragonData", menuName = "ScriptableObjects/DragonData")]
public class DragonData : ScriptableObject
{
   public List<Material> dragonMaterial=new List<Material>();
}
