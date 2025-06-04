using UnityEngine;
using DG.Tweening;
using System.Linq;

public class EmptyZone :MonoBehaviour, ITrigger
{
    private bool isTriggered = false;
    void ITrigger.Triggered(GameObject triggerObject)
    {
        
       
               
              if(!isTriggered)
              {
                    if (triggerObject.TryGetComponent<Egg>(out Egg egg))
                    {


                            if (!EggSpawner.instance.eggStackList[egg.startTopStackIndex].Contains(triggerObject))
                            {
                                    isTriggered = true;
                                    GameManager.instance.RemoveEggListByIndex(-1, triggerObject);
                                    Sequence mySequence = DOTween.Sequence();

                                    // Ýlk Tween'i (ara noktaya hareket) Sequence'e ekle
                                    // Append metodu, önceki Tween bittikten sonra bu Tween'i baþlatýr.
                                    mySequence.Append(egg.transform.DOMove(new Vector3(egg.transform.position.x, 0, 5.5f), .1f));

                                    // Ýkinci Tween'i (asýl hedefe hareket) Sequence'e ekle
                                    mySequence.Append(egg.transform.DOMove(egg.startPos, 0.5f).SetEase(Ease.OutBack));
                                    mySequence.OnComplete(() => {
                                        if (EggSpawner.instance.eggStackList[egg.startTopStackIndex].TryPeek(out GameObject nextEgg))
                                        {
                                            nextEgg.SetActive(false);
                                        }
                                        EggSpawner.instance.eggStackList[egg.startTopStackIndex].Push(egg.gameObject);
                                        egg.gameObject.SetActive(true);
                                        isTriggered = false;
                                    });
                            }

                }
               }
  
        

    }

    void ITrigger.TriggerExit(GameObject triggerObject)
    {
       
    }
}
