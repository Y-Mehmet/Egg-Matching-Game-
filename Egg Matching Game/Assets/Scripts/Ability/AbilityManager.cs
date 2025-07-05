using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour // Singleton<T> kullanýyorsanýz ondan türetebilirsiniz
{
    public static AbilityManager Instance { get; private set; }

    [Header("Assets")]
    public GameObject hammerPrefab; // Çekiç animasyonu için prefab

    [Header("UI")]
    public Text infoText; // "Bir slot seçin" gibi bilgilendirme metni için

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

    void Update()
    {
        // Hedefleme modundaysak fare týklamasýný dinle
        if (_isTargetingMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleTargetSelection();
            }

            // Sað týk ile hedeflemeyi iptal et
            if (Input.GetMouseButtonDown(1))
            {
                CancelTargeting();
            }
        }
    }

    // UI Butonu bu metodu çaðýracak
    public void SelectAbility(AbilityData ability)
    {
        if (_isTargetingMode)
        {
            Debug.Log("Zaten bir yetenek seçili.");
            return;
        }

        _currentAbility = ability;

        if (_currentAbility.RequiresTarget)
        {
            // Hedefleme moduna geç
            _isTargetingMode = true;
            if (infoText != null) infoText.text = $"Bir '{_currentAbility.TargetTag}' seçin. (Ýptal için Sað Týk)";
            // Burada imleci (cursor) deðiþtirmek gibi görsel efektler ekleyebilirsiniz.
        }
        else
        {
            // Hedef gerekmiyorsa direkt kullan
            ExecuteAbility(null);
        }
    }

    private void HandleTargetSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Týklanan obje doðru tag'e sahip mi?
            if (hit.collider.CompareTag(_currentAbility.TargetTag))
            {
                ExecuteAbility(hit.collider.gameObject);
            }
            else
            {
                Debug.Log($"Yanlýþ hedef! Bir '{_currentAbility.TargetTag}' objesi seçmelisiniz.");
            }
        }
    }

    private void ExecuteAbility(GameObject target)
    {
        if (_currentAbility == null) return;

        // Yeteneðin türüne göre ilgili fonksiyonu çalýþtýr
        switch (_currentAbility.Type)
        {
            case AbilityType.FreezeTime:
                StartCoroutine(FreezeTimeCoroutine(_currentAbility.Duration));
                break;

            case AbilityType.BreakSlot:
            case AbilityType.BreakEgg:
                AnimateAndDestroy(target);
                break;

                // YENÝ BÝR YETENEK EKLERSENÝZ BURAYA case EKLERSÝNÝZ
        }

        // Yeteneði kullandýktan sonra hedefleme modundan çýk
        CancelTargeting();
    }

    private IEnumerator FreezeTimeCoroutine(float duration)
    {
        Debug.Log("Zaman dondu!");
        // OYUNUNUZDAKÝ ZAMAN SAYACINI BURADA DURDURUN
        // Örnek: GameManager.Instance.IsTimerPaused = true;

        yield return new WaitForSeconds(duration);

        Debug.Log("Zaman normale döndü!");
        // ZAMAN SAYACINI BURADA DEVAM ETTÝRÝN
        // Örnek: GameManager.Instance.IsTimerPaused = false;
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

    public void CancelTargeting()
    {
        _isTargetingMode = false;
        _currentAbility = null;
        if (infoText != null) infoText.text = "";
    }
}