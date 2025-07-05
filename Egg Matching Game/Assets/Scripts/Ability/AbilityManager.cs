using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour // Singleton<T> kullan�yorsan�z ondan t�retebilirsiniz
{
    public static AbilityManager Instance { get; private set; }

    [Header("Assets")]
    public GameObject hammerPrefab; // �eki� animasyonu i�in prefab

    [Header("UI")]
    public Text infoText; // "Bir slot se�in" gibi bilgilendirme metni i�in

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
        // Hedefleme modundaysak fare t�klamas�n� dinle
        if (_isTargetingMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleTargetSelection();
            }

            // Sa� t�k ile hedeflemeyi iptal et
            if (Input.GetMouseButtonDown(1))
            {
                CancelTargeting();
            }
        }
    }

    // UI Butonu bu metodu �a��racak
    public void SelectAbility(AbilityData ability)
    {
        if (_isTargetingMode)
        {
            Debug.Log("Zaten bir yetenek se�ili.");
            return;
        }

        _currentAbility = ability;

        if (_currentAbility.RequiresTarget)
        {
            // Hedefleme moduna ge�
            _isTargetingMode = true;
            if (infoText != null) infoText.text = $"Bir '{_currentAbility.TargetTag}' se�in. (�ptal i�in Sa� T�k)";
            // Burada imleci (cursor) de�i�tirmek gibi g�rsel efektler ekleyebilirsiniz.
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
            // T�klanan obje do�ru tag'e sahip mi?
            if (hit.collider.CompareTag(_currentAbility.TargetTag))
            {
                ExecuteAbility(hit.collider.gameObject);
            }
            else
            {
                Debug.Log($"Yanl�� hedef! Bir '{_currentAbility.TargetTag}' objesi se�melisiniz.");
            }
        }
    }

    private void ExecuteAbility(GameObject target)
    {
        if (_currentAbility == null) return;

        // Yetene�in t�r�ne g�re ilgili fonksiyonu �al��t�r
        switch (_currentAbility.Type)
        {
            case AbilityType.FreezeTime:
                StartCoroutine(FreezeTimeCoroutine(_currentAbility.Duration));
                break;

            case AbilityType.BreakSlot:
            case AbilityType.BreakEgg:
                AnimateAndDestroy(target);
                break;

                // YEN� B�R YETENEK EKLERSEN�Z BURAYA case EKLERS�N�Z
        }

        // Yetene�i kulland�ktan sonra hedefleme modundan ��k
        CancelTargeting();
    }

    private IEnumerator FreezeTimeCoroutine(float duration)
    {
        Debug.Log("Zaman dondu!");
        // OYUNUNUZDAK� ZAMAN SAYACINI BURADA DURDURUN
        // �rnek: GameManager.Instance.IsTimerPaused = true;

        yield return new WaitForSeconds(duration);

        Debug.Log("Zaman normale d�nd�!");
        // ZAMAN SAYACINI BURADA DEVAM ETT�R�N
        // �rnek: GameManager.Instance.IsTimerPaused = false;
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

    public void CancelTargeting()
    {
        _isTargetingMode = false;
        _currentAbility = null;
        if (infoText != null) infoText.text = "";
    }
}