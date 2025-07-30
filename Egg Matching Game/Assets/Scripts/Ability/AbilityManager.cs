using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input System kullan�yorsan�z bu gerekebilir
using System.Linq;
using static UnityEngine.GraphicsBuffer;
public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }

    [Header("Assets")]
 
    public Action<int> frezzeTimeAction;
    public Action<Tag> breakSlotAction; // Bu eylemin kendisi bir Coroutine ba�latacak
    public Action<Tag> breakDragonEggAction;
    public Action<Tag> breakEggAction;
    public Action shuffleAction;
    public Action<AbilityType> curentAbilityTypeChanged;
    public AbilityType currentAbilityType;
    public AbiliityDataHolder abilityDataHolder;
    // Hedefleme modunda olup olmad���m�z� takip eden bir bayrak (flag)
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
        // breakSlotAction bir Coroutine ba�lataca�� i�in direkt olarak metoda ba�lanmaz.
        // Bunun yerine, bir "ba�lat�c�" metoda ba�lan�r.
        breakSlotAction += StartBreakSlotTargeting;
        breakEggAction += StartBreakEgg;
        curentAbilityTypeChanged += OnAbilityTypeChanged;
        breakDragonEggAction += StartBreakDragonEgg;
    }

    private void OnDisable()
    {
        breakSlotAction -= StartBreakSlotTargeting;
        breakEggAction -= StartBreakEgg;
        curentAbilityTypeChanged -= OnAbilityTypeChanged;
        breakDragonEggAction -= StartBreakDragonEgg;
    }
    
    private void OnAbilityTypeChanged(AbilityType abilityType)
    {
        currentAbilityType=abilityType;
        PanelManager.Instance.ShowPanel(PanelID.AbilityPurchasePanel, PanelShowBehavior.SHOW_PREVISE);

    }
    private void StartBreakEgg(Tag tag)
    {
        // E�er zaten bir hedefleme i�lemi devam ediyorsa, yenisini ba�latma.
        if (_isTargetingMode)
        {
            Debug.LogWarning("Zaten bir hedefleme i�lemi devam ediyor.");
            return;
        }

        //PanelManager.Instance.ShowPanel(PanelID.AbilityPurchasePanel, PanelShowBehavior.SHOW_PREVISE);
        GameManager.instance.ShowOutline(EggSpawner.instance.eggList);
        StartCoroutine(BreakSlotCoroutine(tag));
    }
    private void StartBreakDragonEgg(Tag tag)
    {// E�er zaten bir hedefleme i�lemi devam ediyorsa, yenisini ba�latma.
        if (_isTargetingMode)
        {
            Debug.LogWarning("Zaten bir hedefleme i�lemi devam ediyor.");
            return;
        }

        //PanelManager.Instance.ShowPanel(PanelID.AbilityPurchasePanel, PanelShowBehavior.SHOW_PREVISE);
        GameManager.instance.ShowOutline(EggSpawner.instance.eggList);
        StartCoroutine(BreakDragonEggCoroutine(tag));

    }
    private IEnumerator BreakDragonEggCoroutine(Tag tag)
    {
        _isTargetingMode = true; // Hedefleme modunu a�
        GameManager.instance.gameStarted = false; // Oyunu duraklat
        Debug.Log($"Hedefleme modu aktif. L�tfen bir '{tag}' objesine t�klay�n.");


        // Bu d�ng�, kullan�c� do�ru bir t�klama yapana kadar devam edecek,
        // ama her frame'de sadece bir kez �al��acak.
        while (true)
        {
            // Kullan�c� farenin sol tu�una t�klad� m�?
            // NOT: Eski Input Manager i�in "Input.GetMouseButtonDown(0)"
            // Yeni Input System i�in "Mouse.current.leftButton.wasPressedThisFrame"
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);


                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                    // I��n bir �eye �arpt� m�?
                    // LayerMask'i do�ru kullanmak �nemli. Layer'� isme g�re almak yerine bitmask kullanmak daha performansl�d�r.
                    int layerMask = LayerMask.GetMask(tag.ToString()); // "Slot" layer'�n� kullan�yoruz
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                    {
                        // �arpan obje do�ru layer'da oldu�u i�in tag kontrol� art�k gereksiz, ama g�vende olmak iyidir.
                        if (hit.collider.CompareTag(tag.ToString()))
                        {
                            Debug.Log($"Ba�ar�l� hedef: {hit.collider.name}. Obje yok edilecek.");

                            if (tag == Tag.Slot)
                                GameManager.instance.HideOutline(hit.transform, GameManager.instance.slotList);
                            else
                            {
                                GameManager.instance.HideOutline(hit.transform, EggSpawner.instance.eggList);
                            }
                            // Hedefi yok et (veya bir animasyon oynat)
                            AnimateAndDestroy2(hit.transform.gameObject, tag);
                            PanelManager.Instance.HideLastPanel();
                            break;
                        }
                    }
                    else
                    {
                        // E�er ���n bir �eye �arpt�ysa ama yanl�� layer'daysa veya hi� �arpmad�ysa
                        Debug.Log("Ge�ersiz t�klama. Hedefe t�klanmad�.");
                    }
                }
                // Kameradan fare pozisyonuna bir ���n g�nder

            }

            // Bir sonraki frame'e kadar bekle. Bu sat�r, oyunun donmas�n� engeller.
            yield return null;
        }

        // Coroutine bitti, durumu s�f�rla.
        _isTargetingMode = false;
        GameManager.instance.gameStarted = true; // Oyunu devam ettir
        Debug.Log("Hedefleme modu kapat�ld�.");
    }

    /// <summary>
    /// Bu metot, hedefleme i�lemini ba�latan Coroutine'i tetikler.
    /// </summary>
    private void StartBreakSlotTargeting(Tag tag)
    {
        // E�er zaten bir hedefleme i�lemi devam ediyorsa, yenisini ba�latma.
        if (_isTargetingMode)
        {
            Debug.LogWarning("Zaten bir hedefleme i�lemi devam ediyor.");
            return;
        }
        GameManager.instance.ShowOutline(GameManager.instance.slotList);
        // Hedefleme coroutine'ini ba�lat.
        StartCoroutine(BreakSlotCoroutine(tag));
    }


    /// <summary>
    /// Kullan�c�dan do�ru hedefe t�klamas�n� bekleyen Coroutine.
    /// </summary>
    private IEnumerator BreakSlotCoroutine(Tag tag)
    {
        _isTargetingMode = true; // Hedefleme modunu a�
        GameManager.instance.gameStarted = false; // Oyunu duraklat
        Debug.Log($"Hedefleme modu aktif. L�tfen bir '{tag}' objesine t�klay�n.");


        // Bu d�ng�, kullan�c� do�ru bir t�klama yapana kadar devam edecek,
        // ama her frame'de sadece bir kez �al��acak.
        while (true)
        {
            // Kullan�c� farenin sol tu�una t�klad� m�?
            // NOT: Eski Input Manager i�in "Input.GetMouseButtonDown(0)"
            // Yeni Input System i�in "Mouse.current.leftButton.wasPressedThisFrame"
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                    // I��n bir �eye �arpt� m�?
                    // LayerMask'i do�ru kullanmak �nemli. Layer'� isme g�re almak yerine bitmask kullanmak daha performansl�d�r.
                    int layerMask = LayerMask.GetMask(tag.ToString()); // "Slot" layer'�n� kullan�yoruz
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                    {
                        // �arpan obje do�ru layer'da oldu�u i�in tag kontrol� art�k gereksiz, ama g�vende olmak iyidir.
                        if (hit.collider.CompareTag(tag.ToString()))
                        {
                            Debug.Log($"Ba�ar�l� hedef: {hit.collider.name}. Obje yok edilecek.");

                            if(tag==Tag.Slot)
                            GameManager.instance.HideOutline(hit.transform, GameManager.instance.slotList );
                            else
                            {
                                GameManager.instance.HideOutline(hit.transform, EggSpawner.instance.eggList);
                            }
                            // Hedefi yok et (veya bir animasyon oynat)
                            AnimateAndDestroy(hit.transform.gameObject, tag);
                            PanelManager.Instance.HideLastPanel();
                            break;
                        }
                    }
                    else
                    {
                        // E�er ���n bir �eye �arpt�ysa ama yanl�� layer'daysa veya hi� �arpmad�ysa
                        Debug.Log("Ge�ersiz t�klama. Hedefe t�klanmad�.");
                    }
                }
                // Kameradan fare pozisyonuna bir ���n g�nder
               
            }

            // Bir sonraki frame'e kadar bekle. Bu sat�r, oyunun donmas�n� engeller.
            yield return null;
        }

        // Coroutine bitti, durumu s�f�rla.
        _isTargetingMode = false;
        GameManager.instance.gameStarted = true; // Oyunu devam ettir
        Debug.Log("Hedefleme modu kapat�ld�.");
    }

    private void AnimateAndDestroy(GameObject target, Tag tag)
    {
        if ( target == null) return;
        GameObject hammerInstance = null;
        if(tag==Tag.Slot)
        {
            hammerInstance = OneObjectPool.Instance.GetObjectWhitName(ObjectName.Hammer);
            SoundManager.instance.PlaySfx(SoundType.BreakSlot);
        }else if(tag == Tag.Egg)
        {
            hammerInstance = OneObjectPool.Instance.GetObjectWhitName(ObjectName.Missile);
            SoundManager.instance.PlaySfx(SoundType.BreakEgg);
        }
       
        HammerAnimator hammerAnimator = hammerInstance.GetComponent<HammerAnimator>();
        if (hammerAnimator != null)
        {
            hammerAnimator.targetToDestroy = target;
            if (tag == Tag.Slot) // E�er hedef bir slot ise
                hammerAnimator.OnSmashAnimationComplete();
            else if (tag == Tag.Egg) // E�er hedef bir yumurta ise
                hammerAnimator.OnThrowBombAnimationComplated();
            
        }
        else
        {
            Debug.LogError("Hammer prefab'�nda HammerAnimator script'i bulunamad�!");
            Destroy(hammerInstance);
            // Animasyon �al��mazsa bile hedefi yok etmeliyiz
            target.SetActive(false);
            
        }
    }
    private void AnimateAndDestroy2(GameObject target, Tag tag)
    {
        if (target == null) return;
        GameObject hammerInstance = null;
        if (tag == Tag.Slot)
        {
            hammerInstance = OneObjectPool.Instance.GetObjectWhitName(ObjectName.Hammer);
        }
        else if (tag == Tag.Egg)
        {
            hammerInstance = OneObjectPool.Instance.GetObjectWhitName(ObjectName.Missile);
        }

        HammerAnimator hammerAnimator = hammerInstance.GetComponent<HammerAnimator>();
        if (hammerAnimator != null)
        {
            hammerAnimator.targetToDestroy = target;
            if (tag == Tag.Slot) // E�er hedef bir slot ise
                hammerAnimator.OnSmashAnimationComplete();
            else if (tag == Tag.Egg && AbilityManager.Instance.currentAbilityType== AbilityType.BreakEgg) // E�er hedef bir yumurta ise
            {
                hammerAnimator.OnThrowBombAnimationComplated();
            }else
            {
                hammerAnimator.OnThrowBombAnimationComplated2();
            }

        }
        else
        {
            Debug.LogError("Hammer prefab'�nda HammerAnimator script'i bulunamad�!");
            Destroy(hammerInstance);
            // Animasyon �al��mazsa bile hedefi yok etmeliyiz
            target.SetActive(false);

        }
    }

}
