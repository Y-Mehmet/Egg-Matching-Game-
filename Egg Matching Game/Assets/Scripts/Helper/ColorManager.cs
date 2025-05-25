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
            int randomEggColorIndex = Random.Range(0, GameManager.instance.EggColorList.Count);
            eggColor = GameManager.instance.EggColorList[randomEggColorIndex]; // Rastgele renk seçimi
            egg.GetComponentInChildren<Renderer>().material= baseEggMat; // Temel yumurta materyalini kullan
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
}
