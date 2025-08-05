using UnityEngine;

[CreateAssetMenu(fileName = "New Tooltip", menuName = "ScriptableObjects/Tooltip")]
public class Tooltip : ScriptableObject
{
    public Sprite icon;
    public string title;
    public string description;
    public bool isNewFeature; // Indicates if this tooltip is for a new feature
    public int reqLevelIndex;
   

}
