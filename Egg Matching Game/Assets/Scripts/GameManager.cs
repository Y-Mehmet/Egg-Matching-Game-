using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<EggColor> EggColorList = new List<EggColor>();
    public List<EggColor> TopEggColorList= new List<EggColor>();
    public List<Vector3> SlotPositionList = new List<Vector3>();
    public List<Vector3> TopEggPosList = new List<Vector3>();

    public List<int> eggSlotIndexList = new List<int>();
    public List<GameObject> slotList= new List<GameObject>();
    public Dictionary< int, GameObject> eggSlotDic = new Dictionary<int, GameObject>();
    public Action<int, GameObject> onSlotIndexChange;
    public Action onSlotedEggCountChange;
    [HideInInspector]
    public int slotCount = 3;
    public float TimeSpeed = 7;
    public Action<int> timeChanged;
    public Action<int> trueEggCountChanged;
    public Action<int> levelChanged;
    public Action gameStart;
    private Color originalColor;
    private bool gameStarted = false;
    public bool AnyPanelisOpen = false;



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
        levelChanged?.Invoke(SceeneManager.instance.level);
    }
    
    

    void Update()
    {
        if (!gameStarted && !AnyPanelisOpen  && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            gameStarted = true;
            Debug.Log("Game Start");
            gameStart?.Invoke();
            trueEggCountChanged.Invoke(0);
           // Shuffel.instance.StartShuffle(EggSpawner.instance.eggList);
        }
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
    
    public void AddEggListByIndex(int slotIndex , GameObject eggObj)
    {
        onSlotedEggCountChange?.Invoke();
        int tempIndex=-1;
        foreach (KeyValuePair<int, GameObject> dic in eggSlotDic)
        {
            if (dic.Value == eggObj)
            {
                tempIndex = dic.Key;
               
                
            }
        }
        if(eggSlotDic.ContainsKey(slotIndex))
        {
            Debug.Log("egg slot index " + slotIndex + " already exist" );
        }
        if (eggSlotDic.Count==0 || !eggSlotDic.ContainsKey(slotIndex))
        {
            foreach (KeyValuePair<int,GameObject> kvp in eggSlotDic)
            {
                if(kvp.Value == eggObj)
                {
                    RemoveEggListByIndex(kvp.Key, kvp.Value);
                    break;
                }
            }
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
        
    }
    public void Check()
    {
        if (eggSlotDic.Count <= slotCount)
        {
            for (int i = 0; i < slotCount; i++)
            {
                if (!eggSlotDic.ContainsKey(i))
                {
                    Color red = Color.red;
                    red.a = 0.4f;
                    slotList[i].GetComponentInChildren<Renderer>().material.color = red;
                    slotList[i].transform.DOKill();
                    int fixedIndex = i;
                    slotList[fixedIndex].transform.DOShakePosition(
                        duration: 3f,
                        strength: 0.3f,
                        vibrato: 10,
                        randomness: 45f
                    ).OnComplete(() =>
                    {
                        slotList[fixedIndex].GetComponentInChildren<Renderer>().material.color = originalColor;
                    });
                }
                else
                {
                    slotList[i].GetComponentInChildren<Renderer>().material.color = originalColor;
                }
            }

            trueEggCountChanged.Invoke(0);
        }

        if (eggSlotDic.Count == slotCount)
        {
            int trueCount = 0;
            int i = 0;
            foreach (var item in EggColorList)
            {
                Egg eggScript = eggSlotDic[i].GetComponent<Egg>();
                if (eggScript != null && eggScript.IsCorrect(item))
                {
                    trueCount++;
                }
                i++;
            }
            trueEggCountChanged.Invoke(trueCount);
            if(trueCount== slotCount)
            {
                SceeneManager.instance.level++;
                SceeneManager.instance.LoadScene(SceeneManager.instance.level);
                levelChanged?.Invoke(SceeneManager.instance.level);
            }
        }
        
    }

}
public enum EggColor
{
    Yellow,
    Red,
    Green,
    Blue,
    Orange,
    Purple,
    Pink,
    Cyan,
    White,
    Black,
    Random
}
