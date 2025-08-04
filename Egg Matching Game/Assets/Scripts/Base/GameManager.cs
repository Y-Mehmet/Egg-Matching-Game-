
using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;
using System.Linq;
using Sequence = DG.Tweening.Sequence;

using Color = UnityEngine.Color;
using System.Collections;

using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public bool isTutorialLevel = false;
    private int tutorialIndex = 0;
    private bool canCheck = true;
    
    public GameObject AbilityBarPanel;
    public bool isSelectTrueDaragonEgg;
    [Header("Shuffle Animation Settings")]
    private float swapDuration = 1.2f; // Süreyi biraz artırmak daha iyi görünebilir (örn: 0.6s)
    private float delayBetweenSwaps = 0.1f;
    private Ease shuffleEase = Ease.InOutSine; // InOutSine gibi yumuşak geçişler bu animasyonda iyi durur
    private float zOffsetOnSwap = 0.20f; // Geri çekilme mesafesi -.2
  
    private float assignAnimationDuration = 0.3f; // Animasyonun toplam süresi
     private float zOffsetOnAssign = -1.0f;     // Geri çekilme mesafesi
     private Ease assignEase = Ease.InOutSine; // Animasyon yumuşaklığı
    


    private float fadeDuration=3f; // Materyal geçiş süresi

    //public List<LevelData> levelDatas = new List<LevelData>();
    public LevelDataHolder levelDataHolder;
    public DragonData DragonDataSO;
    
    public List<Vector3> SlotPositionList = new List<Vector3>();
    
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
    public Action continueTime;
    public Action gameReStart;
    public Action<int> addSec;
    public Action gameOver;
    public Action stopTime;
    public AbilityData abilityData;
    


    private Color originalColor;
    public bool gameStarted = false;
    public bool AnyPanelisOpen = false;
    public bool IsFirstSave = true;
    public SaveGameData gameData;

    private bool isAssigningEggs = false;

    private Vector3 targetPos;
    // --- YENİ TIKLAMA MEKANİĞİ İÇİN EKLENEN DEĞİŞKENLER ---
    // Yorum Satırı: Oyuncunun tıkladığı yumurtayı geçici olarak hafızada tutar.
    public GameObject selectedEgg;

    // Yorum Satırı: Seçilen yumurtayı görsel olarak vurgulamak için kullanılacak renk.
    private Color selectedHighlightColor = new Color(0.8f, 0.9f, 1f, 0.7f);

    // Yorum Satırı: Vurgulanan objenin orijinal rengini saklar ki seçimi kaldırınca geri yükleyebilelim.
    private Color originalObjectColor;

    // Yorum Satırı: Rengini değiştirdiğimiz son objenin renderer bileşenini tutar.
    private Renderer lastSelectedRenderer;
    private Camera mainCamera;
    public int shufflePasses = 3;
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
        mainCamera = Camera.main;
        

    }
    private void OnEnable()
    {
        gameStart += GameStart;
        gameStart += StartTutorial; // Oyun başladığında tutorial'ı başlat
        gameOver += GameOver;
        


        Time.timeScale = 1;
        // Menü aktif olduğunda banner reklamı göster
        if (AdsManager.Instance != null && AdsManager.Instance.bannerAds != null)
        {
            AdsManager.Instance.bannerAds.ShowBannerAd();
        }

    }
    private void OnDisable()
    {
        gameStart -= GameStart;
        gameStart -= StartTutorial; // Oyun başladığında tutorial'ı başlatmayı durdur
        gameOver -= GameOver;
       
        // Menü kapandığında banner reklamı gizle (isteğe bağlı, oyununuzun tasarımına göre değişir)
        if (AdsManager.Instance != null && AdsManager.Instance.bannerAds != null)
        {
            AdsManager.Instance.bannerAds.HideBannerAd();
        }
    }
    private void Start()
    {
        originalColor = Color.gray;
        originalColor.a = 0.5f;


        ReStart();
      

    }
    private  void StartTutorial()
    {
        if (gameData.isTutorial && TutorialManager.Instance != null)
        {
            StartCoroutine(Tutorial());

        }
        else
        {
            Debug.LogWarning("is tootrial manager  null");
        }
    }
    IEnumerator Tutorial()
    {
       

        Vector3 screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.EggParent.GetChild(0).position);
        Vector3 offset = new Vector3(-150, 150, 0);

        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitForSeconds(2f);

        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.SlotParent.GetChild(0).position);
        TutorialManager.Instance.HandMovment(screenPoint+ offset, new Vector3(0, -10, 0));
            yield return new WaitUntil(() => eggSlotDic.ContainsKey(0));


        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.EggParent.GetChild(1).position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitForSeconds(2f);
        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.SlotParent.GetChild(1).position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitUntil(() => eggSlotDic.ContainsKey(1));

        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.EggParent.GetChild(2).position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitForSeconds(1f);
        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.SlotParent.GetChild(2).position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitUntil(() => eggSlotDic.ContainsKey(2));

         AbilityBarPanel.transform.GetChild(4).TryGetComponent<CheckBtn>(out CheckBtn checkBtn);
        Vector3 checkBtnPos= checkBtn.transform.position;
        TutorialManager.Instance.HandMovment(checkBtnPos+offset*2, new Vector3(0, -10, 0));
        yield return new WaitUntil(() => tutorialIndex == 1);
        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.EggParent.GetChild(0).position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitForSeconds(3f);
        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.EggParent.GetChild(1).position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitUntil(()=> GetTrueEggCount()>=3);
        TutorialManager.Instance.HandMovment(checkBtnPos + offset*2, new Vector3(0, -10, 0));
        yield return new WaitForSeconds(3f);
        TutorialManager.Instance.handPointer.SetActive(false);

        yield return new WaitUntil(() => isShuffling == false);
        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.dragon.transform.position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitUntil(()=>isSelectTrueDaragonEgg==true);
        TutorialManager.Instance.EndTutorial();




    }
    private void Update()
    {
        // Mobil ve Editor için dokunma/tıklama kontrolü
        if (Input.GetMouseButtonDown(0))
        {
            HandleSelection();
        }
    }  
    private void HandleSelection()
    {
        if (!gameStarted || isShuffling)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (eggSlotDic.Count > 0)
            {
                foreach (var item in eggSlotDic)
                {
                    Debug.LogWarning("index  " + item.Key + " name  " + item.Value);
                }
            }

            // Tıklanan obje bir yumurta mı?
            if (hit.collider.TryGetComponent<Egg>(out Egg clickedEgg))
            {
               
              
                if (selectedEgg == null)
                {
                    SelectObject(clickedEgg.gameObject); // İlk kez seçiliyorsa, seç ve vurgula.
                }
                // Zaten bu yumurta seçiliyse, seçimi iptal et.
                else if (selectedEgg == clickedEgg.gameObject)
                {
                    Debug.LogWarning("Deselecting egg: " + clickedEgg.gameObject.name);

                    DeselectObject();
                }
                else if (!eggSlotDic.ContainsValue(selectedEgg) && eggSlotDic.ContainsValue(clickedEgg.gameObject)) 
                {
                    Debug.LogWarning("selected out slot and clicked in slot: ");
                    AddEggListByIndex(eggSlotDic.FirstOrDefault(k => k.Value == clickedEgg.gameObject).Key, selectedEgg);
                    DeselectObject();
                }
                else if (eggSlotDic.ContainsValue(selectedEgg) && eggSlotDic.ContainsValue(clickedEgg.gameObject))
                {
                    AddEggListByIndex(eggSlotDic.FirstOrDefault(k => k.Value == clickedEgg.gameObject).Key, selectedEgg);
                    DeselectObject();
                }
                else if (eggSlotDic.ContainsValue(selectedEgg) && !eggSlotDic.ContainsValue(clickedEgg.gameObject))
                {
                    AddEggListByIndex(-1, selectedEgg);
                    DeselectObject();
                }
                else
                {
                    DeselectObject();
                }
               
                
            }
            // Tıklanan obje bir slot mu?
            else if (hit.collider.TryGetComponent<Slot>(out Slot clickedSlot))
            {
                Debug.LogWarning("slot selected");
                // Eğer bir yumurta seçiliyse, yerleştirme işlemini yap.
                if (selectedEgg != null)
                {
                    // Mevcut yerleştirme/değiştirme mekaniğinizi çağırıyoruz.
                    AddEggListByIndex(clickedSlot.slotIndex, selectedEgg);
                    PopStack(selectedEgg); // Seçilen yumurtayı yığından çıkar
                    DeselectObject(); // İşlem bitti, seçimi temizle.
                }
            }
            else if(selectedEgg!= null)
            {
                AddEggListByIndex(-1, selectedEgg);
                DeselectObject();
            }
            else
            {
                Debug.LogWarning("bos alan tıklandı");
                DeselectObject();
            }
        }
        // Hiçbir şeye tıklanmadıysa yine seçimi iptal et.
        else
        {
            DeselectObject();
        }
    }
    public void AddEggListByIndex(int slotIndex, GameObject eggObj)
    {

        int tempIndex = -1;
        foreach (KeyValuePair<int, GameObject> dic in eggSlotDic)
        {
            if (dic.Value == eggObj)
            {
                tempIndex = dic.Key;


            }
        }

        if (eggSlotDic.Count == 0 || !eggSlotDic.ContainsKey(slotIndex))
        {
            foreach (KeyValuePair<int, GameObject> kvp in eggSlotDic)
            {
                if (kvp.Value == eggObj)
                {
                    RemoveEggListByIndex(kvp.Key, kvp.Value);
                    break;
                }
            }
            if (slotIndex != -1)
                eggSlotDic[slotIndex] = eggObj;
            //else
            //    PushStack(eggObj);
            onSlotIndexChange?.Invoke(slotIndex, eggObj);
        }
        else if (eggSlotDic.ContainsKey(slotIndex) && eggObj != eggSlotDic[slotIndex])
        {
            GameObject tempEgg = eggSlotDic[slotIndex];

            if (tempIndex != -1)
            {

                eggSlotDic[tempIndex] = tempEgg;
                onSlotIndexChange?.Invoke(tempIndex, tempEgg);
            }
            else
            {
                Debug.LogWarning("go to start pos ");
                //eggSlotDic[tempIndex] = tempEgg;
                onSlotIndexChange?.Invoke(-1, tempEgg);
            }
            if(slotIndex!=-1)
            eggSlotDic[slotIndex] = eggObj;
           
            onSlotIndexChange?.Invoke(slotIndex, eggObj);
            Debug.LogWarning("go to slot pos ");
            //if(tempIndex==-1)
            //{
            //    PushStack(tempEgg);
            //}
        }
        //if(slotIndex==-1)
        //{
        //    PushStack(eggObj);
        //}
        


    }

    // Yorum Satırı: Bir objeyi (yumurta) seçer ve görsel olarak vurgular.
    // Eskiden: Böyle bir metod yoktu.
    // Şimdi: Seçilen objenin renderer'ını bulur, orijinal rengini kaydeder ve onu bir vurgu rengiyle değiştirir.
    private void SelectObject(GameObject obj)
    {
        if (!gameStarted || isShuffling)
            return;
            // Önceki seçimi temizle
            DeselectObject();

        selectedEgg = obj;
       
    

      
        lastSelectedRenderer = selectedEgg.GetComponentInChildren<Renderer>();
        if (lastSelectedRenderer != null)
        {
            originalObjectColor = lastSelectedRenderer.material.color; // Orijinal rengi kaydet
                                                                       // Mevcut rengin Kırmızı, Yeşil, Mavi değerlerini koruyarak,
                                                                       // sadece alfası 0.5f olan yeni bir renk oluşturup atayın.
            lastSelectedRenderer.material.color = new Color(
                lastSelectedRenderer.material.color.r,
                lastSelectedRenderer.material.color.g,
                lastSelectedRenderer.material.color.b,
                0.5f
            );
        }
    }

    // Yorum Satırı: Mevcut seçimi kaldırır ve objenin rengini eski haline getirir.
    // Eskiden: Böyle bir metod yoktu.
    // Şimdi: Seçili bir obje varsa, rengini kaydettiğimiz orijinal renge geri döndürür ve seçim değişkenlerini sıfırlar.
    private void DeselectObject()
    {
       if(selectedEgg!=null &&  selectedEgg.TryGetComponent<Egg>(out Egg egg))
        {
            
            if (lastSelectedRenderer != null)
            {
                // Orijinal rengi farklı bir yumurtaya veya materyale ait olabileceğinden,
                // her seferinde materyalin mevcut rengine değil, saklanan orijinal renge dönmek önemlidir.
                lastSelectedRenderer.material.color = originalObjectColor;
            }
            selectedEgg = null;
            lastSelectedRenderer = null;
        }
        
    }

    public void UpdateQueueSetActive()
    {
        if (EggSpawner.instance.eggStackList.Count == 0) return;
        foreach (var eggStack in EggSpawner.instance.eggStackList)
        {
            if (eggStack.Count > 0)
            {
                // İlk yumurtayı aktif yap
                GameObject topEgg = eggStack.Peek();
                topEgg.SetActive(true);
                
            }
            else
            {
                // Eğer yığın boşsa, aktif yapacak bir şey yok
                continue;
            }
        }
    }
    public void PopStack(GameObject obj)
    {
        if (EggSpawner.instance.eggStackList.Count == 0) return;
        int eggStackIndex = obj.gameObject.GetComponent<Egg>().startTopStackIndex;
        GameObject egg = EggSpawner.instance.eggStackList[eggStackIndex]
                            .FirstOrDefault(e => e == obj);

        if (egg != null)
        {
            EggSpawner.instance.eggStackList[eggStackIndex].Dequeue();
            
        }
        UpdateQueueSetActive();
    }
    public void PushStack(GameObject obj)
    {
        int eggStackIndex = obj.gameObject.GetComponent<Egg>().startTopStackIndex;
        
        EggSpawner.instance.eggStackList[eggStackIndex].Enqueue(obj);
        
        obj.SetActive(false);
        
        UpdateQueueSetActive();
    }


    private void GameOver()
    {
        ResourceManager.Instance.SpendResource(ResourceType.Energy, 1);
        PanelManager.Instance.ShowPanel(PanelID.TryAgainPanel, PanelShowBehavior.HIDE_PREVISE);

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
        if (ResourceManager.Instance.GetResourceAmount(ResourceType.Energy) <= 0)
        {
            Debug.LogWarning("Yeterli enerji yok, sahne yüklenemiyor.");
            SceeneManager.instance.LoadSceneAndEnergyPanel();
        }
        ResourceManager.Instance.AddResource(ResourceType.PlayCount, 1); // Oyun oynama sayısını artırıyoruz
        
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
        if (gameData.isTutorial)
            return levelDataHolder.levels[0];
        int halfLevel = (int)(levelDataHolder.levels.Count/2);
        
        int index = gameData.levelIndex % levelDataHolder.levels.Count;
        if (index < halfLevel && gameData.levelIndex>levelDataHolder.levels.Count)
        {
            index += halfLevel;
        }
        return levelDataHolder.levels[index];
    }

  
    private void GameStart()
    {
       
        gameStarted = true;
        Debug.Log("Game Start");
       
        trueEggCountChanged.Invoke(0);
       // AssignMissingEggs();
    }



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
        SoundManager.instance.PlaySfx(SoundType.GoToStart);
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
        SoundManager.instance.StopClip(SoundType.Tiktak);
        stopTime?.Invoke();
        List<Transform> emtyOrWrongColorSlotTransformList = new List<Transform>();
        foreach (GameObject slot in slotList)
        {
            if (slot.TryGetComponent<Slot>(out Slot slotScript) && !eggSlotDic.ContainsKey(slotScript.slotIndex))
            {
                emtyOrWrongColorSlotTransformList.Add(slot.transform);
                Debug.LogWarning("slot is null " + slotScript.slotIndex);
            }
            else if (!currentLevel.eggColors.Contains(eggSlotDic[slotScript.slotIndex].GetComponent<Egg>().eggColor))
            {
                Debug.LogWarning("slot is wrong color  " + slotScript.slotIndex + " color " + eggSlotDic[slotScript.slotIndex].name);
                emtyOrWrongColorSlotTransformList.Add(slot.transform);
                onSlotIndexChange?.Invoke(-1, eggSlotDic[slotScript.slotIndex]);
                yield return new WaitForSeconds(.5f);
            }
        }
        if (emtyOrWrongColorSlotTransformList.Count > 0)
        {
            foreach (Transform egg in EggSpawner.instance.EggParent)
            {

                if (egg.gameObject.activeInHierarchy && !eggSlotDic.ContainsValue(egg.gameObject) && GetLevelData().GetTempTopEggColorList().Contains(egg.GetComponent<Egg>().eggColor))
                {


                    Sequence eggAnimation = AnimateEggToSlot(egg.gameObject, emtyOrWrongColorSlotTransformList[0]);


                    yield return eggAnimation.WaitForCompletion();
                    eggSlotDic[emtyOrWrongColorSlotTransformList[0].GetComponent<Slot>().slotIndex] = egg.gameObject;
                    PopStack(egg.gameObject);
                    //  Debug.LogWarning("slot index " + emtyOrWrongColorSlotTransformList[0].GetComponent<Slot>().slotIndex + " egg name " + egg.name);
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
            PanelManager.Instance.HidePanelWithPanelID(panelID: PanelID.AbilityPurchasePanel);
            continueTime?.Invoke(); // Eğer zaman devam ediyorsa, devam et
            yield break; // yield return null yerine yield break kullanmak daha temiz.
        }

        isShuffling = true;

        // 2. Adım: Animasyon için bir DOTween Sequence oluştur
        // Sequence, animasyonları arka arkaya veya aynı anda oynatmamızı sağlar
        Sequence shuffleSequence = DOTween.Sequence();
        float stepDuration = swapDuration / 6.0f;

        // Animasyon sırasında pozisyonları takip etmek için geçici bir liste oluşturalım.
        // Çünkü Transform'ların pozisyonları animasyon sırasında değişecektir.
        List<Vector3> initialPositions = canShuffleEggList.Select(egg => egg.transform.position).ToList();

        for (int n = canShuffleEggList.Count - 1; n > 0; n--)
        {
            int k = UnityEngine.Random.Range(0, n);

            GameObject eggA = canShuffleEggList[n];
            GameObject eggB = canShuffleEggList[k];

            Vector3 posA = initialPositions[n];
            Vector3 posB = initialPositions[k];
            int tempIndexA= eggSlotDic.FirstOrDefault(kvp => kvp.Value == eggA).Key;
            int tempIndexB = eggSlotDic.FirstOrDefault(kvp => kvp.Value == eggB).Key;
            if (tempIndexA != -1|| tempIndexB !=-1)
            {
                eggSlotDic[tempIndexA] = eggB; // İlk yumurtayı ikinci ile değiştir
                eggSlotDic[tempIndexB] = eggA; // İkinci yumurtayı ilk ile değiştir
            }else
            {
                Debug.LogWarning("index is -1");
            }

            // --- Lambda ifadelerinde doğru değerleri kullanmak için yerel kopyalar oluştur ---
            GameObject currentEggA = eggA;
            GameObject currentEggB = eggB;
            int currentStep = (canShuffleEggList.Count - 1) - n + 1;


            // --- 1. Adım: Yumurtaları Z ekseninde geriye çek ---
            Vector3 backPosA = new Vector3(posA.x, posA.y+ zOffsetOnSwap, posA.z );
            Vector3 backPosB = new Vector3(posB.x, posB.y- zOffsetOnSwap, posB.z );

            // Append ile ilk yumurtanın geri gitme animasyonunu başlat
            shuffleSequence.Append(eggA.transform.DOMove(backPosA, stepDuration).SetEase(shuffleEase)
                // Bu Append işlemi başladığında log yazdır
                .OnStart(() => {
                    SoundManager.instance.PlaySfx(SoundType.GoToSlot);
                   
                }));

            // Join ile ikinci yumurtanın da aynı anda geri gitmesini sağla
            shuffleSequence.Join(eggB.transform.DOMove(backPosB, stepDuration).SetEase(shuffleEase));


            // --- 2. Adım: Geri pozisyondayken X/Y pozisyonlarını değiştir ---
            Vector3 swapTargetForA = new Vector3(posB.x, posB.y, backPosA.z);
            Vector3 swapTargetForB = new Vector3(posA.x, posA.y, backPosB.z);

            shuffleSequence.Append(eggA.transform.DOMove(swapTargetForA, stepDuration).SetEase(shuffleEase));

            shuffleSequence.Join(eggB.transform.DOMove(swapTargetForB, stepDuration).SetEase(shuffleEase));


            // --- 3. Adım: Yeni yerlerinde Z ekseninde ileri giderek orijinal Z'ye dön ---
            shuffleSequence.Append(eggA.transform.DOMove(posB, stepDuration).SetEase(shuffleEase));


            shuffleSequence.Join(eggB.transform.DOMove(posA, stepDuration).SetEase(shuffleEase));


            // --- İki yumurtanın takası sonrası bekleme ---
            if (delayBetweenSwaps > 0)
            {
                shuffleSequence.AppendInterval(delayBetweenSwaps);
            }

            // Mantıksal pozisyonları bir sonraki döngü için güncelle
            (initialPositions[n], initialPositions[k]) = (initialPositions[k], initialPositions[n]);
        }
       

        // 4. Adım: Tüm animasyonlar bittiğinde yapılacaklar
        shuffleSequence.OnComplete(() => {
            
            Debug.Log("Shuffling complete!");
            PanelManager.Instance.HidePanelWithPanelID(panelID: PanelID.AbilityPurchasePanel);
            continueTime?.Invoke();
            isShuffling = false; // Bayrağı indir, böylece tekrar karıştırılabilir
            
        });

    }
    private IEnumerator AssingEggs()
    {
        LevelData currentLevel = GetLevelData();
        List<Transform> emtyOrWrongColorSlotTransformList = new List<Transform>();
        foreach (GameObject slot in slotList)
        {
            if (slot.TryGetComponent<Slot>(out Slot slotScript) && !eggSlotDic.ContainsKey(slotScript.slotIndex))
            {
                emtyOrWrongColorSlotTransformList.Add(slot.transform);
                Debug.LogWarning("slot is null " + slotScript.slotIndex);
            }
           
        }
        if (emtyOrWrongColorSlotTransformList.Count > 0)
        {
            int index = 0;
            foreach (Transform egg in EggSpawner.instance.EggParent)
            {
                if (index == currentLevel.GetTopEggPerCount())
                {
                    index = 0;
                    continue;
                }else if(index!=0)
                    continue;
                else index++;

                if (egg.gameObject.activeInHierarchy && !eggSlotDic.ContainsValue(egg.gameObject) && GetLevelData().GetTempTopEggColorList().Contains(egg.GetComponent<Egg>().eggColor))
                {


                    Sequence eggAnimation = AnimateEggToSlot(egg.gameObject, emtyOrWrongColorSlotTransformList[0]);


                    yield return eggAnimation.WaitForCompletion();
                    eggSlotDic[emtyOrWrongColorSlotTransformList[0].GetComponent<Slot>().slotIndex] = egg.gameObject;
                    PopStack(egg.gameObject);
                    //  Debug.LogWarning("slot index " + emtyOrWrongColorSlotTransformList[0].GetComponent<Slot>().slotIndex + " egg name " + egg.name);
                    emtyOrWrongColorSlotTransformList.RemoveAt(0);
                    if (emtyOrWrongColorSlotTransformList.Count == 0)
                    {
                        Check();
                        break;
                    }
                       
                }
            }
        }else
            Check();
    }
    public void ShuffleRandomly()
    {
        StartCoroutine(ShuffleRandomlyCoroutine());
    }

    private IEnumerator ShuffleRandomlyCoroutine()
    {
        if (isShuffling)
        {
            Debug.LogWarning("Karıştırma zaten devam ediyor.");
            yield break;
        }
        isShuffling = true;

        List<GameObject> eggsToShuffle = new List<GameObject>(eggSlotDic.Values);

        if (eggsToShuffle.Count < 2)
        {
            Debug.Log("Karıştırılacak yeterli yumurta yok.");
            isShuffling = false;
            // Orijinal kodunuzdaki gibi, hata durumunda panel ve yetenek çağrılıyor
            PanelManager.Instance.ShowPanel(PanelID.BreakDragonEggPanel);
             abilityData.action.Execute();
            yield break;
        }
        yield return new WaitForSeconds(fadeDuration);

        Sequence mainShuffleSequence = DOTween.Sequence();
        float initialStepDuration = swapDuration / 4.0f;
        float minDuration = swapDuration / 12.0f;
        float decraseDuration = .05f;

        // <<< ANA DEĞİŞİKLİK: Tüm karıştırma mantığını birden çok kez çalıştıracak dış döngü >>>
        for (int pass = 0; pass < shufflePasses; pass++)
        {
            // Her "pas" başladığında mantık listesini ve hızı sıfırla
            List<GameObject> shuffleLogicList = new List<GameObject>(eggsToShuffle);
            float stepDuration = initialStepDuration;

            // Orijinal karıştırma döngünüz
            for (int n = shuffleLogicList.Count - 1; n > 0; n--)
            {
                int k = Random.Range(0, n + 1);
                if (n == k)
                    continue;

                GameObject eggA = shuffleLogicList[n];
                GameObject eggB = shuffleLogicList[k];

                (shuffleLogicList[n], shuffleLogicList[k]) = (shuffleLogicList[k], shuffleLogicList[n]);

                // --- TAKAS MANTIĞI VE ANİMASYONU (Orijinal haliyle korunmuştur) ---
                int tempIndexA = eggSlotDic.FirstOrDefault(kvp => kvp.Value == eggA).Key;
                int tempIndexB = eggSlotDic.FirstOrDefault(kvp => kvp.Value == eggB).Key;

                if (tempIndexA == default && eggSlotDic.All(kvp => kvp.Value != eggA) || tempIndexB == default && eggSlotDic.All(kvp => kvp.Value != eggB))
                {
                    Debug.LogWarning("Yumurtalardan biri sözlükte bulunamadı, bu takas atlanıyor.");
                    continue;
                }

                Vector3 posA = slotList.FirstOrDefault(slot => slot.GetComponent<Slot>()?.slotIndex == tempIndexA).transform.position;
                Vector3 posB = slotList.FirstOrDefault(slot => slot.GetComponent<Slot>()?.slotIndex == tempIndexB).transform.position;

                eggSlotDic[tempIndexA] = eggB;
                eggSlotDic[tempIndexB] = eggA;

                Vector3 backPosA = new Vector3(posA.x, posA.y + zOffsetOnSwap, posA.z);
                Vector3 backPosB = new Vector3(posB.x, posB.y - zOffsetOnSwap, posB.z);

                mainShuffleSequence.Append(eggA.transform.DOMove(backPosA, stepDuration).SetEase(shuffleEase)
                    // Her animasyon başladığında ses efekti çalınır
                    .OnStart(() => SoundManager.instance.PlaySfx(SoundType.GoToSlot)));
                mainShuffleSequence.Join(eggB.transform.DOMove(backPosB, stepDuration).SetEase(shuffleEase));

                Vector3 swapTargetForA = new Vector3(posB.x, posB.y, backPosA.z);
                Vector3 swapTargetForB = new Vector3(posA.x, posA.y, backPosB.z);

                mainShuffleSequence.Append(eggA.transform.DOMove(swapTargetForA, stepDuration).SetEase(shuffleEase));
                mainShuffleSequence.Join(eggB.transform.DOMove(swapTargetForB, stepDuration).SetEase(shuffleEase));

                mainShuffleSequence.Append(eggA.transform.DOMove(posB, stepDuration).SetEase(shuffleEase));
                mainShuffleSequence.Join(eggB.transform.DOMove(posA, stepDuration).SetEase(shuffleEase));

                if (delayBetweenSwaps > 0)
                {
                    mainShuffleSequence.AppendInterval(delayBetweenSwaps);
                }
                if (stepDuration >= minDuration)
                {
                    stepDuration -= decraseDuration;
                }
            }
        }

        // Tüm animasyon döngüleri bittiğinde yapılacaklar (Orijinal haliyle korunmuştur)
        mainShuffleSequence.OnComplete(() => {
            isShuffling = false;
            Debug.Log("Random Shuffle complete!");

            // Oyun akışınız için gerekli olan panel gösterme ve eylem yürütme işlemleri
            PanelManager.Instance.ShowPanel(PanelID.BreakDragonEggPanel);
             abilityData.action.Execute();
        });

        // Coroutine'in, tüm animasyon sekansı bitene kadar beklemesini sağla
        yield return mainShuffleSequence.WaitForCompletion();
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
                       // PopStack(capturedEgg);
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
    public void BreakDragonEggProses(GameObject Egg)
    {
        int i = 0;
        foreach (var item in GetLevelData().eggColors)
        {
            if (eggSlotDic.TryGetValue(i, out GameObject egg))
            {

                if (egg == Egg)
                {
                    
                    if(egg.transform.childCount>1)
                    {
                        if (EggSpawner.instance.dragon != null)
                        {
                            EggSpawner.instance.dragon.transform.SetParent(null);
                            EggSpawner.instance.dragon.SetActive(true);
                            EggSpawner.instance.dragon.transform.DOMoveZ(-1f, 2f).SetEase(Ease.InOutSine).OnComplete(() =>
                            {
                                isSelectTrueDaragonEgg = true;
                                PanelManager.Instance.ShowPanel(PanelID.LevelUpPanel);
                                Debug.LogWarning(" true egg for dragon");
                            });
                           
                        }
                        else
                            Debug.LogWarning("dragon egg is null");
                      
                    }
                    else
                    {
                        Debug.LogWarning(" wrong egg for dragon");

                        EggSpawner.instance.dragon.transform.SetParent(null);
                        EggSpawner.instance.dragon.SetActive(true);
                        EggSpawner.instance.dragon.transform.DOMoveZ(-1f, 2f).SetEase(Ease.InOutSine).OnComplete(() =>
                        {
                           isSelectTrueDaragonEgg = false;
                            PanelManager.Instance.ShowPanel(PanelID.LevelUpPanel);
                        });
                    }
                    eggSlotDic.Remove(i);
                    break;
                }
            }
            i++;
        }
        
    }
    public void Check()
    {

        if (!canCheck)
            return;
        LevelData currentLevel = GetLevelData();
        // Önce level verisinin var olup olmadığını kontrol et
        if (currentLevel == null)
        {
            Debug.LogError("CHECK YAPILAMADI: Geçerli bir level verisi bulunamadı! GameManager üzerindeki LevelDataHolder'ı kontrol edin.");
            return; // Metodu güvenli bir şekilde sonlandır
        }
        int sltCount = GetSlotCount();
        int brokenEggCount = GetLevelData().GetBrokenEggCount();

        int startSlotCount = GetLevelData().eggColors.Count;
        int currentEggCount = startSlotCount - brokenEggCount;
        int currentSlotCount = startSlotCount - GetLevelData().GetBrokenSlotCount();
        int valueableEggCount = 0;

        int ceckedEggCount = currentEggCount > currentSlotCount ? currentSlotCount : currentEggCount;
        
        if (eggSlotDic.Count< ceckedEggCount)
        {
            StartCoroutine(AssingEggs());
            return;
           
        }
        foreach (Transform eggTransform in EggSpawner.instance.EggParent)
        {
            if (eggTransform.gameObject.activeInHierarchy)
            {//eggSlotDic.ContainsValue(eggTransform.gameObject) &&
                valueableEggCount++;
            }
        }
        if (valueableEggCount < ceckedEggCount)
        {
            ceckedEggCount = valueableEggCount;
        }

        if (eggSlotDic.Count < ceckedEggCount)
        {
            //canCheck = false; 
            //Debug.Log("Egg Slot Count : " + eggSlotDic.Count + "  Cecked Egg Count : " + ceckedEggCount + " slot count " + sltCount);
            //for (int i = 0; i < sltCount; i++)
            //{
            //    if (!eggSlotDic.ContainsKey(i))
            //    {
            //        Color red = Color.red;
            //        red.a = 0.4f;
            //        slotList[i].GetComponentInChildren<Renderer>().material.color = red;
            //        slotList[i].transform.DOKill();
            //        int fixedIndex = i;
            //        slotList[fixedIndex].transform.DOShakePosition(
            //            duration: 2f,
            //            strength: 0.05f,
            //            vibrato: 10,
            //            randomness: 45f
            //        ).OnComplete(() =>
            //        {
            //            canCheck = true;
            //            slotList[fixedIndex].GetComponentInChildren<Renderer>().material.color = originalColor;
            //        });
            //    }
            //    else
            //    {
            //        slotList[i].GetComponentInChildren<Renderer>().material.color = originalColor;
            //    }
            //}

            //trueEggCountChanged.Invoke(0);
        }
        else
        {
            canCheck = true;
            int trueCount = GetTrueEggCount();

            trueEggCountChanged.Invoke(trueCount);
            if (trueCount >= ceckedEggCount)
            {
                SoundManager.instance.StopClip(SoundType.Tiktak);
                stopTime?.Invoke();
                if (eggSlotDic.Count > 0)
                {
                    foreach (var item in eggSlotDic)
                    {
                        if (item.Value != null)
                        {

                            EggSpawner.instance.dragon.transform.SetParent(item.Value.transform);
                            EggSpawner.instance.dragon.transform.localPosition = new Vector3(0, 0.2f, 0);
                            break;
                        }
                    }
                }
                // gameReStart?.Invoke();
                AbilityBarPanel.SetActive(false);

                EggSpawner.instance.DragonSetActive();
                ChangeMaterial();

                SoundManager.instance.PlaySfx(SoundType.Check);



                ResourceManager.Instance.AddResource(ResourceType.LevelIndex, 1);


            }
            else if (gameData.isTutorial)
            {
                tutorialIndex = 1;
            }
        }

    }
    private int GetTrueEggCount()
    {
        
        int trueCount = 0;
        int i = 0;
        LevelData currentLevel = GetLevelData();
        foreach (var item in currentLevel.eggColors)
        {
            if (eggSlotDic.TryGetValue(i, out GameObject egg))
            {
                Egg eggScript = egg.GetComponent<Egg>();
                if (eggScript != null && eggScript.IsCorrect(item))
                {
                    trueCount++;
                }
            }
            i++;
        }
      
        return trueCount;
    }
   private void ChangeMaterial()
    {
       foreach(Transform egg in EggSpawner.instance.EggParent)
        {
            if( !eggSlotDic.ContainsValue(egg.gameObject))
            {
                egg.gameObject.SetActive(false);
            }
        }
        foreach (var item in eggSlotDic)
        {
            Renderer objectRenderer = item.Value.GetComponentInChildren<Renderer>();
            if (item.Value != null)
            {
                // Bir animasyon zinciri (Sequence) oluşturuyoruz
                Sequence mySequence = DOTween.Sequence();

                // 1. ADIM: Mevcut materyali yavaşça görünmez yap (Fade out)
                // Not: .material kullandığımız için materyalin bir kopyası üzerinde çalışıyoruz.
                mySequence.Append(objectRenderer.material.DOFade(0f, fadeDuration));

                // 2. ADIM: Fade out bittiğinde materyali değiştir ve yeni materyali anında görünmez yap
                mySequence.AppendCallback(() => {
                   Material newMaterial= DragonDataSO.dragonMaterial[0];
                    Color newColor = newMaterial.color;
                    newColor.a = 0f;

                    // Önce rengi ayarla, sonra materyali ata
                    // Bu kısım önemli, çünkü yeni materyalin doğrudan 0 alfa ile başlamasını sağlıyoruz
                    newMaterial.color = newColor;
                    objectRenderer.material = newMaterial;
                });

                // 3. ADIM: Şimdi yeni materyali yavaşça görünür yap (Fade in)
                // Tekrar .material diyoruz ki doğru kopyayı fade edelim
                mySequence.Append(objectRenderer.material.DOFade(1f, fadeDuration));
            }
        }
        ShuffleRandomly();
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
public enum RewardedType
{
    Energy,
    Resource,
    OneResource,
}
