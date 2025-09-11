
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
    public bool isGameFinish = false;
    

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
    private Coroutine coroutine;

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
        gameStart += StartTutorial;
        gameOver += GameOver;
        onSlotIndexChange += (int slotIndex, GameObject egg) => EmtySlotChecker();

        Time.timeScale = 1;
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowBannerAd();
        }

    }
    private void OnDisable()
    {
        gameStart -= GameStart;
        gameStart -= StartTutorial;
        gameOver -= GameOver;
        onSlotIndexChange -= (int slotIndex, GameObject egg) => EmtySlotChecker();


        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.HideBannerAd();
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

        TutorialManager.Instance.HandButton.interactable = false;
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.EggParent.GetChild(0).position);
        Vector3 offset = new Vector3(-75, 150, 0);

        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitUntil(()=>selectedEgg!= null);

        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.SlotParent.GetChild(0).position);
        TutorialManager.Instance.HandMovment(screenPoint+ offset, new Vector3(0, -10, 0));
            yield return new WaitUntil(() => eggSlotDic.ContainsKey(0));


        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.EggParent.GetChild(1).position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitUntil(() => selectedEgg != null);
        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.SlotParent.GetChild(1).position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitUntil(() => eggSlotDic.ContainsKey(1));

        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.EggParent.GetChild(2).position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitUntil(() => selectedEgg != null);
        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.SlotParent.GetChild(2).position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitUntil(() => eggSlotDic.ContainsKey(2));
        
         AbilityBarPanel.transform.GetChild(4).TryGetComponent<CheckBtn>(out CheckBtn checkBtn);
       

        Vector3 checkBtnPos= FinalCountUI.instance.eggCountText.transform.position;
        TutorialManager.Instance.HandMovment(checkBtnPos+offset+new Vector3(-40,90,0), new Vector3(0, -10, 0));
        yield return new WaitUntil(() => tutorialIndex == 1);
        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.EggParent.GetChild(0).position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitForSeconds(3f);
        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.EggParent.GetChild(1).position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitUntil(()=> GetTrueEggCount()>=3);
        TutorialManager.Instance.HandMovment(checkBtnPos + offset+new Vector3(-40, 90, 0), new Vector3(0, -10, 0));

        yield return new WaitUntil(() => isShuffling == true);
        
        TutorialManager.Instance.handPointer.SetActive(false);
        
        yield return new WaitUntil(() => isShuffling == false);
        screenPoint = mainCamera.WorldToScreenPoint(EggSpawner.instance.dragon.transform.position);
        TutorialManager.Instance.HandMovment(screenPoint + offset, new Vector3(0, -10, 0));
        yield return new WaitUntil(()=>isSelectTrueDaragonEgg==true);
        TutorialManager.Instance.HandButton.interactable = true;
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
        if (!gameStarted || isShuffling || isGameFinish)
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
                    Check();
                }
                else if (eggSlotDic.ContainsValue(selectedEgg) && eggSlotDic.ContainsValue(clickedEgg.gameObject))
                {
                    AddEggListByIndex(eggSlotDic.FirstOrDefault(k => k.Value == clickedEgg.gameObject).Key, selectedEgg);
                    DeselectObject();
                    Check();
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
                    Check();
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
    public void CheckSlotEggColor()
    {
        Debug.LogWarning("Slot color checker is called");
        gameStarted = false;
        StartCoroutine(AnimateWrongEggs());
    }


    public IEnumerator AnimateWrongEggs()
    {
        List<GameObject> wrongEggs = new List<GameObject>();
        float totalDuration = 5f;
        float slowPhase = totalDuration * 0.5f; // İlk yarı yavaş
        float fastPhase = totalDuration * 0.5f; // İkinci yarı hızlı

        // Yanlış yumurtaları bul
        foreach (var egg in eggSlotDic.Values)
        {
            egg.transform.DOKill();

            if (!GetLevelData().eggColors.Contains(egg.GetComponent<Egg>().eggColor))
            {
                wrongEggs.Add(egg);
            }
        }

        if (wrongEggs.Count == 0)
        {
            yield return new WaitForSeconds(7f);
            AbilityBarPanel.SetActive(true);
            PanelManager.Instance.HidePanelWithPanelID(panelID: PanelID.AbilityPurchasePanel);
            gameStarted = true;

        }
           

        // DOTween Sequence
        Sequence seq = DOTween.Sequence();

        // 1️⃣ 2 saniye bekle
        seq.AppendInterval(2f);

        // 2️⃣ Slow Phase (tüm yanlış yumurtalar aynı anda)
        foreach (var egg in wrongEggs)
        {
            seq.Join(
                egg.transform.DOShakePosition(
                    duration: slowPhase,
                    strength: 0.05f,
                    vibrato: 8,
                    randomness: 45f
                )
            );
        }

        // 3️⃣ Fast Phase (tüm yanlış yumurtalar aynı anda)
        seq.AppendCallback(() =>
        {
            foreach (var egg in wrongEggs)
            {
                egg.transform.DOShakePosition(
                    duration: fastPhase,
                    strength: 0.06f,
                    vibrato: 8,
                    randomness: 45f
                ).OnComplete(() => { AddEggListByIndex(-1, egg); });
            }
        });

        // 4️⃣ Bittikten sonra diğer kodlar
        seq.OnComplete(() =>
        {
            AbilityBarPanel.SetActive(true);
            PanelManager.Instance.HidePanelWithPanelID(panelID: PanelID.AbilityPurchasePanel);
            gameStarted = true;
        });

        // Sequence çalıştır
        seq.Play();

        // Sequence bitene kadar bekle
        yield return seq.WaitForCompletion();
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
            onSlotedEggCountChange?.Invoke();
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
        gameStarted = false;

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
    public void HideAllOutline( List<GameObject> list)
    {
        foreach (var item in list)
        {

            GameObject child = item.transform.GetChild(0).transform.GetChild(0).gameObject;
            if (child != null)
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
        isGameFinish = false;
        if (ResourceManager.Instance.GetResourceAmount(ResourceType.Energy) <= 0)
        {
            Debug.LogWarning("Yeterli enerji yok, sahne yüklenemiyor.");
            SceeneManager.instance.LoadSceneAndEnergyPanel();
            return;
        }
        ResourceManager.Instance.AddResource(ResourceType.PlayCount, 1); // Oyun oynama sayısını artırıyoruz
        
        GetLevelData().RestartLevelData();
        trueEggCountChanged?.Invoke(0);
        levelChanged?.Invoke(gameData.levelIndex);
        if(eggSlotDic.Count > 0)
        {
            eggSlotDic.Clear();
        }
        if(coroutine!= null)
            StopCoroutine(coroutine);

       


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
        if(!ResourceManager.Instance.isTutorial)
        coroutine = StartCoroutine(AssingEggs());

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
        float stepDuration = swapDuration / 6.0f;
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
            Check();
            
        });

    }
    public IEnumerator AssingEggs()
    {
        LevelData currentLevel = GetLevelData();
        List<Transform> emtyOrWrongColorSlotTransformList = new List<Transform>();

        // 1. Boş slotları bul (Bu kısım aynı kalıyor, doğru çalışıyor)
        foreach (GameObject slot in slotList)
        {
            if (slot.TryGetComponent<Slot>(out Slot slotScript) && !eggSlotDic.ContainsKey(slotScript.slotIndex))
            {
                emtyOrWrongColorSlotTransformList.Add(slot.transform);
            }
        }

        if (emtyOrWrongColorSlotTransformList.Count > 0)
        {
            // <<< YENİ MANTIK BAŞLANGICI >>>
            // Atama düzenini ve sayacını döngüden önce ayarla.
            int skipPattern = currentLevel.GetTopEggPerCount();
            int skipCounter = 1; // Sayaç 1'den başlar. Sadece 1 olduğunda atama yaparız.

            // `EggParent` altındaki tüm potansiyel yumurtaları dolaş
            foreach (Transform egg in EggSpawner.instance.EggParent)
            {
                // Bu yumurtanın atanıp atanmayacağını belirle
                bool isAssignTurn = (skipCounter == 1);

                // Sayacı BİR SONRAKİ yumurta için her zaman güncelle.
                skipCounter++;
                if (skipCounter > skipPattern)
                {
                    skipCounter = 1; // Döngüyü başa sar
                }

                // Eğer bu yumurtayı atama sırası değilse, döngünün bir sonraki adımına geç.
                if (!isAssignTurn)
                {
                    continue;
                }

                // <<< ESKİ KODUN DOĞRULAMA KISMI >>>
                // Eğer atama sırasıysa, yumurtanın diğer koşulları sağlayıp sağlamadığını kontrol et.
                if (egg.gameObject.activeInHierarchy && !eggSlotDic.ContainsValue(egg.gameObject) && GetLevelData().GetTempTopEggColorList().Contains(egg.GetComponent<Egg>().eggColor))
                {
                    // Geçerli bir yumurta bulundu, ilk boş slota ata.
                    Transform targetSlotTransform = emtyOrWrongColorSlotTransformList[0];

                    Sequence eggAnimation = AnimateEggToSlot(egg.gameObject, targetSlotTransform);
                    yield return eggAnimation.WaitForCompletion();

                    eggSlotDic[targetSlotTransform.GetComponent<Slot>().slotIndex] = egg.gameObject;
                    PopStack(egg.gameObject);

                    // Dolu slotu listeden çıkar
                    emtyOrWrongColorSlotTransformList.RemoveAt(0);

                    // Eğer doldurulacak başka slot kalmadıysa, döngüyü tamamen bitir.
                    if (emtyOrWrongColorSlotTransformList.Count == 0)
                    {
                        Check();
                        yield break; // Coroutine'i sonlandır.
                    }
                }
            }
        }
        
    }
    public void ShuffleRandomly()
    {
        StartCoroutine(ShuffleRandomlyCoroutine());
    }

    // ESKİ ShuffleRandomlyCoroutine METODUNU BU YENİ VERSİYONLA TAMAMEN DEĞİŞTİRİN

    private IEnumerator ShuffleRandomlyCoroutine()
    {
        yield return new WaitForSeconds(1);
        if (isShuffling)
        {
            Debug.LogWarning("Karıştırma zaten devam ediyor.");
            yield break;
        }
        isShuffling = true;

        List<GameObject> allEggsInSlots = new List<GameObject>(eggSlotDic.Values);

        if (allEggsInSlots.Count < 2)
        {
            Debug.Log("Karıştırılacak yeterli yumurta yok.");
            isShuffling = false;
            PanelManager.Instance.ShowPanel(PanelID.BreakDragonEggPanel);
            abilityData.action.Execute();
            yield break;
        }

        // Animasyon başlamadan önce materyallerin geçişi için bekle
        yield return new WaitForSeconds(fadeDuration);

        Sequence mainShuffleSequence = DOTween.Sequence();
        float initialStepDuration = swapDuration / 4.0f;
        float minDuration = swapDuration / 12.0f;
        float decreaseDuration = .1f;

        // ANA DEĞİŞİKLİK: Karıştırma işlemini daha kaotik hale getirmek için birkaç tur (pass) yapıyoruz.
        for (int pass = 0; pass < shufflePasses; pass++)
        {
            // Her turda, henüz bir gruba atanmamış yumurtaların listesini tutarız.
            var remainingEggs = new List<GameObject>(allEggsInSlots);
            float stepDuration = initialStepDuration;

            // İşlenecek en az 2 yumurta kaldığı sürece gruplar oluşturmaya devam et.
            while (remainingEggs.Count >= 2)
            {
                // --- 1. ADIM: Takas grubunun boyutunu belirle ---
                // Mevcut yumurta sayısına göre 2, 3 veya 4'lü bir grup oluştur.
                // Örneğin 3 yumurta kaldıysa, 2'li veya 3'lü bir grup seçilebilir.
                // Animasyonun çok yavaşlamaması için maksimum grup boyutunu 4 ile sınırlıyoruz.
                int maxGroupSize = Mathf.Min(5, remainingEggs.Count + 1); // Random.Range'in üst sınırı exclusive olduğu için +1 ve 5
                int groupSize = Random.Range(2, maxGroupSize);

                // --- 2. ADIM: Rastgele bir takas grubu oluştur ---
                List<GameObject> currentGroup = new List<GameObject>();
                // Kalan yumurtaları karıştırıp ilk 'groupSize' tanesini seçerek rastgele bir grup oluştur.
                for (int i = 0; i < groupSize; i++)
                {
                    int randomIndex = Random.Range(0, remainingEggs.Count);
                    currentGroup.Add(remainingEggs[randomIndex]);
                    remainingEggs.RemoveAt(randomIndex);
                }

                // --- 3. ADIM: Grubun mevcut pozisyonlarını ve hedef pozisyonlarını belirle ---
                Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>();
                Dictionary<GameObject, int> initialIndexes = new Dictionary<GameObject, int>();

                foreach (var egg in currentGroup)
                {
                    int index = eggSlotDic.FirstOrDefault(kvp => kvp.Value == egg).Key;
                    initialIndexes[egg] = index;
                    initialPositions[egg] = slotList.FirstOrDefault(s => s.GetComponent<Slot>().slotIndex == index).transform.position;
                }

                // --- 4. ADIM: Animasyonları Sıralı Olarak Oluştur (Dairesel Takas) ---
                // Bu grubun tüm animasyonları AYNI ANDA başlayacak şekilde ayarlanacak.

                // ANİMASYON AŞAMASI 1: Tüm yumurtaları geri çek
                for (int i = 0; i < currentGroup.Count; i++)
                {
                    GameObject egg = currentGroup[i];
                    Vector3 backPos = new Vector3(initialPositions[egg].x, initialPositions[egg].y + zOffsetOnSwap, initialPositions[egg].z);

                    var moveTween = egg.transform.DOMove(backPos, stepDuration).SetEase(shuffleEase);
                    if (i == 0)
                    {
                        // Grubun ilk animasyonu 'Append' ile eklenir ve ses efekti tetiklenir
                        mainShuffleSequence.Append(moveTween.OnStart(() => SoundManager.instance.PlaySfx(SoundType.GoToSlot)));
                    }
                    else
                    {
                        // Gruptaki diğer tüm animasyonlar 'Join' ile ilkine katılır, böylece hepsi aynı anda başlar.
                        mainShuffleSequence.Join(moveTween);
                    }
                }

                // ANİMASYON AŞAMASI 2: Yumurtaları yeni pozisyonlarına (dairesel olarak) hareket ettir
                for (int i = 0; i < currentGroup.Count; i++)
                {
                    GameObject currentEgg = currentGroup[i];
                    // Bir sonraki yumurtanın pozisyonunu hedef olarak al (listenin sonundaysa başa dön)
                    GameObject targetEgg = currentGroup[(i + 1) % groupSize];
                    Vector3 targetPos = initialPositions[targetEgg];

                    Vector3 swapTarget = new Vector3(targetPos.x, targetPos.y, currentEgg.transform.position.z);

                    var moveTween = currentEgg.transform.DOMove(swapTarget, stepDuration * 2).SetEase(shuffleEase);
                    if (i == 0) mainShuffleSequence.Append(moveTween); else mainShuffleSequence.Join(moveTween);
                }

                // ANİMASYON AŞAMASI 3: Yumurtaları son pozisyonlarına yerleştir
                for (int i = 0; i < currentGroup.Count; i++)
                {
                    GameObject currentEgg = currentGroup[i];
                    GameObject targetEgg = currentGroup[(i + 1) % groupSize];
                    Vector3 finalPos = initialPositions[targetEgg];

                    var moveTween = currentEgg.transform.DOMove(finalPos, stepDuration).SetEase(shuffleEase);
                    if (i == 0) mainShuffleSequence.Append(moveTween); else mainShuffleSequence.Join(moveTween);
                }

                // --- 5. ADIM: Sözlükteki (eggSlotDic) verileri güncelle ---
                // Animasyonlar sıralanırken, mantıksal verileri de güncelleyelim.
                for (int i = 0; i < currentGroup.Count; i++)
                {
                    GameObject currentEgg = currentGroup[i];
                    GameObject targetEgg = currentGroup[(i + 1) % groupSize];

                    // Mevcut yumurtayı, hedef yumurtanın eski slot index'ine ata
                    eggSlotDic[initialIndexes[targetEgg]] = currentEgg;
                }

                // Takas grupları arasında küçük bir bekleme ekleyelim
                if (delayBetweenSwaps > 0)
                {
                    mainShuffleSequence.AppendInterval(delayBetweenSwaps);
                }

                // Animasyonu hızlandır
                if (stepDuration > minDuration)
                {
                    stepDuration -= decreaseDuration;
                }
            }
        }

        // Tüm animasyon döngüleri bittiğinde yapılacaklar
        mainShuffleSequence.OnComplete(() => {
            isShuffling = false;
            Debug.Log("Random Multi-Swap Shuffle complete!");
            PanelManager.Instance.ShowPanel(PanelID.BreakDragonEggPanel);
            abilityData.action.Execute();
        });

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

    private void EmtySlotChecker() // Parametreleri kaldırdık, çünkü artık onSlotedEggCountChange tarafından çağrılacak.
    {
        

        int sltCount = GetSlotCount();
        int ceckedEggCount = GetCheckedEggCount();
        if(eggSlotDic.Count>=ceckedEggCount)
        {
            HideAllOutline(slotList);
            return;
        }

        List<GameObject> emptySlotTransforms = new List<GameObject>(); // Sadece boş slotların objelerini tutacak

        // Tüm slotları dolaş
        for (int i = 0; i < sltCount; i++)
        {
            // Eğer bu slot boşsa (yani eggSlotDic'te bu index yoksa)
            if (!eggSlotDic.ContainsKey(i))
            {
                emptySlotTransforms.Add(slotList[i]);
           
            }
            else
            {
                // Dolu slotların rengini orijinal haline döndür
                slotList[i].GetComponentInChildren<Renderer>().material.color = originalColor;
            }
        }

        // Boş slotlar varsa onları işaretle, yoksa tüm işaretleri kaldır
        if (emptySlotTransforms.Count > 0)
        {
            HideAllOutline(slotList); // Önce tüm işaretleri kaldır
            ShowOutline(emptySlotTransforms); // Sadece boş olanları işaretle
        }
        else
        {
            HideAllOutline(slotList); // Hiç boş slot yoksa tüm işaretleri kaldır
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
     
        int ceckedEggCount = GetCheckedEggCount();




        if (eggSlotDic.Count >= ceckedEggCount)
        {
          
            canCheck = true;
            int trueCount = GetTrueEggCount();
            
            trueEggCountChanged.Invoke(trueCount);
           
            if (trueCount >= ceckedEggCount)
            {
                SoundManager.instance.StopClip(SoundType.Tiktak);
                isGameFinish = true;
                stopTime?.Invoke();
                //if (eggSlotDic.Count > 0)
                //{
                //    foreach (var item in eggSlotDic)
                //    {
                //        if (item.Value != null)
                //        {

                //            EggSpawner.instance.dragon.transform.SetParent(item.Value.transform);
                //            EggSpawner.instance.dragon.transform.localPosition = new Vector3(0, 0.2f, 0);
                //            break;
                //        }
                //    }
                //}

                if (eggSlotDic.Count > 0)
                {
                    // Null olmayan slotları filtrele
                    var validSlots = eggSlotDic
     .Where(kvp => kvp.Key != -1 && kvp.Value != null)
     .Select(kvp => kvp.Value)
     .ToList();



                    if (validSlots.Count > 0)
                    {
                        // Rastgele bir slot seç
                        int randomIndex = UnityEngine.Random.Range(0, validSlots.Count);
                        Transform chosenSlot = validSlots[randomIndex].transform;

                        // Dragon'u bu slot'a yerleştir
                        EggSpawner.instance.dragon.transform.SetParent(chosenSlot);
                        EggSpawner.instance.dragon.transform.localPosition = new Vector3(0, 0.2f, 0);
                    }
                    else
                    {
                        Debug.LogWarning("No valid egg slots found.");
                    }
                }
                else
                {
                    Debug.LogWarning("eggSlotDic is empty.");
                }

                // gameReStart?.Invoke();
                AbilityBarPanel.SetActive(false);

                EggSpawner.instance.DragonSetActive();

                if (EggSpawner.instance.dragon != null)
                {
                    // Önceki kodunuzun tamamını bu blokla değiştirebilirsiniz

                    GameObject dragon = EggSpawner.instance.dragon;
                    Transform parentTransform = dragon.transform.parent; // Mevcut parent'ı al

                    // 1. Hedefin dünya uzayındaki pozisyonunu belirle.
                    // Ejderhanın mevcut X ve Y dünya pozisyonunu koruyup, Z'sini -1f yap.
                    Vector3 targetWorldPosition = new Vector3(dragon.transform.position.x, dragon.transform.position.y, -1f);

                    // 2. Bu dünya pozisyonunu, parent'ın yerel koordinat sistemine çevir.
                    // "Bu dünya noktası, parent'ımın hangi lokal pozisyonuna denk geliyor?" diye sorarız.
                    Vector3 targetLocalPosition = parentTransform.InverseTransformPoint(targetWorldPosition);

                    // Ejderhayı aktif et
                    dragon.SetActive(true);

                    // 3. DOLocalMove ile parent'ına göre hareket ettir.
                    dragon.transform.DOLocalMove(targetLocalPosition, 1.5f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        // Animasyon bittiğinde ejderhanın pozisyonunu yeniden ayarlamaya gerek kalmaz,
                        // çünkü zaten doğru lokal pozisyonda olacaktır.
                        // Ancak yine de tam olarak istediğiniz pozisyonda olmasını garanti etmek için bu satırı bırakabilirsiniz.
                        dragon.transform.localPosition = new Vector3(0, 0.2f, 0);
                    });
                }
                else
                    Debug.LogWarning("dragon egg is null");

                ChangeMaterial();

               
                SoundManager.instance.PlaySfxSequentially(SoundType.UGOTIT, SoundType.Yess);
                SoundManager.instance.PlaySfx(SoundType.Clapping);



                ResourceManager.Instance.AddResource(ResourceType.LevelIndex, 1);


            }
            else if (gameData.isVibrationEnabled)
            {
                SoundManager.instance.PlaySfx((SoundType)(trueCount + 18));
            }
            if ((trueCount < ceckedEggCount) && gameData.isTutorial)
            {
                tutorialIndex = 1;
            }
        }

    }

    // ESKİ METODU BU YENİ VE DOĞRU BLOKLA DEĞİŞTİRİN

    /// <summary>
    /// Sahnedeki aktif slotların gerektirdiği renkler ile mevcut aktif yumurtaların renklerini,
    /// her bir rengin sayısını (frekansını) dikkate alarak karşılaştırır.
    /// Örneğin, 2 sarı slot için en az 2 sarı yumurta varsa eşleşme sayısını doğru hesaplar.
    /// </summary>
    /// <returns>Doğru eşleştirilebilecek maksimum yumurta sayısı.</returns>
    public int GetCheckedEggCount()
    {
        LevelData currentLevel = GetLevelData();
        if (currentLevel == null)
        {
            Debug.LogError("GetCheckedEggCount: LevelData bulunamadı!");
            return 0;
        }

        // Adım 1: Aktif olan slotların hangi renkte yumurtalara ihtiyaç duyduğunu bir listeye topla.
        // Bu listede renkler tekrar edebilir (örn: [Sarı, Sarı, Mavi]).
        List<EggColor> requiredColors = new List<EggColor>();
        foreach (GameObject slotGo in slotList)
        {
            if (slotGo != null && slotGo.activeInHierarchy)
            {
                Slot slotComponent = slotGo.GetComponent<Slot>();
                if (slotComponent != null && slotComponent.slotIndex < currentLevel.eggColors.Count)
                {
                    requiredColors.Add(currentLevel.eggColors[slotComponent.slotIndex]);
                }
            }
        }

        // Adım 2: Sahnedeki tüm aktif yumurtaların renklerini ve sayısını bir sözlüğe (Dictionary) kaydet.
        // Bu yapı, her renkten kaç tane olduğunu verimli bir şekilde tutar (örn: {Sarı: 2, Mavi: 1}).
        Dictionary<EggColor, int> availableColorCounts = new Dictionary<EggColor, int>();
        if (EggSpawner.instance != null && EggSpawner.instance.EggParent != null)
        {
            foreach (Transform eggTransform in EggSpawner.instance.EggParent)
            {
                if (eggTransform != null && eggTransform.gameObject.activeInHierarchy)
                {
                    Egg eggComponent = eggTransform.GetComponent<Egg>();
                    if (eggComponent != null)
                    {
                        EggColor color = eggComponent.eggColor;
                        if (availableColorCounts.ContainsKey(color))
                        {
                            availableColorCounts[color]++; // Eğer renk zaten varsa, sayısını bir artır.
                        }
                        else
                        {
                            availableColorCounts.Add(color, 1); // Eğer yeni bir renkse, sözlüğe ekle.
                        }
                    }
                }
            }
        }

        // Adım 3: Gereken renkler listesini, mevcut yumurta sayılarıyla karşılaştırarak eşleşme sayısını bul.
        int matchCount = 0;
        foreach (EggColor requiredColor in requiredColors)
        {
            // Eğer gereken renk, mevcut yumurtalar arasında varsa VE o renkten hala kullanabileceğimiz sayıda varsa...
            if (availableColorCounts.ContainsKey(requiredColor) && availableColorCounts[requiredColor] > 0)
            {
                matchCount++; // Eşleşme sayısını bir artır.
                              // Bu rengi "kullandığımız" için sözlükteki sayısını bir azalt.
                              // Böylece 2 sarı gerekip 1 sarı varken sadece bir eşleşme sayılır.
                availableColorCounts[requiredColor]--;
            }
        }

        return matchCount;
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
                mySequence.Append(objectRenderer.material.DOFade(1f, 2*fadeDuration));
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
