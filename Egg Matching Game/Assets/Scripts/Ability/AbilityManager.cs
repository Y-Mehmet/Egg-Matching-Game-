using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input System kullanýyorsanýz bu gerekebilir
using System.Linq;
using static UnityEngine.GraphicsBuffer;
public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }

    [Header("Assets")]
    public GameObject hammerPrefab;

    public Action<int> frezzeTimeAction;
    public Action<Tag> breakSlotAction; // Bu eylemin kendisi bir Coroutine baþlatacak
    public Action<Tag> breakEggAction;
    public Action shuffleAction;

    // Hedefleme modunda olup olmadýðýmýzý takip eden bir bayrak (flag)
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
        // breakSlotAction bir Coroutine baþlatacaðý için direkt olarak metoda baðlanmaz.
        // Bunun yerine, bir "baþlatýcý" metoda baðlanýr.
        breakSlotAction += StartBreakSlotTargeting;
        breakEggAction += StartBreakEgg;
    }

    private void OnDisable()
    {
        breakSlotAction -= StartBreakSlotTargeting;
        breakEggAction -= StartBreakEgg;
    }
    private void StartBreakEgg(Tag tag)
    {
        // Eðer zaten bir hedefleme iþlemi devam ediyorsa, yenisini baþlatma.
        if (_isTargetingMode)
        {
            Debug.LogWarning("Zaten bir hedefleme iþlemi devam ediyor.");
            return;
        }

        GameManager.instance.ShowOutline(EggSpawner.instance.eggList);
        StartCoroutine(BreakSlotCoroutine(tag));
    }

    /// <summary>
    /// Bu metot, hedefleme iþlemini baþlatan Coroutine'i tetikler.
    /// </summary>
    private void StartBreakSlotTargeting(Tag tag)
    {
        // Eðer zaten bir hedefleme iþlemi devam ediyorsa, yenisini baþlatma.
        if (_isTargetingMode)
        {
            Debug.LogWarning("Zaten bir hedefleme iþlemi devam ediyor.");
            return;
        }
        GameManager.instance.ShowOutline(GameManager.instance.slotList);
        // Hedefleme coroutine'ini baþlat.
        StartCoroutine(BreakSlotCoroutine(tag));
    }

    /// <summary>
    /// Kullanýcýdan doðru hedefe týklamasýný bekleyen Coroutine.
    /// </summary>
    private IEnumerator BreakSlotCoroutine(Tag tag)
    {
        _isTargetingMode = true; // Hedefleme modunu aç
        GameManager.instance.gameStarted = false; // Oyunu duraklat
        Debug.Log($"Hedefleme modu aktif. Lütfen bir '{tag}' objesine týklayýn.");


        // Bu döngü, kullanýcý doðru bir týklama yapana kadar devam edecek,
        // ama her frame'de sadece bir kez çalýþacak.
        while (true)
        {
            // Kullanýcý farenin sol tuþuna týkladý mý?
            // NOT: Eski Input Manager için "Input.GetMouseButtonDown(0)"
            // Yeni Input System için "Mouse.current.leftButton.wasPressedThisFrame"
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                    // Iþýn bir þeye çarptý mý?
                    // LayerMask'i doðru kullanmak önemli. Layer'ý isme göre almak yerine bitmask kullanmak daha performanslýdýr.
                    int layerMask = LayerMask.GetMask(tag.ToString()); // "Slot" layer'ýný kullanýyoruz
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                    {
                        // Çarpan obje doðru layer'da olduðu için tag kontrolü artýk gereksiz, ama güvende olmak iyidir.
                        if (hit.collider.CompareTag(tag.ToString()))
                        {
                            Debug.Log($"Baþarýlý hedef: {hit.collider.name}. Obje yok edilecek.");

                            if(tag==Tag.Slot)
                            GameManager.instance.HideOutline(hit.transform, GameManager.instance.slotList );
                            else
                            {
                                GameManager.instance.HideOutline(hit.transform, EggSpawner.instance.eggList);
                            }
                            // Hedefi yok et (veya bir animasyon oynat)
                            AnimateAndDestroy(hit.transform.gameObject, tag);
                            //hit.transform.gameObject.SetActive(false); // Veya direkt yok et

                            // Hedefleme baþarýlý oldu, döngüden çýk.
                            break;
                        }
                    }
                    else
                    {
                        // Eðer ýþýn bir þeye çarptýysa ama yanlýþ layer'daysa veya hiç çarpmadýysa
                        Debug.Log("Geçersiz týklama. Hedefe týklanmadý.");
                    }
                }
                // Kameradan fare pozisyonuna bir ýþýn gönder
               
            }

            // Bir sonraki frame'e kadar bekle. Bu satýr, oyunun donmasýný engeller.
            yield return null;
        }

        // Coroutine bitti, durumu sýfýrla.
        _isTargetingMode = false;
        GameManager.instance.gameStarted = true; // Oyunu devam ettir
        Debug.Log("Hedefleme modu kapatýldý.");
    }

    private void AnimateAndDestroy(GameObject target, Tag tag)
    {
        if (hammerPrefab == null || target == null) return;

        // Bu fonksiyonun içeriði zaten iyi, olduðu gibi kalabilir.
        GameObject hammerInstance = Instantiate(hammerPrefab, target.transform.position, Quaternion.identity);
        HammerAnimator hammerAnimator = hammerInstance.GetComponent<HammerAnimator>();
        if (hammerAnimator != null)
        {
            hammerAnimator.targetToDestroy = target;
            if (tag == Tag.Slot) // Eðer hedef bir slot ise
                hammerAnimator.OnSmashAnimationComplete();
            else if (tag == Tag.Egg) // Eðer hedef bir yumurta ise
                hammerAnimator.OnThrowBombAnimationComplated();
            
        }
        else
        {
            Debug.LogError("Hammer prefab'ýnda HammerAnimator script'i bulunamadý!");
            Destroy(hammerInstance);
            // Animasyon çalýþmazsa bile hedefi yok etmeliyiz
            target.SetActive(false);
            
        }
    }
    private void ThrowBomb(GameObject target)
    {
        if (hammerPrefab == null || target == null) return;

        // Bu fonksiyonun içeriði zaten iyi, olduðu gibi kalabilir.
        GameObject hammerInstance = Instantiate(hammerPrefab, target.transform.position, Quaternion.identity);
        HammerAnimator hammerAnimator = hammerInstance.GetComponent<HammerAnimator>();
        if (hammerAnimator != null)
        {
            hammerAnimator.targetToDestroy = target;
            hammerAnimator.OnThrowBombAnimationComplated();
        }
        else
        {
            Debug.LogError("Hammer prefab'ýnda HammerAnimator script'i bulunamadý!");
            Destroy(hammerInstance);
            // Animasyon çalýþmazsa bile hedefi yok etmeliyiz
            target.SetActive(false);
        }
    }
}