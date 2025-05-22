using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoBehaviour
{
    public TMP_Text timeText,trueEggCountText;
    
    private void OnEnable()
    {
        GameManager.instance.timeChanged += UpdateTime;
        GameManager.instance.trueEggCountChanged += UpdateTrueEggCount;
    }
    private void OnDisable()
    {
        GameManager.instance.timeChanged -= UpdateTime;
        GameManager.instance.trueEggCountChanged -= UpdateTrueEggCount;
    }
    private void UpdateTime(int time)
    {
        timeText.text = "Days to hatch " + time;
    }
    private void UpdateTrueEggCount(int count)
    {
        trueEggCountText.text = "X " + count;
    }
}
