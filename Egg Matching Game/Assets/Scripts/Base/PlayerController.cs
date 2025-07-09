using System.Linq;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
        
    private Vector3 offset;
    private Camera cam;
    private Vector3 originalPosition;
    private Transform currentSlot;
    private Transform selectedEgg;
    public LayerMask eggLayerMask;
    public static PlayerController instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        cam = Camera.main;
        currentSlot = transform.parent; // Yumurtanýn baðlý olduðu slot
        originalPosition = transform.position;
        
    }
    private void OnEnable()
    {
        GameManager.instance.onSlotedEggCountChange += DropEgg;
    }
    private void OnDisable()
    {
        GameManager.instance.onSlotedEggCountChange -= DropEgg;
    }

    void Update()
    {
        if (GameManager.instance.gameStarted)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                TryPickEgg(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                DragEgg(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                DropEgg();
            }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                TryPickEgg(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                DragEgg(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                DropEgg();
            }
        }
#endif
        }

    }

    public bool isDragging = false;

    void TryPickEgg(Vector2 screenPos)
    {
        
        {
            Ray ray = cam.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, eggLayerMask))
            {
                if (hit.collider.gameObject.CompareTag("Egg"))
                {
                    offset = hit.collider.gameObject.transform.position - hit.point;
                    isDragging = true;
                    //GameManager.instance.SetTargetPos(hit.collider.gameObject.GetComponent<Egg>().slotIndex);

                    //Debug.Log("Egg Pos : " + GameManager.instance.GetTargetPos());
                    selectedEgg = hit.collider.gameObject.transform;
                    int eggStackIndex = hit.collider.gameObject.GetComponent<Egg>().startTopStackIndex;
                    GameObject egg = EggSpawner.instance.eggStackList[eggStackIndex]
                                        .FirstOrDefault(e => e == hit.collider.gameObject);

                    if (egg != null)
                    {
                        EggSpawner.instance.eggStackList[eggStackIndex].Pop();
                        if (EggSpawner.instance.eggStackList[eggStackIndex].TryPeek(out GameObject nextEgg))
                        {
                            nextEgg.SetActive(true);
                        }
                    }

                    foreach (var item in EggSpawner.instance.eggStackList)
                    {
                        if (item.Contains(hit.collider.gameObject))
                        {
                            item.Pop();
                            item.Peek().SetActive(true);
                        }
                    }

                }
            }
        }
    }

    void DragEgg(Vector2 screenPos)
    {
        if (!isDragging) return;

        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPos = hit.point + offset;
            if(targetPos.y<-1.9f)
            { targetPos.y = -1.9f; }

            if(targetPos.x< -2.0f || targetPos.x > 2.0f)
            { targetPos.x = Mathf.Clamp(targetPos.x, -2.0f, 2.0f); }
            selectedEgg.position= new Vector3(targetPos.x, targetPos.y, selectedEgg.position.z); // sabit Y
        }
    }

    public void DropEgg()
    {
        if (!isDragging) return;
        isDragging = false;

        // Çarptýðý yumurtayý bulmak için SphereCast
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var col in hits)
        {
            if (col.gameObject != gameObject && col.CompareTag("Egg"))
            {
                SwapWith(col.gameObject);
                return;
            }
        }

        // Çarpan olmadýysa geri dön
        transform.position = originalPosition;
    }

    void SwapWith(GameObject otherEgg)
    {
        
    }

    public void UpdateSlot(Transform newSlot)
    {
        currentSlot = newSlot;
        originalPosition = newSlot.position;
    }
}
