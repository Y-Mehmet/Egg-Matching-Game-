using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EggSpawner : MonoBehaviour
{
    public static EggSpawner instance;
    public GameObject EggPrefab, SlotPrefab, JokerEggPrefab, AntiJokerEggPrefab, HiddenEggPrefeb;
    public Transform EggParent, SlotParent;
    public int HiddenEggCount = 0; // Gizli yumurta sayýsý
    public int JokerEggCount = 0; // Joker yumurta sayýsý
    public int AntiJokerEggCount = 0; // Anti-joker yumurta sayýsý



    public int slotCount = 3;

    private bool TruePos = false;
    public List<EggColor> mixColorList = new List<EggColor>();
    public List<GameObject> eggList = new List<GameObject>();
    private float eggDistance = 2f;
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
        GameManager.instance.slotCount = slotCount;
        GetMaterial();
        GetColor();
        Spawner();
        SetColor();
        
    }
    private void GetMaterial()
    {
        hidenMat = Resources.Load<Material>("Materials/HiddenMat");
        jokerMat = Resources.Load<Material>("Materials/JokerMat");
        antiJokerMat = Resources.Load<Material>("Materials/AntiJokerMat");

        if (hidenMat != null)
        {
            Debug.Log("we found mat");
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
            Vector3 eggPoss= GameManager.instance.SlotPositionList[i]+ new Vector3(0,eggDistance,0);
            GameObject slot=Instantiate(SlotPrefab, GameManager.instance.SlotPositionList[i], Quaternion.identity, SlotParent);
            GameManager.instance.slotList.Add(slot); 
            GameObject egg = Instantiate(EggPrefab, eggPoss, Quaternion.identity, EggParent);
            eggList.Add(egg);
            egg.GetComponent<Egg>().startPos = eggPoss;
            
        }
       
    }
    private void  GetColor()
    {
        foreach (var color in GameManager.instance.EggColorList)
        {
            mixColorList.Add(color);
        }
        mixColorList = mixColorList.OrderBy(x => Random.value).ToList(); // Listeyi rastgele sýralayarak karýþtýr
    }
    private void SetColor()
    {
        for(int i= 0;i < eggList.Count;i ++)
        {
            string eggcolor = mixColorList[0].ToString();
           
                eggList[i].GetComponent<Egg>().eggColor = mixColorList[0];
                eggList[i].GetComponentInChildren<Renderer>().material.color = ColorManager.instance.GetEggColor(mixColorList[0]);
                mixColorList.Remove(mixColorList[0]);
            
            Debug.LogWarning(eggcolor + " "+i);
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
