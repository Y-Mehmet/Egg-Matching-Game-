using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Tooltip Data Holder", menuName = "ScriptableObjects/Tooltip Data Holder")]
public class TooltipDataHolder : ScriptableObject
{
   public List<Tooltip> tooltips;
}
