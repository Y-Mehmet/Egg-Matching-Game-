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
        GameManager.instance.gameReStart += StopTimeCoroutine;
        AbilityManager.Instance.frezzeTimeAction += StopTimerWhitSecond; // Zaman dondurma eylemi i�in


    }
    void OnDisable()
    {
        GameManager.instance.gameStart -= StartTimer;
        GameManager.instance.pauseGame -= StopTimer;
        GameManager.instance.continueGame -= ContinueTimer;
        GameManager.instance.gameReStart -= StopTimeCoroutine;
        AbilityManager.Instance.frezzeTimeAction -= StopTimerWhitSecond; // Zaman dondurma eylemi i�in
    }

    void StartTimer()
    {
        
        timeSpeed = GameManager.instance.TimeSpeed;
        currentTime= startTime;
        GameManager.instance.timeChanged?.Invoke(currentTime);
        if(timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
        timeCoroutine = StartCoroutine(TimeRoutine());
    }
    void StopTimeCoroutine()
    {
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
        Time.timeScale = 1;
        GameManager.instance.timeChanged?.Invoke(startTime);
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
            GameManager.instance.timeChanged?.Invoke(currentTime);
        }
        Debug.Log("game over!"); 

    }
}
