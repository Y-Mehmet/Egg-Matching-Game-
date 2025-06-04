using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Shuffel : MonoBehaviour
{
    public static Shuffel instance;
   public float duration = 5f;
   public int swapCountPerEgg = 10;
   
    public float elapsed = 0f;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StartShuffle(List<GameObject> eggList)
    {
        StartCoroutine(ShuffleCoroutine(eggList));
    }

    private IEnumerator ShuffleCoroutine(List<GameObject> eggList)
    {
        int totalSwaps = swapCountPerEgg * eggList.Count;
        float delayBetweenSwaps = duration / totalSwaps;

        List<Vector3> positions = eggList.Select(egg => egg.transform.position).ToList();

        for (int i = 0; i < totalSwaps; i++)
        {
            if(i==totalSwaps-1)
            {
                
                foreach (var egg in eggList)
                {
                    if (egg.GetComponent<Egg>().properties.Count>0 )
                    {
                        egg.GetComponent<Egg>().properties[0].SetColor(egg,EggColor.Random); 
                    }
                }
            }
            // Shuffle h�z�n� artt�rmak i�in s�reyi azalt
            float t = (float)i / totalSwaps; // 0 -> 1
            float currentDelay = Mathf.Lerp(0.3f, 0.01f, t); // ba�ta 0.5sn, sonda 0.05sn

            // Rastgele iki yumurta se�
            int indexA = Random.Range(0, eggList.Count);
            int indexB;
            do
            {
                indexB = Random.Range(0, eggList.Count);
            } while (indexA == indexB);

            GameObject eggA = eggList[indexA];
            GameObject eggB = eggList[indexB];

            Vector3 posA = eggA.transform.position;
            Vector3 posB = eggB.transform.position;

            // DOTween ile yer de�i�tir
            eggA.transform.DOMove(posB, currentDelay).SetEase(Ease.InOutSine);
            eggB.transform.DOMove(posA, currentDelay).SetEase(Ease.InOutSine);

            // Yerlerini listede de de�i�tir
            eggList[indexA] = eggB;
            eggList[indexB] = eggA;

            yield return new WaitForSeconds(currentDelay);
        }
    }
}
