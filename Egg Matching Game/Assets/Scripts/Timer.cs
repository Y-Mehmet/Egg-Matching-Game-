using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public int startTime = 21;
    private int currentTime;
    private float timeSpeed;

    private void OnEnable()
    {
        GameManager.instance.gameStart += StartTimer;
        
    }
    void OnDisable()
    {
        GameManager.instance.gameStart -= StartTimer;
    }

void StartTimer()
    {
        timeSpeed = GameManager.instance.TimeSpeed;
        currentTime= startTime;
        GameManager.instance.timeChanged?.Invoke(currentTime);
        StartCoroutine(TimeRoutine());
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
