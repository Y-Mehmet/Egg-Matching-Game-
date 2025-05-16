using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private Vector3 originalPosition;
    private Transform currentSlot;
    private Transform selectedEgg;

    void Start()
    {
        cam = Camera.main;
        currentSlot = transform.parent; // Yumurtan�n ba�l� oldu�u slot
        originalPosition = transform.position;
    }

    void Update()
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

    private bool isDragging = false;

    void TryPickEgg(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("Egg") )
            {
                offset = hit.collider.gameObject.transform.position - hit.point;
                isDragging = true;
                GameManager.instance.SetTargetPos(hit.collider.gameObject.GetComponent<Egg>().eggSlotIndex);

                Debug.Log("Egg Pos : " + GameManager.instance.GetTargetPos());
                selectedEgg = hit.collider.gameObject.transform;

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
            selectedEgg.position= new Vector3(targetPos.x, targetPos.y, selectedEgg.position.z); // sabit Y
        }
    }

    void DropEgg()
    {
        //if (!isDragging) return;
        //isDragging = false;

        //// �arpt��� yumurtay� bulmak i�in SphereCast
        //Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);
        //foreach (var col in hits)
        //{
        //    if (col.gameObject != gameObject && col.CompareTag("Egg"))
        //    {
        //        SwapWith(col.gameObject);
        //        return;
        //    }
        //}

        //// �arpan olmad�ysa geri d�n
        //transform.position = originalPosition;
    }

    void SwapWith(GameObject otherEgg)
    {
        // Slotlar
        //Transform mySlot = currentSlot;
        //Transform otherSlot = otherEgg.transform.parent;

        //// Pozisyon de�i�imi
        //transform.SetParent(otherSlot);
        //transform.position = otherSlot.position;

        //otherEgg.transform.SetParent(mySlot);
        //otherEgg.transform.position = mySlot.position;

        //// Slot bilgilerini g�ncelle
        //currentSlot = otherSlot;

        //// originalPosition da g�ncellenmeli
        //originalPosition = transform.position;
        //otherEgg.GetComponent<EggDragHandler>().UpdateSlot(mySlot);
    }

    public void UpdateSlot(Transform newSlot)
    {
        currentSlot = newSlot;
        originalPosition = newSlot.position;
    }
}
