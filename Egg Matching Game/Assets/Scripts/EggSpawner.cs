using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EggSpawner : MonoBehaviour
{
    public GameObject EggPrefab, SlotPrefab;
    public Transform EggParent, SlotParent;



    public int slotCount = 3;

    private bool TruePos = false;
    public List<EggColor> mixColorList = new List<EggColor>();
    private List<GameObject> eggList = new List<GameObject>();
    private float eggDistance = 2f; 
    void Start()
    {
        GetColor();
        Spawner();
        SetColor();
    }

   
    private void Spawner()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Vector3 eggPoss= GameManager.instance.SlotPositionList[i]+ new Vector3(0,eggDistance,0);
            Instantiate(SlotPrefab, GameManager.instance.SlotPositionList[i], Quaternion.identity, SlotParent);
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
                eggList[i].GetComponentInChildren<Renderer>().material.color = GameManager.instance.GetEggColor(mixColorList[0]);
                mixColorList.Remove(mixColorList[0]);
            
            Debug.LogWarning(eggcolor + " "+i);
        }
    }
}
