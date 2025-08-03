using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Rendering; // TextMeshPro için gerekli

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    
    [Header("Tutorial Elemanlarý")]
    public GameObject handPointer;
    public GameObject blockerPanel;
    public TextMeshProUGUI tutorialTextLabel;
    public Button HandButton;

    [Header("Tutorial Adýmlarý")]
    public Button[]tutorialSteps;
    int  stepCountMax=3; // Adým sayýsý, baþlangýçta 0
    



    public int currentStep = 0;
    private string welcomeMessage = "Welcome, Boss! Our goal is to build a legendary dragon family by arranging their eggs... Let's begin! Tap the 'Level 1' button to get your first dragon!";
    private string missionMessage = "Nice work, Boss! You have the gems for our first dragon. Tap 'Mission' to hatch it. Remember, dragons give you more time to play!";
    public List<Vector3> offsetList = new List<Vector3>();
    private void Awake()
    {
        // Singleton deseni ile bu sýnýfýn tek örneðini oluþtur
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Bu nesneyi sahneler arasýnda taþýmak için
        }
        else
        {
            Destroy(gameObject); // Eðer zaten bir örnek varsa, bu yeni örneði yok et
        }
    }
    private void OnEnable()
    {
        if(!ResourceManager.Instance.isTutorial)
        {
            gameObject.SetActive(false);
        }
        HandButton.onClick.AddListener(OnTargetButtonClicked);
    }
    private void OnDisable()
    {
        HandButton.onClick.RemoveListener(OnTargetButtonClicked);
    }
    void Start()
    {
        SaveGameData gameData = SaveSystem.Load();
        if (gameData == null && !gameData.isTutorial)
        {
            gameObject.SetActive(false);// Eðer oyun verisi yoksa bu nesneyi yok et
        }
       if(gameData.levelIndex==0)
        {
            offsetList.Add(new Vector3(-150, 120, 0)); // Ýlk adým için elin konumu
                                                       // Baþlangýçta her þeyi gizle
            handPointer.SetActive(false);
            blockerPanel.SetActive(true); // Týklamalarý en baþtan engelle
            tutorialTextLabel.gameObject.SetActive(true);

            // Tutorial akýþýný Coroutine olarak baþlat
            StartCoroutine(TutorialFlow());
        }else if( gameData.levelIndex==1)
        {
            offsetList.Clear();
            offsetList.Add(new Vector3(-150, 120, 0)); // Ýlk adým için elin konumu
                                                       // Baþlangýçta her þeyi gizle
            handPointer.SetActive(false);
            blockerPanel.SetActive(true); // Týklamalarý en baþtan engelle
            tutorialTextLabel.gameObject.SetActive(true);

            // Tutorial akýþýný Coroutine olarak baþlat
            StartCoroutine(TutorialFlow2());
        }
    }

    private IEnumerator TutorialFlow()
    {
        yield return null;
        yield return TypewriterHelper.Instance.Run(welcomeMessage, tutorialTextLabel, 0.04f);

        // Metin bittikten sonra biraz daha bekle ki oyuncu okuyabilsin
        yield return new WaitForSeconds(.5f);



        ShowNextStep();
    }
    private IEnumerator TutorialFlow2()
    {
        yield return null;
        yield return TypewriterHelper.Instance.Run(missionMessage, tutorialTextLabel, 0.04f);
        yield return new WaitForSeconds(.5f);




        currentStep = 3;
        ShowNextStep();
       
    }


    private void ShowNextStep()
    {
        // ... (Bu kýsýmdaki kodunuz ayný kalabilir) ...
        if (currentStep > stepCountMax)
        {
            EndTutorial();
            return;
        }

       if(currentStep==0)
        {
            Button targetButton = tutorialSteps[currentStep];
            blockerPanel.SetActive(true);
            handPointer.SetActive(true);


            handPointer.transform.SetAsLastSibling();
            targetButton.onClick.RemoveAllListeners();
            targetButton.onClick.AddListener(OnTargetButtonClicked);
            HandMovment(tutorialSteps[0].transform.position + offsetList[0], new Vector3(0, -10, 0));
        }
       else if(currentStep==3)
        {
            Button targetButton = tutorialSteps[1];
            blockerPanel.SetActive(true);
            handPointer.SetActive(true);


            handPointer.transform.SetAsLastSibling();
            targetButton.onClick.RemoveAllListeners();
            targetButton.onClick.AddListener(OnTargetButtonClicked);
            HandMovment(tutorialSteps[1].transform.position + offsetList[0], new Vector3(0, -10, 0));
        }
    }
    public void HandMovment(Vector3 targetPos, Vector3 yoyoMovement)
    {
        handPointer.SetActive(true);
        // Önce her iki UI elemanýnýn da RectTransform bileþenlerini almamýz gerekiyor.
        RectTransform handRectTransform = handPointer.GetComponent<RectTransform>();
        

      
      

        // Son olarak, elin RectTransform'unun pozisyonunu atýyoruz.
        handRectTransform.position = targetPos;
        Debug.LogWarning("Hand position set to: " + targetPos + " " + handRectTransform.position);

      
       

        // 2. Önceki animasyonlarý durdur (ÇOK ÖNEMLÝ!)
        // Bir önceki adýmdan kalan sonsuz döngüyü temizler.
        handPointer.transform.DOKill();
        handPointer.transform.DOBlendableMoveBy(yoyoMovement, 0.7f) // Mevcut konumdan 0.7 saniyede Y'de -30 hareket et
                   .SetEase(Ease.InOutSine) // Yumuþak bir geçiþ için InOutSine harikadýr
                   .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnTargetButtonClicked()
    {
        handPointer.SetActive(false);
        currentStep++;
        switch (currentStep)
        {
            case 1:

                SceeneManager.instance.LoadScene(2);
                break;
            case 4:
                ResourceManager.Instance.isTutorial = false;
                ResourceManager.Instance.SaveResources();
               
                PanelManager.Instance.ShowPanel(PanelID.DragonPaint);
               
                EndTutorial();
                break;
            default:
                break;
        }
            
            
        
    }

    public void EndTutorial()
    {
        // ... (Bu kýsýmdaki kodunuz ayný kalabilir) ...
        handPointer.SetActive(false);
        blockerPanel.SetActive(false);
        tutorialTextLabel.gameObject.SetActive(false);
    }
}