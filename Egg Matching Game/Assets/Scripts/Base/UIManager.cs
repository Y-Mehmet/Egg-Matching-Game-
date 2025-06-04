using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoBehaviour
{
    public TMP_Text timeText,trueEggCountText,levelText;
    
    private void OnEnable()
    {
        GameManager.instance.timeChanged += UpdateTime;
        GameManager.instance.trueEggCountChanged += UpdateTrueEggCount;
        GameManager.instance.levelChanged += UpdateLevel;

    }
    private void OnDisable()
    {
        GameManager.instance.timeChanged -= UpdateTime;
        GameManager.instance.trueEggCountChanged -= UpdateTrueEggCount;
        GameManager.instance.levelChanged -= UpdateLevel;
    }
    private void UpdateTime(int time)
    {
        timeText.text = "Days to hatch " + time;
    }
    private void UpdateTrueEggCount(int count)
    {
        trueEggCountText.text = "X " + count;
    }
    private void UpdateLevel(int level)
    {
        levelText.text = "Level " + level;
    }
}
