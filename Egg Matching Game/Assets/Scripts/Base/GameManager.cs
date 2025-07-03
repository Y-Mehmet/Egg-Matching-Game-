using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;

using System.IO;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //public List<LevelData> levelDatas = new List<LevelData>();
    public LevelDataHolder levelDataHolder; 
    public List<Vector3> SlotPositionList = new List<Vector3>();
    public List<Vector3> TopEggPosList = new List<Vector3>();

    public List<int> eggSlotIndexList = new List<int>();
    public List<GameObject> slotList= new List<GameObject>();
    public Dictionary< int, GameObject> eggSlotDic = new Dictionary<int, GameObject>();
    public Action<int, GameObject> onSlotIndexChange;
    public Action onSlotedEggCountChange;
    [HideInInspector]
    public int slotCount = 3;
    public float TimeSpeed = 1;
    public Action<int> timeChanged;
    public Action<int> trueEggCountChanged;
    public Action<int> levelChanged;
    public Action gameStart;
    public Action pauseGame;
    public Action continueGame;
    public Action gameReStart;
    private Color originalColor;
    public bool gameStarted = false;
    public bool AnyPanelisOpen = false;
    public bool IsFirstSave = true;
    public SaveGameData gameData;



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
        gameData = SaveSystem.Load(); 
        if (gameData.isFirstLaunch)
        {
            Debug.Log("Bu ilk oyun açılışı!");
            gameData.isFirstLaunch = false;
            Save();
        }


    }
    private void OnEnable()
    {
        gameStart += GameStart;
        Time.timeScale = 1;

    }
    private void OnDisable()
    {
        gameStart -= GameStart;
    }
    private void Start()
    {
        originalColor = Color.gray;
        originalColor.a = 0.5f;


        levelChanged?.Invoke(gameData.levelIndex);
    }
    
    public void Save()
    {
        SaveSystem.Save(gameData);
    }
    public void ReStart()
    {
        levelChanged?.Invoke(gameData.levelIndex);
    }

    
    

    
   

    public LevelData GetLevelData()
    {
       

        if (levelDataHolder == null || levelDataHolder.levels.Count == 0)
        {
            Debug.LogError("LevelDataHolder boş veya atanmamış.");
            return null;
        }

        int index = gameData.levelIndex % levelDataHolder.levels.Count; 
        return levelDataHolder.levels[index];
    }

    //void Update()
    //{
    //    if (!gameStarted && !AnyPanelisOpen  && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    //    {
    //        gameStarted = true;
    //        Debug.Log("Game Start");
    //        gameStart?.Invoke();
    //        trueEggCountChanged.Invoke(0);
    //       // Shuffel.instance.StartShuffle(EggSpawner.instance.eggList);
    //    }
    //}
    private void GameStart()
    {
        gameStarted = true;
        Debug.Log("Game Start");
       
        trueEggCountChanged.Invoke(0);
    }
    private void PauseGame()
    {

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
            foreach (var item in GetLevelData().eggColors)
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

               
                gameData.IncraseLevelData();
                GetLevelData();
                levelChanged?.Invoke(gameData.levelIndex);
            }
        }
        
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }
    private void OnApplicationQuit()
    {
        Save();
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
