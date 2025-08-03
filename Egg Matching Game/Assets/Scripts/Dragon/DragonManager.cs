using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DragonManager : MonoBehaviour
{
    public static DragonManager Instance { get; private set; }
    public DragonHolder dragonHolder;

    

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
    private void Start()
    {
       
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

        ResourceManager.Instance.AddResource(ResourceType.DragonIndex, 1);
        DragonManager.Instance.dragonHolder.dragonSOList[GetDragonIndex()].DragonGemAmount = 0;
        PanelManager.Instance.ShowPanel(PanelID.DragonInfo, PanelShowBehavior.HIDE_PREVISE);

    }
    public int GetDragonIndex()
    {
        return ResourceManager.Instance.GetResourceAmount(ResourceType.DragonIndex);
    }
    public int GetMissionGemAmount()
    {
        return dragonHolder.dragonSOList[GetDragonIndex()].dragonMissionGemValue;
    }
    public void AddDragonGemAmount(int Amount)
    {
       int tempAmount= dragonHolder.dragonSOList[GetDragonIndex()].DragonGemAmount + Amount;
        if(tempAmount >= GetMissionGemAmount())
        {
            ResourceManager.Instance.SpendResource(ResourceType.Gem, GetRequiredGemAmount());
            dragonHolder.dragonSOList[GetDragonIndex()].DragonGemAmount = GetMissionGemAmount();
           
            OnDragonIndexChange?.Invoke(1);
            if(GetDragonIndex()>0)
            ResourceManager.Instance.AddResource(ResourceType.Time, dragonHolder.dragonSOList[GetDragonIndex()-1].addTime);

        }
        else
        {
            ResourceManager.Instance.SpendResource(ResourceType.Gem, Amount);
            dragonHolder.dragonSOList[GetDragonIndex()].DragonGemAmount = tempAmount;
          
        }
       
    }
    public DragonSO GetCurrentDragonSO()
    {
       
        return dragonHolder.dragonSOList[GetDragonIndex()];
        
    }
    public int GetRequiredGemAmount()
    {
       
        return GetCurrentDragonSO().dragonMissionGemValue- GetCurrentDragonSO().DragonGemAmount;
    }
}
