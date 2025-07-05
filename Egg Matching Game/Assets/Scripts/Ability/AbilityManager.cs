using System;
using System.Collections;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour // Singleton<T> kullan�yorsan�z ondan t�retebilirsiniz
{
    public static AbilityManager Instance { get; private set; }

    [Header("Assets")]
    public GameObject hammerPrefab; // �eki� animasyonu i�in prefab

 

    public Action<int> frezzeTimeAction; // Zaman dondurma eylemi i�in
    
    public Action<Tag> breakSlotAction; // Slot k�rma eylemi i�in
    public Action breakEggAction; // Yumurta k�rma eylemi i�in
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
                // T�klanan obje do�ru tag'e sahip mi?
                if (hit.collider.CompareTag(tag.ToString()))
                {
                    hit.transform.gameObject.SetActive(false);
                    break;
                }
                else
                {
                    Debug.Log($"Yanl�� hedef! Bir {tag} objesi se�melisiniz.");
                }
            }
        }

    }

    

   

    private void AnimateAndDestroy(GameObject target)
    {
        if (hammerPrefab == null || target == null) return;

        // �ekici hedef objenin pozisyonunda olu�tur
        GameObject hammerInstance = Instantiate(hammerPrefab, target.transform.position, Quaternion.identity);

        // �ekicin script'ine hangi objeyi yok edece�ini s�yle
        HammerAnimator hammerAnimator = hammerInstance.GetComponent<HammerAnimator>();
        if (hammerAnimator != null)
        {
            hammerAnimator.targetToDestroy = target;
        }
        else
        {
            Debug.LogError("Hammer prefab'�nda HammerAnimator script'i bulunamad�!");
            Destroy(hammerInstance); // �ekici yok et, ��nk� i�levsiz
        }
    }

   
}