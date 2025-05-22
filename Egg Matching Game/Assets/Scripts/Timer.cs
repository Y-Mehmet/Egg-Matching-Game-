using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public int startTime = 21;
    private int currentTime;
    private float timeSpeed;
   


    void Start()
    {
        timeSpeed = GameManager.instance.TimeSpeed;
        currentTime= startTime;
        GameManager.instance.timeChanged?.Invoke(currentTime);
        StartCoroutine(TimeRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
