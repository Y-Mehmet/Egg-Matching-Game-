using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class EggSpawner : MonoBehaviour
{
    public static EggSpawner instance;
    public GameObject EggPrefab, SlotPrefab, JokerEggPrefab, AntiJokerEggPrefab, HiddenEggPrefeb, dragonPrefab, dragon;
    public Transform EggParent, SlotParent;
    public int HiddenEggCount = 0; // Gizli yumurta sayýsý
    public int JokerEggCount = 0; // Joker yumurta sayýsý
    public int AntiJokerEggCount = 0; // Anti-joker yumurta sayýsý



    public int slotCount ;
    public int topEggCount ;
    public int topEggCountPerColor = 1;

    private bool TruePos = false;
    public List<EggColor> mixColorList = new List<EggColor>();
    public List<EggColor> topEggColor = new List<EggColor>();
    public List<GameObject> eggList = new List<GameObject>();
    public List<Queue<GameObject>> eggStackList = new List<Queue<GameObject>>();
    public List<Vector3> slotPos = new List<Vector3>();
    public List<Vector3> topEggPos = new List<Vector3>();


    private Vector3 slotStartPos, slotSecondStartPos, topSlotStartPos,topSlotSecondStartPos, topSlotOfsset= new Vector3(0,2,0);
    private float eggDistance = 2f, eggWeight = .7f, slotHalfWeight;
    Material hidenMat,jokerMat, antiJokerMat;
    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        dragon = Instantiate(dragonPrefab);
        dragon.gameObject.SetActive(false);
    }
    public void DragonSetActive()
    {
        Debug.LogWarning("dragon set actif");
        dragon.SetActive(true);
    }

    private void OnEnable()
    {
        GameManager.instance.levelChanged += SpawnEgg;
    }
    private void OnDisable()
    {
        GameManager.instance.levelChanged -= SpawnEgg;
    }
    
    public void SpawnEgg(int levelIndex)
    {
        
        mixColorList.Clear();
        SaveGameData gameData = SaveSystem.Load();
        if (!gameData.isTutorial)
        topEggColor.Clear();
        foreach (var item in eggList)
        {
            Destroy(item.gameObject);
        }
        foreach ( Transform item in SlotParent)
        {
            Destroy(item.gameObject);
        }
        GameManager.instance.slotList.Clear();
        GameManager.instance.SlotPositionList.Clear();
        eggList.Clear();
        eggStackList.Clear();
        slotPos.Clear();
        topEggPos.Clear();
        slotHalfWeight = eggWeight / 2f;
        slotCount = GameManager.instance.GetLevelData().GetSlotCount();
        topEggCountPerColor = GameManager.instance.GetLevelData().GetTopEggPerCount();
        topEggCount = GameManager.instance.GetLevelData().GetTopEggCount()/topEggCountPerColor;
        
        GameManager.instance.slotCount = slotCount;
        CalculatePositions();
    
        GetColor();
        Spawner();
        SetColor();
    }
    Vector3 CalculateStartPos(int count, float eggWeight, float slotHalfWeight)
    {
        if (count <= 5)
        {
            int halfCount = count / 2;
            float offset = (halfCount - 1) * eggWeight;
            if (count % 2 == 0)
                return new Vector3(-slotHalfWeight - offset, 0, 0);
            else
                return new Vector3(-eggWeight - offset, 0, 0);
        }
        else
        {
            if (count % 2 == 0)
            {
                int halfCount = (count / 2) / 2;
                float offset = (halfCount - 1) * eggWeight;
                return new Vector3(-eggWeight - offset, 0, 0);
            }
            else
            {
                if (count == 7)
                    return new Vector3(-eggWeight, 0, 0);
                else if (count == 9)
                    return new Vector3(-eggWeight * 1.5f, 0, 0);
            }
        }
        return Vector3.zero;
    }

    Vector3 CalculateSecondStartPos(int count, float eggWeight, float slotHalfWeight)
    {
        if (count == 7)
            return new Vector3(-slotHalfWeight - eggWeight, 0, 0);
        else if (count == 9)
            return new Vector3(-eggWeight * 2, 0, 0);
        else
        {
            int halfCount = (count / 2) / 2;
            float offset = (halfCount - 1) * eggWeight;
            return new Vector3(-eggWeight - offset, 0, 0);
        }
    }

    void CalculatePositions()
    {
        slotStartPos = CalculateStartPos(slotCount, eggWeight, slotHalfWeight);
        topSlotStartPos = CalculateStartPos(topEggCount, eggWeight, slotHalfWeight);

        if (slotCount > 5)
            slotSecondStartPos = CalculateSecondStartPos(slotCount, eggWeight, slotHalfWeight);
        if (topEggCount > 5)
            topSlotSecondStartPos = CalculateSecondStartPos(topEggCount, eggWeight, slotHalfWeight);

        slotSecondStartPos += new Vector3(0, -1, 0);
        topSlotStartPos += topSlotOfsset * 1.5f;
        topSlotSecondStartPos += topSlotOfsset;

        AddSlotPositions(slotCount, slotStartPos, slotSecondStartPos, GameManager.instance.SlotPositionList, slotPos);
        AddSlotPositions(topEggCount, topSlotStartPos, topSlotSecondStartPos, null, topEggPos);
    }

    void AddSlotPositions(int count, Vector3 start, Vector3 secondStart, List<Vector3> globalList, List<Vector3> localList)
    {
        int half = count / 2;
        for (int i = 0; i < count; i++)
        {
            Vector3 basePos = (count > 5 && i >= half) ? secondStart : start;
            int index = (count > 5 && i >= half) ? (i - half) : i;
            Vector3 pos = basePos + new Vector3(index * eggWeight, 0, 0);

            localList.Add(pos);
            if (globalList != null)
                globalList.Add(pos);
        }
    }

    private void GetMaterial()
    {
        hidenMat = Resources.Load<Material>("Materials/HiddenMat");
        jokerMat = Resources.Load<Material>("Materials/JokerMat");
        antiJokerMat = Resources.Load<Material>("Materials/AntiJokerMat");

        if (hidenMat != null)
        {
            //Debug.Log("we found mat");
        }
        else
        {
            Debug.LogError("Material bulunamadý!");
        }
    }
   
    private void Spawner()
    {
        for (int i = 0; i < slotCount; i++)
        {
            
            GameObject slot=Instantiate(SlotPrefab, slotPos[i], Quaternion.identity, SlotParent);
            slot.GetComponent<Slot>().slotIndex = i;
          
            GameManager.instance.slotList.Add(slot); 
            
            
        }
        for(int i=0; i < topEggCount; i++)
        {
            Queue<GameObject> eggStack = new Queue<GameObject>();
            for (int j=0;j<topEggCountPerColor; j++)
            {
                if (j == 0)
                    eggStack.Clear();

                Vector3 eggPoss = topEggPos[i];
                
                GameObject egg = Instantiate(EggPrefab, eggPoss, Quaternion.identity, EggParent);
                egg.GetComponent<Egg>().startTopStackIndex = i;
                


                eggStack.Enqueue(egg);
                if(j==0)
                    egg.SetActive(true);

                else if (j == topEggCountPerColor - 1)
                {
                    egg.SetActive(false);
                    eggStackList.Add(eggStack);
                }
                else 
                    egg.SetActive(false);
                eggList.Add(egg);
               
                egg.GetComponent<Egg>().startPos = eggPoss;
               
            }
        }
       
    }
    private void  GetColor()
    {
        foreach (var color in GameManager.instance.GetLevelData().topEggColors)
        {
            mixColorList.Add(color);
        }
        if(GameManager.instance.GetLevelData().mixColor == true)
        {
            mixColorList = mixColorList.OrderBy(x => Random.value).ToList(); // Listeyi rastgele sýralayarak karýþtýr
        }  
    }
   
    private void SetColor()
    {
        for(int i= 0;i < eggList.Count;i ++)
        {
            string eggcolor = mixColorList[0].ToString();
           
                eggList[i].GetComponent<Egg>().eggColor = mixColorList[0];
                eggList[i].GetComponentInChildren<Renderer>().material.color = ColorManager.instance.GetEggColor(mixColorList[0]);
                mixColorList.Remove(mixColorList[0]);

            eggList[i].name = eggList[i].GetComponent<Egg>().eggColor.ToString() + "Egg" ;
        }
        if (HiddenEggCount > 0)
        {
            for (int i = 0; i < HiddenEggCount; i++)
            {

                Renderer rend = eggList[i].transform.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                    rend.material = new Material(hidenMat);
                }
                else
                {
                    Debug.LogWarning($"Yumurta {i} için Renderer bulunamadý!");
                }
                eggList[i].GetComponent<Egg>().properties.Add(new HiddenProperty());
                eggList[i].name = "HiddenEgg";
            }
        }
        if (JokerEggCount > 0)
        {
            for (int i = HiddenEggCount; i < HiddenEggCount+JokerEggCount; i++)
            {

                Renderer rend = eggList[i].transform.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                    rend.material = new Material(jokerMat);
                }
                else
                {
                    Debug.LogWarning($"Yumurta {i} için Renderer bulunamadý!");
                }
                eggList[i].GetComponent<Egg>().properties.Add(new JokerProperty());
                eggList[i].name = "JokerEgg";
            }
        }
        if (AntiJokerEggCount > 0)
        {
            for (int i = HiddenEggCount+JokerEggCount; i < HiddenEggCount + JokerEggCount+ AntiJokerEggCount; i++)
            {

                Renderer rend = eggList[i].transform.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                    rend.material = new Material(antiJokerMat);
                }
                else
                {
                    Debug.LogWarning($"Yumurta {i} için Renderer bulunamadý!");
                }
                eggList[i].GetComponent<Egg>().properties.Add(new AntiJokerProperty());
                eggList[i].name = "AJokerEgg";
            }
        }
    }
    
}
