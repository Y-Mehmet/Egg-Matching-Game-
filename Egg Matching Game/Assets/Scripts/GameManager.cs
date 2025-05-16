using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<EggColor> EggColorList = new List<EggColor>();
    public List<Vector3> SlotPositionList = new List<Vector3>();


    private Vector3 targetPos;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }

    }
    public void SetTargetPos(int eggIndex)
    {
        targetPos= SlotPositionList[eggIndex];
    }
    public Vector3 GetTargetPos()
    {
        return targetPos;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
