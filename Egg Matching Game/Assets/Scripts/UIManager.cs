using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoBehaviour
{
    public TMP_Text timeText;
    private void OnEnable()
    {
        GameManager.instance.timeChanged += UpdateTime;
    }
    private void OnDisable()
    {
        GameManager.instance.timeChanged -= UpdateTime;
    }
    private void UpdateTime(int time)
    {
        timeText.text = "Days to hatch " + time;
    }
}
