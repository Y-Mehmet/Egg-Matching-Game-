using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class EggSpawner : MonoBehaviour
{
    public static EggSpawner instance;
    public GameObject EggPrefab, SlotPrefab, JokerEggPrefab, AntiJokerEggPrefab, HiddenEggPrefeb;
    public Transform EggParent, SlotParent;
    public int HiddenEggCount = 0; // Gizli yumurta sayýsý
    public int JokerEggCount = 0; // Joker yumurta sayýsý
    public int AntiJokerEggCount = 0; // Anti-joker yumurta sayýsý



    public int slotCount = 3;
    public int topEggCount = 3;
    public int topEggCountPerColor = 1;

    private bool TruePos = false;
    public List<EggColor> mixColorList = new List<EggColor>();
    public List<EggColor> topEggColor = new List<EggColor>();
    public List<GameObject> eggList = new List<GameObject>();
    public List<Stack<GameObject>> eggStackList = new List<Stack<GameObject>>();
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

    void Start()
    {
        slotHalfWeight = eggWeight / 2f;
        GameManager.instance.slotCount = slotCount;
        CalculatePos();
        GetMaterial();
        GetColor();
        Spawner();
        SetColor();
        
    }
    void CalculatePos()
    {

        Vector3 pos;
       
        if(slotCount<=5)
        {
            if(slotCount%2==0)
            {
                int halfSlotCount = (int)(slotCount / 2f);
                float offset = (halfSlotCount - 1) * eggWeight;
                slotStartPos = new Vector3(-slotHalfWeight - offset, 0, 0);
            }else
            {
                int halfSlotCount = (int)(slotCount / 2f);
                float offset = (halfSlotCount - 1) * eggWeight;
                slotStartPos = new Vector3(-eggWeight-offset, 0, 0);
            }
          
        }
        else
        {
            if (slotCount % 2 == 0)
            {
                int tempSlotCount = (int)(slotCount / 2);
               
                
                    int halfSlotCount = (int)(tempSlotCount / 2f);
                    float offset = (halfSlotCount - 1) *  eggWeight;
                slotStartPos = new Vector3((-eggWeight - offset), 0, 0);
                slotSecondStartPos = slotStartPos;
               


            }
            else
            {
                if(slotCount==7)
                {
                    slotStartPos = new Vector3((-eggWeight), 0, 0);
                    slotSecondStartPos = new Vector3((-slotHalfWeight - eggWeight), 0, 0);
                   
                }
                else if(slotCount == 9)
                {
                    slotStartPos = new Vector3((- eggWeight*1.5f), 0, 0);
                    slotSecondStartPos = new Vector3((-eggWeight*2), 0, 0);
                   

                }

            }
        }

        if (topEggCount <= 5)
        {
            if (topEggCount % 2 == 0)
            {
                int halfSlotCount = (int)(topEggCount / 2f);
                float offset = (halfSlotCount - 1) * eggWeight;
                topSlotStartPos = new Vector3(-slotHalfWeight - offset, 0, 0);
            }
            else
            {
                int halfSlotCount = (int)(topEggCount / 2f);
                float offset = (halfSlotCount - 1) * eggWeight;
                topSlotStartPos = new Vector3(-eggWeight - offset, 0, 0);
            }

        }
        else
        {
            if (topEggCount % 2 == 0)
            {
                int tempSlotCount = (int)(topEggCount / 2);


                int halfSlotCount = (int)(tempSlotCount / 2f);
                float offset = (halfSlotCount - 1) * eggWeight;
                topSlotStartPos = new Vector3((-eggWeight - offset), 0, 0);
                topSlotSecondStartPos = topSlotStartPos;


            }
            else
            {
                if (topEggCount == 7)
                {
                    topSlotStartPos = new Vector3((-eggWeight), 0, 0);
                    topSlotSecondStartPos = new Vector3((-slotHalfWeight - eggWeight), 0, 0);

                }
                else if (topEggCount == 9)
                {
                    topSlotStartPos = new Vector3((-eggWeight * 1.5f), 0, 0);
                    topSlotSecondStartPos = new Vector3((-eggWeight * 2),0, 0);


                }

            }
        }

        topSlotStartPos += topSlotOfsset*1.5f;
        topSlotSecondStartPos += topSlotOfsset;
        slotSecondStartPos += new Vector3(0, -1, 0);

        if (slotCount<=5)
        {
            for (int i = 0; i < slotCount; i++)
            {
                pos = slotStartPos + new Vector3(i * eggWeight, 0, 0);
                GameManager.instance.SlotPositionList.Add(pos);
                slotPos.Add(pos);
              
               
            }
           
        }
        else
        {
            for(int i = 0; i <(int) (slotCount/2); i++)
            {
                pos = slotStartPos + new Vector3(i * eggWeight, 0, 0);
                GameManager.instance.SlotPositionList.Add(pos);
                slotPos.Add(pos);
               

            }
            for (int i = (int)(slotCount/2); i < slotCount ; i++)
            {
                pos = slotSecondStartPos + new Vector3((i - (int)(slotCount / 2)) * eggWeight, 0, 0);
                GameManager.instance.SlotPositionList.Add(pos);
                slotPos.Add(pos);
                

            }
            
        }
            if(topEggCount<=5)
        {
            for (int i = 0; i < topEggCount; i++)
            {

                pos = topSlotStartPos + new Vector3(i * eggWeight, 0, 0);
                topEggPos.Add(pos);

            }
        }else
        {
            for (int i = 0; i < (int)(topEggCount / 2); i++)
            {

                pos = topSlotStartPos + new Vector3(i * eggWeight, 0, 0);
                topEggPos.Add(pos);

            }
            for (int i = (int)(topEggCount / 2); i < topEggCount ; i++)
            {

                pos = topSlotSecondStartPos + new Vector3((i - (int)(topEggCount / 2) )* eggWeight, 0, 0);
                topEggPos.Add(pos);

            }
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
            GameManager.instance.slotList.Add(slot); 
            
            
        }
        for(int i=0; i < topEggCount; i++)
        {
            Stack<GameObject> eggStack = new Stack<GameObject>();
            for (int j=0;j<topEggCountPerColor; j++)
            {
                if (j == 0)
                    eggStack.Clear();

                Vector3 eggPoss = topEggPos[i];
                
                GameObject egg = Instantiate(EggPrefab, eggPoss, Quaternion.identity, EggParent);
                eggStack.Push(egg);
                if (j == topEggCountPerColor - 1)
                {
                    egg.SetActive(true);
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
        foreach (var color in GameManager.instance.EggColorList)
        {
            mixColorList.Add(color);
        }
        mixColorList = mixColorList.OrderBy(x => Random.value).ToList(); // Listeyi rastgele sýralayarak karýþtýr
        foreach (var color in GameManager.instance.TopEggColorList)
        {
            topEggColor.Add(color);
        }
        topEggColor = topEggColor.OrderBy(x => Random.value).ToList(); // Listeyi rastgele sýralayarak karýþtýr
    }
    private void SetColor()
    {
        for(int i= 0;i < eggList.Count;i ++)
        {
            string eggcolor = mixColorList[0].ToString();
           
                eggList[i].GetComponent<Egg>().eggColor = mixColorList[0];
                eggList[i].GetComponentInChildren<Renderer>().material.color = ColorManager.instance.GetEggColor(mixColorList[0]);
                mixColorList.Remove(mixColorList[0]);
            
           // Debug.LogWarning(eggcolor + " "+i);
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
