using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class EggSpawner : MonoBehaviour
{
    public static EggSpawner instance;
    public GameObject EggPrefab, SlotPrefab, JokerEggPrefab, AntiJokerEggPrefab, HiddenEggPrefeb, dragonPrefab, dragon;
    public Transform EggParent, SlotParent;
    public int HiddenEggCount = 0; // Gizli yumurta say�s�
    public int JokerEggCount = 0; // Joker yumurta say�s�
    public int AntiJokerEggCount = 0; // Anti-joker yumurta say�s�

    public int slotCount;
    public int topEggCount;
    public int topEggCountPerColor = 1;

    private bool TruePos = false;
    public List<EggColor> mixColorList = new List<EggColor>();
   // public List<EggColor> topEggColor = new List<EggColor>();
    public List<GameObject> eggList = new List<GameObject>();
    public List<Queue<GameObject>> eggStackList = new List<Queue<GameObject>>();
    public List<Vector3> slotPos = new List<Vector3>();
    public List<Vector3> topEggPos = new List<Vector3>();

    private Vector3 slotStartPos, slotSecondStartPos, topSlotStartPos, topSlotSecondStartPos, topSlotOfsset = new Vector3(0, 2, 0);
    private float eggDistance = 2f, eggWeight = .7f;
    Material hidenMat, jokerMat, antiJokerMat;

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
    }
    private void Start()
    {
        dragon = Instantiate(dragonPrefab);
        dragon.gameObject.SetActive(false);
    }
    public void DragonSetActive()
    {
        Debug.LogWarning("dragon set actif");
        dragon.SetActive(true);
    }

    private void OnEnable()
    {
        GameManager.instance.levelChanged += SpawnEgg;
    }
    private void OnDisable()
    {
        GameManager.instance.levelChanged -= SpawnEgg;
    }

    public void SpawnEgg(int levelIndex)
    {
        mixColorList.Clear();
        SaveGameData gameData = SaveSystem.Load();
        //if (!gameData.isTutorial)
        //    topEggColor.Clear();
        foreach (var item in eggList)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in SlotParent)
        {
            Destroy(item.gameObject);
        }
        GameManager.instance.slotList.Clear();
        GameManager.instance.SlotPositionList.Clear();
        eggList.Clear();
        eggStackList.Clear();
        slotPos.Clear();
        topEggPos.Clear();
        slotCount = GameManager.instance.GetLevelData().GetSlotCount();
        topEggCountPerColor = GameManager.instance.GetLevelData().GetTopEggPerCount();
        topEggCount = GameManager.instance.GetLevelData().GetTopEggCount() / topEggCountPerColor;

        GameManager.instance.slotCount = slotCount;
        CalculatePositions(); // De�i�tirilen mant��� �a��r�r

        GetColor();
        Spawner();
        SetColor();
    }

    // <<< DE���T�R�LEN FONKS�YON 1 >>>
    /// <summary>
    /// Verilen say�da nesneyi X ekseninde m�kemmel �ekilde ortalamak i�in 
    /// gereken ba�lang�� pozisyonunu hesaplar. Hem tek hem de �ift say�lar i�in �al���r.
    /// </summary>
    /// <param name="itemsInRow">Sadece bu s�radaki nesne say�s�.</param>
    /// <param name="itemWeight">�ki nesne merkezi aras�ndaki mesafe.</param>
    /// <returns>�lk nesnenin olmas� gereken merkezlenmi� pozisyon.</returns>
    Vector3 CalculateCenteredStartPosition(int itemsInRow, float itemWeight)
    {
        if (itemsInRow <= 1)
        {
            return Vector3.zero;
        }
        // S�ran�n toplam geni�li�ini hesapla: (nesne say�s� - 1) * aral�k
        float totalWidth = (itemsInRow - 1) * itemWeight;
        // Ba�lang�� X pozisyonu, bu geni�li�in yar�s�n�n negatifi olmal�d�r.
        float startX = -totalWidth / 2.0f;
        return new Vector3(startX, 0, 0);
    }

    // <<< DE���T�R�LEN FONKS�YON 2 >>>
    /// <summary>
    /// Slotlar�n ve yumurtalar�n tek veya iki s�ra halinde, her zaman
    /// merkezlenmi� olarak pozisyonlar�n� hesaplar.
    /// </summary>
    void CalculatePositions()
    {
        // --- Slot Pozisyonlar�n� Hesapla ---
        if (slotCount <= 5)
        {
            // Tek s�ra varsa, t�m slotlar i�in tek bir ba�lang�� pozisyonu hesapla
            slotStartPos = CalculateCenteredStartPosition(slotCount, eggWeight);
            slotSecondStartPos = Vector3.zero; // �kinci s�ra kullan�lm�yor
        }
        else
        {
            // �ki s�ra varsa, her s�ra i�in ayr� ayr� hesapla
            int firstRowCount = slotCount / 2;
            int secondRowCount = slotCount - firstRowCount;
            slotStartPos = CalculateCenteredStartPosition(firstRowCount, eggWeight);
            slotSecondStartPos = CalculateCenteredStartPosition(secondRowCount, eggWeight);
        }

        // --- �st Yumurta Pozisyonlar�n� Hesapla (Ayn� mant�k) ---
        if (topEggCount <= 5)
        {
            topSlotStartPos = CalculateCenteredStartPosition(topEggCount, eggWeight);
            topSlotSecondStartPos = Vector3.zero;
        }
        else
        {
            int firstRowCount = topEggCount / 2;
            int secondRowCount = topEggCount - firstRowCount;
            topSlotStartPos = CalculateCenteredStartPosition(firstRowCount, eggWeight);
            topSlotSecondStartPos = CalculateCenteredStartPosition(secondRowCount, eggWeight);
        }

        // Dikey ofsetleri en son uygula
        slotSecondStartPos += new Vector3(0, -1, 0);
        topSlotStartPos += topSlotOfsset * 1.5f;
        topSlotSecondStartPos += topSlotOfsset;

        // Hesaplanan pozisyonlar� listelere doldur
        AddSlotPositions(slotCount, slotStartPos, slotSecondStartPos, GameManager.instance.SlotPositionList, slotPos);
        AddSlotPositions(topEggCount, topSlotStartPos, topSlotSecondStartPos, null, topEggPos);
    }

    // AddSlotPositions fonksiyonu, yeni hesaplama mant���yla uyumlu oldu�u i�in de�i�tirilmedi.
    void AddSlotPositions(int count, Vector3 start, Vector3 secondStart, List<Vector3> globalList, List<Vector3> localList)
    {
        int half = count / 2;
        for (int i = 0; i < count; i++)
        {
            // count > 5 ise iki s�ral� mant��� kullan
            bool isSecondRow = (count > 5 && i >= half);

            Vector3 basePos = isSecondRow ? secondStart : start;
            int indexInRow = isSecondRow ? (i - half) : i;

            Vector3 pos = basePos + new Vector3(indexInRow * eggWeight, 0, 0);

            localList.Add(pos);
            if (globalList != null)
                globalList.Add(pos);
        }
    }

    // --- SINIFIN GER� KALANI DE���T�R�LMED� ---

    private void GetMaterial()
    {
        hidenMat = Resources.Load<Material>("Materials/HiddenMat");
        jokerMat = Resources.Load<Material>("Materials/JokerMat");
        antiJokerMat = Resources.Load<Material>("Materials/AntiJokerMat");
    }

    private void Spawner()
    {
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = Instantiate(SlotPrefab, slotPos[i], Quaternion.identity, SlotParent);
            slot.GetComponent<Slot>().slotIndex = i;
            GameManager.instance.slotList.Add(slot);
        }
        for (int i = 0; i < topEggCount; i++)
        {
            Queue<GameObject> eggStack = new Queue<GameObject>();
            for (int j = 0; j < topEggCountPerColor; j++)
            {
                if (j == 0)
                    eggStack.Clear();

                Vector3 eggPoss = topEggPos[i];
                GameObject egg = Instantiate(EggPrefab, eggPoss, Quaternion.identity, EggParent);
                egg.GetComponent<Egg>().startTopStackIndex = i;
                eggStack.Enqueue(egg);
                if (j == 0)
                    egg.SetActive(true);
                else if (j == topEggCountPerColor - 1)
                {
                    egg.SetActive(false);
                    eggStackList.Add(eggStack);
                }
                else
                    egg.SetActive(false);
                eggList.Add(egg);
                egg.GetComponent<Egg>().startPos = eggPoss;
            }
        }
    }
    private void GetColor()
    {
        foreach (var color in GameManager.instance.GetLevelData().topEggColors)
        {
            mixColorList.Add(color);
        }
        if (GameManager.instance.GetLevelData().mixColor == true)
        {
            mixColorList = mixColorList.OrderBy(x => Random.value).ToList();
        }
    }

    private void SetColor()
    {
        for (int i = 0; i < eggList.Count; i++)
        {
            string eggcolor = mixColorList[0].ToString();

            eggList[i].GetComponent<Egg>().eggColor = mixColorList[0];
            eggList[i].GetComponentInChildren<Renderer>().material.color = ColorManager.instance.GetEggColor(mixColorList[0]);
            mixColorList.Remove(mixColorList[0]);

            eggList[i].name = eggList[i].GetComponent<Egg>().eggColor.ToString() + "Egg";
        }
        if (HiddenEggCount > 0)
        {
            for (int i = 0; i < HiddenEggCount; i++)
            {
                Renderer rend = eggList[i].transform.GetComponentInChildren<Renderer>();
                if (rend != null)
                    rend.material = new Material(hidenMat);
                else
                    Debug.LogWarning($"Yumurta {i} i�in Renderer bulunamad�!");
                eggList[i].GetComponent<Egg>().properties.Add(new HiddenProperty());
                eggList[i].name = "HiddenEgg";
            }
        }
        if (JokerEggCount > 0)
        {
            for (int i = HiddenEggCount; i < HiddenEggCount + JokerEggCount; i++)
            {
                Renderer rend = eggList[i].transform.GetComponentInChildren<Renderer>();
                if (rend != null)
                    rend.material = new Material(jokerMat);
                else
                    Debug.LogWarning($"Yumurta {i} i�in Renderer bulunamad�!");
                eggList[i].GetComponent<Egg>().properties.Add(new JokerProperty());
                eggList[i].name = "JokerEgg";
            }
        }
        if (AntiJokerEggCount > 0)
        {
            for (int i = HiddenEggCount + JokerEggCount; i < HiddenEggCount + JokerEggCount + AntiJokerEggCount; i++)
            {
                Renderer rend = eggList[i].transform.GetComponentInChildren<Renderer>();
                if (rend != null)
                    rend.material = new Material(antiJokerMat);
                else
                    Debug.LogWarning($"Yumurta {i} i�in Renderer bulunamad�!");
                eggList[i].GetComponent<Egg>().properties.Add(new AntiJokerProperty());
                eggList[i].name = "AJokerEgg";
            }
        }
    }
}