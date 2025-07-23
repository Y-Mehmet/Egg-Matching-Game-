using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DragonManager : MonoBehaviour
{
    public static DragonManager Instance { get; private set; }
    public DragonHolder dragonHolder;

    public int DragonIndex = 0;

    public Action<int> OnDragonIndexChange;
    public Action<int> OnDragonGemAmountChange;

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
        OnDragonGemAmountChange += AddDragonGemAmount;

    }
    private void OnDisable()
    {
        OnDragonIndexChange -= SetDragonIndex;
        OnDragonGemAmountChange -= AddDragonGemAmount;

    }
    private void SetDragonIndex(int index)
    {

        DragonIndex += index;
        PanelManager.Instance.ShowPanel(PanelID.DragonInfo, PanelShowBehavior.HIDE_PREVISE);

    }
    public int GetDragonIndex()
    {
        return DragonIndex;
    }
    public int GetMissionGemAmount()
    {
        return dragonHolder.dragonSOList[DragonIndex].dragonMissionGemValue;
    }
    public void AddDragonGemAmount(int Amount)
    {
       int tempAmount= dragonHolder.dragonSOList[DragonIndex].DragonGemAmount + Amount;
        if(tempAmount >= GetMissionGemAmount())
        {
           
            ResourceManager.Instance.SpendResource(ResourceType.Gem, GetRequiredGemAmount());
            OnDragonIndexChange?.Invoke(1);

        }
        else
        {
            dragonHolder.dragonSOList[DragonIndex].DragonGemAmount = tempAmount;
            ResourceManager.Instance.SpendResource(ResourceType.Gem, Amount);
        }
       
    }
    public DragonSO GetCurrentDragonSO()
    {
        return dragonHolder.dragonSOList[DragonIndex];
    }
    public int GetRequiredGemAmount()
    {
        return GetCurrentDragonSO().dragonMissionGemValue- GetCurrentDragonSO().DragonGemAmount;
    }
}
