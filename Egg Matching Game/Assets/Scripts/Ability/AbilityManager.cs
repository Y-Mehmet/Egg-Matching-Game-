using System;
using System.Collections;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour // Singleton<T> kullanýyorsanýz ondan türetebilirsiniz
{
    public static AbilityManager Instance { get; private set; }

    [Header("Assets")]
    public GameObject hammerPrefab; // Çekiç animasyonu için prefab

 

    public Action<int> frezzeTimeAction; // Zaman dondurma eylemi için
    
    public Action<Tag> breakSlotAction; // Slot kýrma eylemi için
    public Action breakEggAction; // Yumurta kýrma eylemi için
    public Action shuffleAction;

    private AbilityData _currentAbility;
    private bool _isTargetingMode = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void OnEnable()
    {
        breakSlotAction += BreakSlotAction;

    }
    private void OnDisable()
    {
        breakSlotAction -= BreakSlotAction;
    }
    
   
    private void BreakSlotAction(Tag tag)
    {
        GameManager.instance.gameStarted = false;
       while(true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.NameToLayer(tag.ToString())))
            {
                // Týklanan obje doðru tag'e sahip mi?
                if (hit.collider.CompareTag(tag.ToString()))
                {
                    hit.transform.gameObject.SetActive(false);
                    break;
                }
                else
                {
                    Debug.Log($"Yanlýþ hedef! Bir {tag} objesi seçmelisiniz.");
                }
            }
        }

    }

    

   

    private void AnimateAndDestroy(GameObject target)
    {
        if (hammerPrefab == null || target == null) return;

        // Çekici hedef objenin pozisyonunda oluþtur
        GameObject hammerInstance = Instantiate(hammerPrefab, target.transform.position, Quaternion.identity);

        // Çekicin script'ine hangi objeyi yok edeceðini söyle
        HammerAnimator hammerAnimator = hammerInstance.GetComponent<HammerAnimator>();
        if (hammerAnimator != null)
        {
            hammerAnimator.targetToDestroy = target;
        }
        else
        {
            Debug.LogError("Hammer prefab'ýnda HammerAnimator script'i bulunamadý!");
            Destroy(hammerInstance); // Çekici yok et, çünkü iþlevsiz
        }
    }

   
}