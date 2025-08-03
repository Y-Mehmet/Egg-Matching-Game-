using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private int startTime = 180;
    private int currentTime;
    private float timeSpeed;
    private Coroutine timeCoroutine;
    
    private void OnEnable()
    {
        GameManager.instance.gameStart += StartTimer;
        GameManager.instance.pauseGame += StopTimer;
        GameManager.instance.continueGame += ContinueTimer;
        GameManager.instance.stopTime+= StopGameTimeCoroutine; // Oyun durdurma eylemi i�in
        GameManager.instance.gameReStart += StopTimeCoroutine;
        GameManager.instance.addSec += AddSecondAndContinue; // Zaman� art�rma eylemi i�in
        AbilityManager.Instance.frezzeTimeAction += StopTimerWhitSecond; // Zaman dondurma eylemi i�in
        GameManager.instance.continueTime += ContinueGameTimeCoroutine; // Oyun devam etme eylemi i�in


    }
    void OnDisable()
    {
        GameManager.instance.gameStart -= StartTimer;
        GameManager.instance.pauseGame -= StopTimer;
        GameManager.instance.continueGame -= ContinueTimer;
        GameManager.instance.gameReStart -= StopTimeCoroutine;
        AbilityManager.Instance.frezzeTimeAction -= StopTimerWhitSecond; // Zaman dondurma eylemi i�in
        GameManager.instance.addSec -= AddSecondAndContinue; // Zaman� art�rma eylemi i�in
        GameManager.instance.stopTime -= StopGameTimeCoroutine; // Oyun durdurma eylemi i�in
        GameManager.instance.continueTime -= ContinueGameTimeCoroutine; // Oyun devam etme eylemi i�in
    }
    void StopTimeCoroutine()
    {
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
        Time.timeScale = 1;
        GameManager.instance.timeChanged?.Invoke(ResourceManager.Instance.time);
    }

    void StartTimer()
    {
        
        timeSpeed = GameManager.instance.TimeSpeed;
        currentTime= ResourceManager.Instance.time;
        GameManager.instance.timeChanged?.Invoke(currentTime);
        if(timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
        timeCoroutine = StartCoroutine(TimeRoutine());
    }
    void StopGameTimeCoroutine()
    {
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
        Time.timeScale = 1;
        
    }
  
    void ContinueGameTimeCoroutine()
    {
        
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
        timeCoroutine = StartCoroutine(TimeRoutine());
    }
    void StopTimer()
    {
        Time.timeScale = 0;
    }
    void StopTimerWhitSecond(int delay)
    {
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
        StartCoroutine(StopTimerWhitSecondCorrotune(delay));
    }
    IEnumerator StopTimerWhitSecondCorrotune(int delay)
    {
        PanelManager.Instance.HidePanelWithPanelID(panelID: PanelID.AbilityPurchasePanel);
        yield return new WaitForSeconds(delay);
       
        timeCoroutine = StartCoroutine(TimeRoutine());
    }
    void ContinueTimer()
    {
        Time.timeScale = 1;
    }
    

    // Update is called once per frame
    
    private IEnumerator TimeRoutine()
    {
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(timeSpeed); 

            currentTime--;
            if (currentTime == 10)
                SoundManager.instance.PlaySfx(SoundType.Tiktak);
            GameManager.instance.timeChanged?.Invoke(currentTime);
        }

        GameManager.instance.gameOver?.Invoke();

    }
    public void AddSecondAndContinue(int sec)
    {
        currentTime = sec;
        timeSpeed = GameManager.instance.TimeSpeed;
        GameManager.instance.timeChanged?.Invoke(currentTime);
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
        timeCoroutine = StartCoroutine(TimeRoutine());
       


    }
}
