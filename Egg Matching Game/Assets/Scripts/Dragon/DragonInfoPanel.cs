
using System.Collections.Generic;
using UnityEngine;


public class DragonInfoPanel : MonoBehaviour
{
    [SerializeField] GameObject dragonSlotPrefab;
    private List<GameObject> slotList = new List<GameObject>();
    private void Start()
    {
        for (int i = 0; i < DragonManager.Instance.dragonHolder.dragonSOList.Count; i++)
        {
            slotList.Add(Instantiate(dragonSlotPrefab, transform));

        }

        UpdateUI();

    }
    private void OnEnable()
    {
        UpdateUI();

    }
    private void UpdateUI()

    {
        Debug.Log("update uý  çalýþtý");
        int currentIndex = DragonManager.Instance.GetDragonIndex();
        for (int i = 0; i < slotList.Count; i++)
        {
            if (i < currentIndex)
            {
                foreach (Transform item in slotList[i].transform)
                {
                    if (item.TryGetComponent<DragonSlot>(out DragonSlot obj))
                        item.gameObject.SetActive(true);
                    else
                        item.gameObject.SetActive(false);
                }
                

            }
            else if (i == currentIndex)
            {
                foreach (Transform item in slotList[i].transform)
                {
                    if (item.TryGetComponent<OnProgresDragonSlot>(out OnProgresDragonSlot obj))
                        item.gameObject.SetActive(true);
                    else
                        item.gameObject.SetActive(false);
                }
                slotList[i].GetComponentInChildren<OnProgresDragonSlot>().gameObject.SetActive(true);
                Debug.LogWarning("You are on progress dragon slot");
            }
            else
            {
                foreach (Transform item in slotList[i].transform)
                {
                    if (item.TryGetComponent<UnLockDragonSlot>(out UnLockDragonSlot obj))
                        item.gameObject.SetActive(true);
                    else
                        item.gameObject.SetActive(false);
                }
                
            }
        }
    }
}
