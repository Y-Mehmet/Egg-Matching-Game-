using UnityEngine;
using DG.Tweening;
using System.Linq;

public class EmptyZone :MonoBehaviour, ITrigger
{
    void ITrigger.Triggered(GameObject triggerObject)
    {
        
       
               
                if (triggerObject.TryGetComponent<Egg>(out Egg egg))
                {

                if (!EggSpawner.instance.eggStackList[egg.startTopStackIndex].Contains(triggerObject))
                 {
                    GameManager.instance.RemoveEggListByIndex(-1, triggerObject);
                    Sequence mySequence = DOTween.Sequence();

                    // �lk Tween'i (ara noktaya hareket) Sequence'e ekle
                    // Append metodu, �nceki Tween bittikten sonra bu Tween'i ba�lat�r.
                    mySequence.Append(egg.transform.DOMove(new Vector3(0, 0, -.5f), .1f));

                    // �kinci Tween'i (as�l hedefe hareket) Sequence'e ekle
                    mySequence.Append(egg.transform.DOMove(egg.startPos, 0.5f).SetEase(Ease.OutBack));
                    mySequence.OnComplete(() => {
                        if (EggSpawner.instance.eggStackList[egg.startTopStackIndex].TryPeek(out GameObject nextEgg))
                        {
                            nextEgg.SetActive(false);
                        }
                        EggSpawner.instance.eggStackList[egg.startTopStackIndex].Push(egg.gameObject);
                        egg.gameObject.SetActive(true);
                    });
                    }

                }
  
        

    }

    void ITrigger.TriggerExit(GameObject triggerObject)
    {
       
    }
}
