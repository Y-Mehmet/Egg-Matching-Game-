using UnityEngine;
using UnityEngine.Video;
[CreateAssetMenu(fileName = "DragonSO", menuName = "ScriptableObjects/DragonSO")]
public class DragonSO : ScriptableObject
{
    public int DragonIndex, dragonMissionGemValue, DragonGemAmount;
    public string DragonName;
    public EggColor color;
    public Sprite DragonSprite;
    public VideoClip videoClip;
    public int addTime = 0;



}
