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
        timeText.text = FormatTime(time);
    }
    private void UpdateTrueEggCount(int count)
    {
        trueEggCountText.text = "X " + count;
    }
    private void UpdateLevel(int level)
    {
        levelText.text = "Level " + level;
    }
    private string FormatTime(int totalSeconds)
    {
        // Negatif zaman� �nlemek i�in kontrol
        if (totalSeconds < 0)
        {
            totalSeconds = 0;
        }

        // Dakikay� bulmak i�in toplam saniyeyi 60'a b�l�yoruz (tamsay� b�lmesi)
        int minutes = totalSeconds / 60;

        // Kalan saniyeyi bulmak i�in mod alma (%) operat�r�n� kullan�yoruz
        int seconds = totalSeconds % 60;

        // String.Format ile metni istedi�imiz formata getiriyoruz.
        // seconds.ToString("D2") ifadesi, saniye 10'dan k���kse ba��na '0' ekler (�rn: 5 -> "05")
        return string.Format("{0}:{1}", minutes, seconds.ToString("D2"));
    }
}
