using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


using Color = UnityEngine.Color;
public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;
    private Material baseEggMat;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        baseEggMat = Resources.Load<Material>("Materials/emptyEgg");
    }
   
    public void SetMaterial(GameObject egg,EggColor eggColor= EggColor.Random)
    {
      
        if (eggColor == EggColor.Random)
        {
            int randomEggColorIndex = Random.Range(0, GameManager.instance.GetLevelData().eggColors.Count);
            eggColor = GameManager.instance.GetLevelData().eggColors[randomEggColorIndex]; // Rastgele renk seçimi
            Renderer rend = egg.transform.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                rend.material = new Material(baseEggMat);
                Debug.LogWarning("new joker renderea new material atandý" + rend.name + " " + rend.material.name);  
            }
            else
            {
                Debug.LogWarning($"Yumurta {egg.name} için Renderer bulunamadý!");
            }
        }
            egg.GetComponentInChildren<Renderer>().material.color = GetEggColor(eggColor);
         
    }
    public Color GetEggColor(EggColor color)
    {
        switch (color)
        {
            case EggColor.Yellow:
                return Color.yellow;
            case EggColor.Red:
                return Color.red;
            case EggColor.Green:
                return Color.green;
            case EggColor.Blue:
                return Color.blue;
            case EggColor.Orange:
                return new Color(1f, 0.5f, 0f); // turuncu (RGB: 255,128,0)
            case EggColor.Purple:
                return new Color(0.5f, 0f, 0.5f); // mor (RGB: 128,0,128)
            case EggColor.Pink:
                return new Color(1f, 0.41f, 0.71f); // pembe (RGB: 255,105,180)
            case EggColor.Cyan:
                return Color.cyan;
            case EggColor.White:
                return Color.white;
            case EggColor.Black:
                return Color.black;
            default:
                Debug.LogError("Invalid EggColor");
                return Color.white;
        }


    }


    public EggColor GetRandomColor()
    {
        // Enum'daki eleman sayýsýný alarak daha dinamik bir kod yazabiliriz.
        // Bu sayede yeni bir renk eklediðinizde kodu deðiþtirmek zorunda kalmazsýnýz.
        int enumCount = System.Enum.GetValues(typeof(EggColor)).Length;
        int random = Random.Range(0, enumCount);

        switch (random)
        {
            case 0:
                return EggColor.Yellow;
            case 1:
                return EggColor.Red;
            case 2:
                return EggColor.Green;
            case 3:
                return EggColor.Blue;
            case 4:
                return EggColor.Orange;
            case 5:
                return EggColor.Purple;
            case 6:
                return EggColor.Pink;
            case 7:
                return EggColor.Cyan;
            case 8:
                return EggColor.White;
            case 9:
                return EggColor.Black;
            default:
                Debug.LogError("Invalid random number generated for EggColor.");
                return EggColor.White; // Varsayýlan bir deðer döndürmek iyi bir pratik
        }
    }
}
