using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;
using System.Linq;
using Sequence = DG.Tweening.Sequence;
using System.Drawing;
using Color = UnityEngine.Color;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject AbilityBarPanel;
    [Header("Shuffle Animation Settings")]
    [SerializeField] private float swapDuration = 0.3f; // Süreyi biraz artırmak daha iyi görünebilir (örn: 0.6s)
    [SerializeField] private float delayBetweenSwaps = 0.1f;
    [SerializeField] private Ease shuffleEase = Ease.InOutSine; // InOutSine gibi yumuşak geçişler bu animasyonda iyi durur
    [SerializeField] private float zOffsetOnSwap = -2.0f; // Geri çekilme mesafesi
    [Header("Assign Egg Animation")]
   [SerializeField] private float assignAnimationDuration = 0.6f; // Animasyonun toplam süresi
    [SerializeField] private float zOffsetOnAssign = -2.0f;     // Geri çekilme mesafesi
    [SerializeField] private Ease assignEase = Ease.InOutSine; // Animasyon yumuşaklığı

    //public List<LevelData> levelDatas = new List<LevelData>();
    public LevelDataHolder levelDataHolder; 
    
    public List<Vector3> SlotPositionList = new List<Vector3>();
    public List<Vector3> TopEggPosList = new List<Vector3>();
    private bool isShuffling = false;

    public List<GameObject> slotList= new List<GameObject>();
    public Dictionary< int, GameObject> eggSlotDic = new Dictionary<int, GameObject>();
    public Action<int, GameObject> onSlotIndexChange;
    public Action onSlotedEggCountChange;
    [HideInInspector]
    public int slotCount = 3;
    public float TimeSpeed = 1;
    public Action<int> timeChanged;
    public Action<int> trueEggCountChanged;
    public Action<int> levelChanged;
    public Action gameStart;
    public Action pauseGame;
    public Action continueGame;
    public Action gameReStart;
    private Color originalColor;
    public bool gameStarted = false;
    public bool AnyPanelisOpen = false;
    public bool IsFirstSave = true;
    public SaveGameData gameData;

    private bool isAssigningEggs = false;

    private Vector3 targetPos;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
        gameData = SaveSystem.Load(); 
        if (gameData.isFirstLaunch)
        {
            Debug.Log("Bu ilk oyun açılışı!");
            gameData.isFirstLaunch = false;
            Save();
        }
        

    }
    private void OnEnable()
    {
        gameStart += GameStart;
        

        Time.timeScale = 1;

    }
    private void OnDisable()
    {
        gameStart -= GameStart;
       
    }
    private void Start()
    {
        originalColor = Color.gray;
        originalColor.a = 0.5f;


        ReStart();
        
    }
    public void ShowOutline(List<GameObject> list)
    {
        foreach (var item in list)
        {
            GameObject child = item.transform.GetChild(0).transform.GetChild(0).gameObject;
            if (child != null)
            {
                child.SetActive(true);
                child.transform.DOKill();

                child.transform.DOShakePosition(
                    duration: 3f,
                    strength: 0.05f,
                    vibrato: 5,
                    randomness: 30f
                );
            }
            else
            {
                Debug.LogError("must be have child");
            }
        }
    }
   
    public void HideOutline(Transform obj, List<GameObject> list)
    {
        foreach (var item in list)
        {
            GameObject child = item.transform.GetChild(0).transform.GetChild(0).gameObject;
            if (child != null && item.transform!= obj.transform)
            {
                child.SetActive(false);
            }
           
        }
    }
    
    private int GetSlotCount() { return slotList.Count; }
    
    public void Save()
    {
        SaveSystem.Save(gameData);
    }
    public void ReStart()
    {
        GetLevelData().RestartLevelData();
        trueEggCountChanged?.Invoke(0);
        levelChanged?.Invoke(gameData.levelIndex);
        if(eggSlotDic.Count > 0)
        {
            eggSlotDic.Clear();
        }
    }

    
    

    
   

    public LevelData GetLevelData()
    {
       

        if (levelDataHolder == null || levelDataHolder.levels.Count == 0)
        {
            Debug.LogError("LevelDataHolder boş veya atanmamış.");
            return null;
        }

        int index = gameData.levelIndex % levelDataHolder.levels.Count; 
        return levelDataHolder.levels[index];
    }

  
    private void GameStart()
    {
       
        gameStarted = true;
        Debug.Log("Game Start");
       
        trueEggCountChanged.Invoke(0);
    }


    /// <summary>
    /// Belirtilen yumurta için bir animasyon sekansı OLUŞTURUR ve DÖNDÜRÜR.
    /// Not: Bu metod animasyonu kendisi BAŞLATMAZ.
    /// </summary>
    /// <returns>Oluşturulan DOTween Sekansı</returns>
    private Sequence AnimateEggToSlot(GameObject egg, Transform targetTransform)
    {
        // Yumurtayı başlangıç için hazırla
        egg.SetActive(true);
        egg.transform.localScale = Vector3.one;

        Vector3 startPosition = egg.transform.position;
        Vector3 finalPosition = targetTransform.position;
        float stepDuration = assignAnimationDuration / 6.0f;

        Vector3 startBackPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z + zOffsetOnAssign);
        Vector3 finalBackPosition = new Vector3(finalPosition.x, finalPosition.y, finalPosition.z + zOffsetOnAssign);

        // DOTween Sequence oluştur
        Sequence sequence = DOTween.Sequence();

        // Animasyon sırasında yumurtanın başka bir işlem tarafından etkilenmesini önlemek için
        // DOTween'in hedefleme özelliğini kullanabiliriz.
        sequence.SetTarget(egg);

        // 1. ANİMASYON (PARALEL): Ölçek Değişimi
        sequence.Insert(0,
    // İlk animasyon
    egg.transform.DOScale(1.25f, 3 * stepDuration).SetEase(Ease.OutSine)
    // İlk animasyon BİTİNCE
    .OnComplete(() => {
        // İkinci animasyonu BAŞLAT
        egg.transform.DOScale(0.75f, 2 * stepDuration).SetEase(Ease.OutSine)
        // İkinci animasyon BİTİNCE
        .OnComplete(() => {
            // Üçüncü animasyonu BAŞLAT
            egg.transform.DOScale(1f, stepDuration).SetEase(Ease.OutSine);
        });
    })
);



        // 3. ANİMASYON (SIRALI): 3 Aşamalı Hareket
        sequence.Insert(0,egg.transform.DOMove(startBackPosition, 0).SetEase(assignEase));
        sequence.Insert(0, egg.transform.DOMove(finalBackPosition, assignAnimationDuration).SetEase(assignEase));
        sequence.Insert(assignAnimationDuration, egg.transform.DOMove(finalPosition, 0).SetEase(assignEase));

        // Animasyon bittiğinde ölçeği ve rotasyonu sıfırla
        sequence.AppendCallback(() => {
            egg.transform.localScale = Vector3.one;
            egg.transform.rotation = Quaternion.identity;
        });

        // Oluşturulan sekansı döndür
        return sequence;
    }

    public void Shufle()
    {
        StartCoroutine(ShuffleCoroutine());
    }
    private IEnumerator ShuffleCoroutine()
    {
        if (isShuffling)
        {
            Debug.LogWarning("Shuffling is already in progress.");
            yield break; // Coroutine'i sonlandır
        }
        isShuffling = true; // Bayrağı en başta kaldır

        // ... (ceckedEggCount hesaplamaları buraya gelecek) ...
        LevelData currentLevel = GetLevelData();
        if (currentLevel == null)
        {
            isShuffling = false;
            yield break;
        }
        List<Transform> emtyOrWrongColorSlotTransformList = new List<Transform>();
        foreach( GameObject slot in slotList)
        {
            if(slot.TryGetComponent<Slot>(out Slot slotScript) && !eggSlotDic.ContainsKey(slotScript.slotIndex) )
            {
                emtyOrWrongColorSlotTransformList.Add(slot.transform);
            }
            else if( !currentLevel.GetTempTopEggColorList().Contains(eggSlotDic[slotScript.slotIndex].GetComponent<Egg>().eggColor))
            {
                emtyOrWrongColorSlotTransformList.Add(slot.transform);
                onSlotIndexChange?.Invoke(-1, eggSlotDic[slotScript.slotIndex]);
                yield return new WaitForSeconds(.5f);
            }
        }
        if(emtyOrWrongColorSlotTransformList.Count>0)
        {
            foreach (Transform egg in EggSpawner.instance.EggParent)
            {
                
                if (!eggSlotDic.ContainsValue(egg.gameObject) && GetLevelData().GetTempTopEggColorList().Contains(egg.GetComponent<Egg>().eggColor))
                {

                    
                    Sequence eggAnimation = AnimateEggToSlot(egg.gameObject, emtyOrWrongColorSlotTransformList[0]);

                    
                    yield return eggAnimation.WaitForCompletion();
                    eggSlotDic[emtyOrWrongColorSlotTransformList[0].GetComponent<Slot>().slotIndex] = egg.gameObject;
                    emtyOrWrongColorSlotTransformList.RemoveAt(0);
                    if (emtyOrWrongColorSlotTransformList.Count == 0)
                        break;
                }
            }
        }
        int sltCount = GetSlotCount();
        int brokenEggCount = currentLevel.GetBrokenEggCount();
        int ceckedEggCount = sltCount - brokenEggCount;
        

        // Eğer eksik yumurta varsa, ata ve bitmesini bekle
        if (eggSlotDic.Count < ceckedEggCount)
        {
            Debug.LogWarning(" assing egg start");
            AssignMissingEggs(); // Bu sefer callback'e ihtiyacımız yok, null gönderiyoruz

            // isAssigningEggs bayrağı false olana kadar bu satırda BEKLE
            yield return new WaitWhile(() => isAssigningEggs);
            Debug.LogWarning(" assing egg end");
        }
        Debug.LogWarning(" Shuffle Coroutine started.");
        // 1. Adım: Karıştırılabilecek yumurtaları bul ve listeye ekle
        List<GameObject> canShuffleEggList = new List<GameObject>();
        int i = 0;
        foreach (var item in GetLevelData().eggColors)
        {
            if (eggSlotDic.TryGetValue(i, out GameObject egg))
            {
                if( egg.TryGetComponent<Egg>(out Egg eggScript))
                {
                    // Yanlış pozisyondaki yumurtaları listeye ekle
                    if (eggScript != null && !eggScript.IsCorrect(item))
                    {
                        canShuffleEggList.Add(egg);
                    }
                }
                
            }
            i++;
        }

        if (canShuffleEggList.Count < 2)
        {
            Debug.Log("Not enough eggs to shuffle.");
            isShuffling = false; // <-- BU SATIRI EKLE
            yield break; // yield return null yerine yield break kullanmak daha temiz.
        }

        isShuffling = true;

        // 2. Adım: Animasyon için bir DOTween Sequence oluştur
        // Sequence, animasyonları arka arkaya veya aynı anda oynatmamızı sağlar
        Sequence shuffleSequence = DOTween.Sequence();
        float stepDuration = swapDuration / 3.0f;

        // Animasyon sırasında pozisyonları takip etmek için geçici bir liste oluşturalım.
        // Çünkü Transform'ların pozisyonları animasyon sırasında değişecektir.
        List<Vector3> initialPositions = canShuffleEggList.Select(egg => egg.transform.position).ToList();

        for (int n = canShuffleEggList.Count - 1; n > 0; n--)
        {
            int k = UnityEngine.Random.Range(0, n );

            GameObject eggA = canShuffleEggList[n];
            GameObject eggB = canShuffleEggList[k];

            Vector3 posA = initialPositions[n];
            Vector3 posB = initialPositions[k];

            // --- 1. Adım: Yumurtaları Z ekseninde geriye çek ---
            Vector3 backPosA = new Vector3(posA.x, posA.y, posA.z + zOffsetOnSwap);
            Vector3 backPosB = new Vector3(posB.x, posB.y, posB.z + zOffsetOnSwap);

            // Append ile ilk yumurtanın geri gitme animasyonunu başlat
            shuffleSequence.Append(eggA.transform.DOMove(backPosA, stepDuration).SetEase(shuffleEase));
            // Join ile ikinci yumurtanın da aynı anda geri gitmesini sağla
            shuffleSequence.Join(eggB.transform.DOMove(backPosB, stepDuration).SetEase(shuffleEase));


            // --- 2. Adım: Geri pozisyondayken X/Y pozisyonlarını değiştir ---
            // eggA, eggB'nin X/Y'sine ama hala geri Z pozisyonunda gidecek
            Vector3 swapTargetForA = new Vector3(posB.x, posB.y, backPosA.z);
            // eggB, eggA'nın X/Y'sine ama hala geri Z pozisyonunda gidecek
            Vector3 swapTargetForB = new Vector3(posA.x, posA.y, backPosB.z);

            shuffleSequence.Append(eggA.transform.DOMove(swapTargetForA, stepDuration).SetEase(shuffleEase));
            shuffleSequence.Join(eggB.transform.DOMove(swapTargetForB, stepDuration).SetEase(shuffleEase));


            // --- 3. Adım: Yeni yerlerinde Z ekseninde ileri giderek orijinal Z'ye dön ---
            // eggA'nın son hedefi posB'dir
            // eggB'nin son hedefi posA'dır
            shuffleSequence.Append(eggA.transform.DOMove(posB, stepDuration).SetEase(shuffleEase));
            shuffleSequence.Join(eggB.transform.DOMove(posA, stepDuration).SetEase(shuffleEase));


            // İki yumurtanın tam takas animasyonu bittikten sonra bekleme ekle
            if (delayBetweenSwaps > 0)
            {
                shuffleSequence.AppendInterval(delayBetweenSwaps);
            }

            // Mantıksal pozisyonları bir sonraki döngü için güncelle
            (initialPositions[n], initialPositions[k]) = (initialPositions[k], initialPositions[n]);
        }
        // --- DEĞİŞEN KISIM BURADA BİTİYOR ----

        // 4. Adım: Tüm animasyonlar bittiğinde yapılacaklar
        shuffleSequence.OnComplete(() => {
            Debug.Log("Shuffling complete!");
            PanelManager.Instance.HideLastPanel();
            isShuffling = false; // Bayrağı indir, böylece tekrar karıştırılabilir
            
        });

    }
    public void AssignMissingEggs()
    {
        // Zaten bir atama animasyonu çalışıyorsa, yenisini başlatma.
        if (isAssigningEggs)
        {
            Debug.LogWarning("AssignMissingEggs animasyonu zaten çalışıyor.");
            return;
        }

        isAssigningEggs = true;

        // 1. ADIM: Tüm yumurta animasyonlarını yönetecek ana bir sekans oluştur.
        Sequence masterSequence = DOTween.Sequence();

        // 2. ADIM: Bir sonraki animasyonun ne zaman başlayacağını takip etmek için bir zaman sayacı oluştur.
        float insertTime = 0.0f;
        float staggerDelay = assignAnimationDuration / 3.0f; // Her animasyon arası gecikme (1/3'lük süre)
        Transform targetSlotTransform = null;
        List<int> slotBuffer = new List<int>();
        // 3. ADIM: Her bir slot için animasyon oluştur ve ana sekansa doğru zamanda yerleştir (Insert).
        foreach (GameObject slotObj in slotList)
        {
            if (slotObj != null && slotObj.TryGetComponent<Slot>(out var slotScript))
            {
                GameObject egg = EggSpawner.instance.EggParent.GetChild(slotScript.slotIndex).gameObject;
                if (egg != null && egg.activeInHierarchy && !eggSlotDic.ContainsValue(egg))
                {
                    if (eggSlotDic.ContainsKey(slotScript.slotIndex))
                    {
                        Debug.LogWarning($"Slot {eggSlotDic.TryGetValue(slotScript.slotIndex, out GameObject eggobj) } {eggobj}için zaten bir yumurta atanmış.");
                        foreach (GameObject item in slotList)
                        {
                            if (item.TryGetComponent<Slot>(out Slot slt))
                            {
                                if (!eggSlotDic.ContainsKey(slt.slotIndex) && !slotBuffer.Contains(slt.slotIndex))
                                {
                                    targetSlotTransform = item.transform;
                                    Debug.LogWarning(" target slot index " + slt.slotIndex);
                                    break;
                                }
                            }
                        }
                    }else
                    {
                        targetSlotTransform = slotObj.transform;
                    }
                    slotBuffer.Add(targetSlotTransform.GetComponent<Slot>().slotIndex); // Slot indeksini tampon listeye ekle
                    Sequence singleEggAnimation = AnimateEggToSlot(egg, targetSlotTransform);

                    // --- ÖNEMLİ DEĞİŞİKLİKLER BURADA ---

                    // A) Lambda kapanış sorununu (closure issue) önlemek için döngü değişkenlerinin kopyasını al.
                    // Bu sayede her OnComplete callback'i, doğru slot ve yumurta bilgisine sahip olur.
                    Slot capturedSlot = targetSlotTransform.GetComponent<Slot>();
                    GameObject capturedEgg = egg;

                    // B) Sözlüğe ekleme işlemini, ana sekansın sonuna değil,
                    // HER BİR yumurtanın KENDİ animasyonu bittiği ana taşı.
                    singleEggAnimation.OnComplete(() => {
                        Debug.Log($"Egg has been assigned to slot {capturedSlot.slotIndex}. Dictionary updated.");
                        eggSlotDic[capturedSlot.slotIndex] = capturedEgg;
                        onSlotedEggCountChange?.Invoke();
                    });

                    // C) Animasyonu, ana sekansın sonuna eklemek yerine (Append),
                    // hesapladığımız `insertTime` zamanına yerleştir (Insert).
                    masterSequence.Insert(insertTime, singleEggAnimation);

                    // D) Bir sonraki animasyonun başlangıç zamanını ilerlet.
                    insertTime += staggerDelay;
                }
                else
                {
                    Debug.LogWarning($"Slot {slotScript.slotIndex} için yumurta bulunamadı.");
                }
            }
        }

        // 4. ADIM: TÜM animasyonlar bittiğinde ne olacağını belirle.
        masterSequence.OnComplete(() => {
            Debug.LogWarning("Tüm yumurtaların atanma animasyon dalgası tamamlandı.");
            isAssigningEggs = false;

        });
    }



    public void AddEggListByIndex(int slotIndex , GameObject eggObj)
    {
        onSlotedEggCountChange?.Invoke();
        int tempIndex=-1;
        foreach (KeyValuePair<int, GameObject> dic in eggSlotDic)
        {
            if (dic.Value == eggObj)
            {
                tempIndex = dic.Key;
               
                
            }
        }
        if(eggSlotDic.ContainsKey(slotIndex))
        {
            Debug.Log("egg slot index " + slotIndex + " already exist" );
        }
        if (eggSlotDic.Count==0 || !eggSlotDic.ContainsKey(slotIndex))
        {
            foreach (KeyValuePair<int,GameObject> kvp in eggSlotDic)
            {
                if(kvp.Value == eggObj)
                {
                    RemoveEggListByIndex(kvp.Key, kvp.Value);
                    break;
                }
            }
            eggSlotDic[slotIndex] = eggObj;

            onSlotIndexChange?.Invoke(slotIndex, eggObj);
        }
        else if (eggSlotDic.ContainsKey(slotIndex) && eggObj != eggSlotDic[slotIndex])
        {
            GameObject tempEgg = eggSlotDic[slotIndex];
            
           if(tempIndex!=-1)
            {
                eggSlotDic[tempIndex] = tempEgg;
                onSlotIndexChange?.Invoke(tempIndex, tempEgg);
            }else
            {

                
                onSlotIndexChange?.Invoke(-1, tempEgg);
            }
            eggSlotDic[slotIndex] = eggObj;
            onSlotIndexChange?.Invoke(slotIndex, eggObj);
        }
       
    }
    public void RemoveEggListByIndex(int slotIndex, GameObject eggObj)
    {
        foreach (KeyValuePair<int, GameObject> dic in eggSlotDic)
        {
            if (dic.Value == eggObj)
            {
                slotIndex = dic.Key;
                break;
            }
        }
        if (eggSlotDic.ContainsKey(slotIndex))
        {
            eggSlotDic.Remove(slotIndex);
           
        }
        
    }
    public void BreakSlotProses(int slotIndex)
    {
        if(eggSlotDic.TryGetValue(slotIndex, out GameObject egg))
        {
            egg.transform.position= egg.GetComponent<Egg>().startPos;
            

        }
        if(eggSlotDic.Count>0 && eggSlotDic.ContainsKey(slotIndex))
        eggSlotDic.Remove(slotIndex);
        RemoveSlotByIndex(slotIndex);
        GetLevelData().RemoveSlotByEggColor();





    }
    public void BreakEggProses(GameObject Egg)
    {
        int i = 0;
        foreach (var item in GetLevelData().eggColors)
        {
            if (eggSlotDic.TryGetValue(i, out GameObject egg))
            {
                
                if (egg==Egg)
                {
                    eggSlotDic.Remove(i);
                    
                    break;
                }
            }
            i++;
        }
        int j = 0;
        foreach (var item in GetLevelData().eggColors)
        {
            if( Egg.GetComponent<Egg>().IsCorrect(item) )
            {
                GetLevelData().RemoveEggByEggColor(Egg.GetComponent<Egg>().eggColor);
                break;
            }
            j++;
        }
    }
    public void Check()
    {
        LevelData currentLevel = GetLevelData();
        // Önce level verisinin var olup olmadığını kontrol et
        if (currentLevel == null)
        {
            Debug.LogError("CHECK YAPILAMADI: Geçerli bir level verisi bulunamadı! GameManager üzerindeki LevelDataHolder'ı kontrol edin.");
            return; // Metodu güvenli bir şekilde sonlandır
        }
        int sltCount = GetSlotCount();
        int brokenEggCount= GetLevelData().GetBrokenEggCount();
        
        int startSlotCount= GetLevelData().eggColors.Count;
        int currentEggCount = startSlotCount - brokenEggCount;
        int currentSlotCount = startSlotCount - GetLevelData().GetBrokenEggCount();


        int ceckedEggCount = currentEggCount>currentSlotCount? currentSlotCount:currentEggCount;
        
        if (eggSlotDic.Count < ceckedEggCount)
        {
            Debug.Log("Egg Slot Count : " + eggSlotDic.Count + "  Cecked Egg Count : " + ceckedEggCount+ " slot count "+sltCount);
            for (int i = 0; i < sltCount; i++)
            {
                if (!eggSlotDic.ContainsKey(i))
                {
                    Color red = Color.red;
                    red.a = 0.4f;
                    slotList[i].GetComponentInChildren<Renderer>().material.color = red;
                    slotList[i].transform.DOKill();
                    int fixedIndex = i;
                    slotList[fixedIndex].transform.DOShakePosition(
                        duration: 2f,
                        strength: 0.05f,
                        vibrato: 10,
                        randomness: 45f
                    ).OnComplete(() =>
                    {
                        slotList[fixedIndex].GetComponentInChildren<Renderer>().material.color = originalColor;
                    });
                }
                else
                {
                    slotList[i].GetComponentInChildren<Renderer>().material.color = originalColor;
                }
            }

            trueEggCountChanged.Invoke(0);
        }else
        {
            Debug.Log("Egg Slot Count : " + eggSlotDic.Count + "  Cecked Egg Count : " + ceckedEggCount + " slot count " + sltCount);
            int trueCount = 0;
            int i = 0;
            foreach (var item in currentLevel.eggColors)
            {
                if(eggSlotDic.TryGetValue(i,out GameObject egg))
                {
                    Egg eggScript = egg.GetComponent<Egg>();
                    if (eggScript != null && eggScript.IsCorrect(item))
                    {
                        trueCount++;
                    }
                }
                i++;
            }
            trueEggCountChanged.Invoke(trueCount);
            if(trueCount== ceckedEggCount)
            {

               
                gameData.IncraseLevelData();
                GetLevelData();
                ReStart();
            }
        }
        
    }
    private void RemoveSlotByIndex(int index)
    {
        foreach( var slot in slotList)
        {
            if(slot.GetComponent<Slot>().slotIndex == index)
            {
                slotList.Remove(slot);
                slot.SetActive(false);
                onSlotedEggCountChange?.Invoke();
                return;
            }
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }
    private void OnApplicationQuit()
    {
        Save();
    }



}
public enum EggColor
{
    Yellow,
    Red,
    Green,
    Blue,
    Orange,
    Purple,
    Pink,
    Cyan,
    White,
    Black,
    Random
}
public enum Tag
{
    Egg,
    Slot,
    
}
