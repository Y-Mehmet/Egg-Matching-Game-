using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<EggColor> EggColorList = new List<EggColor>();
    public List<Vector3> SlotPositionList = new List<Vector3>();
    public List<int> eggSlotIndexList = new List<int>();
    public List<GameObject> slotList= new List<GameObject>();
    public Dictionary< int, GameObject> eggSlotDic = new Dictionary<int, GameObject>();
    public Action<int, GameObject> onSlotIndexChange;
    public int slotCount = 3;
    private Color originalColor;


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
    private void Start()
    {
        originalColor = Color.gray;
        originalColor.a = 0.5f;
    }

    public void SetTargetPos(int eggIndex)
    {
        if (eggIndex < 0 || eggIndex >= SlotPositionList.Count)
        {
            Debug.LogError("Invalid egg index");
            return;
        }
        targetPos = SlotPositionList[eggIndex];
    }
    public Vector3 GetTargetPos()
    {
        return targetPos;
    }
    public Color GetEggColor(EggColor color)
    {
        switch (color)
        {
            case EggColor.Red:
                return Color.red;
            case EggColor.Green:
                return Color.green;
            case EggColor.Yellow:
                return Color.yellow;
            default:
                Debug.LogError("Invalid EggColor");
                return Color.white;


        }

    }
    public void AddEggListByIndex(int slotIndex , GameObject eggObj)
    {
        Debug.Log("dic count " + eggSlotDic.Count);
        int tempIndex=-1;
        foreach (KeyValuePair<int, GameObject> dic in eggSlotDic)
        {
            if (dic.Value == eggObj)
            {
                tempIndex = dic.Key;
               
                
            }
        }
        if (eggSlotDic.Count==0 || !eggSlotDic.ContainsKey(slotIndex))
        {
            eggSlotDic[slotIndex] = eggObj;
            onSlotIndexChange?.Invoke(slotIndex, eggObj);
        }
        else if (eggSlotDic.ContainsKey(slotIndex) && eggObj != eggSlotDic[slotIndex])
        {
            GameObject tempEgg = eggSlotDic[slotIndex];
            
           if(tempIndex!=-1)
            {
                eggSlotDic[tempIndex] = tempEgg;
                onSlotIndexChange?.Invoke(tempIndex, tempEgg);
            }else
            {

                
                onSlotIndexChange?.Invoke(-1, tempEgg);
            }
            eggSlotDic[slotIndex] = eggObj;
            onSlotIndexChange?.Invoke(slotIndex, eggObj);
        }
       
    }
    public void RemoveEggListByIndex(int slotIndex, GameObject eggObj)
    {
        foreach (KeyValuePair<int, GameObject> dic in eggSlotDic)
        {
            if (dic.Value == eggObj)
            {
                slotIndex = dic.Key;
                break;
            }
        }
        if (eggSlotDic.ContainsKey(slotIndex))
        {
            eggSlotDic.Remove(slotIndex);
           
        }
        else
        {
            
        }
    }
    public void Check()
    {
        foreach (var item in eggSlotDic)
        {
            Debug.Log(item.Key + " " + item.Value.name);
        }
        if (eggSlotDic.Count<=slotCount )
        {
            for(int i=0; i< slotCount; i++)
            {
                if (!eggSlotDic.ContainsKey(i))
                {
                    slotList[i].GetComponentInChildren<Renderer>().material.color = Color.red;
                }else
                {
                    slotList[i].GetComponentInChildren<Renderer>().material.color = originalColor;
                }
            }
        }
        if(eggSlotDic.Count== slotCount)
        {
            int trueCount = 0;
            int i = 0;
            foreach (var item in EggColorList)
            {
                if (eggSlotDic[i].GetComponent<Egg>().eggColor== item)
                {
                    trueCount++;
                }
                i++;
            }
            Debug.Log("True Count: " + trueCount);
        }
    }
}
public enum EggColor
{
    Yellow,
    Red,
    Green,
}
