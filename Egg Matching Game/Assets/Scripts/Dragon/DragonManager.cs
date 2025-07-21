using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DragonManager : MonoBehaviour
{
    public static DragonManager Instance { get; private set; }
    
    public int DragonIndex = 0;
    public List<int> dragonMissionGemValue = new List<int> { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    public Action<int> OnDragonIndexChange;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        OnDragonIndexChange += SetDragonIndex;

    }
    private void OnDisable()
    {
        OnDragonIndexChange -= SetDragonIndex;

    }
    private void SetDragonIndex(int index)
    {

        DragonIndex += index;

    }
    public int GetDragonIndex()
    {
        return DragonIndex;
    }
    public int GetRequipmendGemAmount()
    {
        return dragonMissionGemValue[DragonIndex];
    }
}
