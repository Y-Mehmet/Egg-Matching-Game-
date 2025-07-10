// AbilityPurchasePanel.cs
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPurchasePanel : MonoBehaviour
{

    public static AbilityPurchasePanel Instance { get; private set; }

    [Header("UI Components")]
   
    [SerializeField] private TMP_Text nameText, desciriptionText;     // "Cost: 50" yazacak metin
    [SerializeField] private Image abilityImage;    // Coin/Gem ikonunu g�sterecek resim
 




    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }
    private void OnEnable()
    {
        Sprite loadedSprite = Resources.Load<Sprite>("Sprites/MySprite");
    }


    //public void ShowPanel()
    //{
    //    AbilityManager.Instance.currentAbilityType
    //    costText.text = cost.ToString();
    //    resourceIcon.sprite = icon;

    //    // Onayland���nda ne yap�laca��n� sakla
    //    this.onConfirmAction = onConfirm;

    //    // Paneli g�ster
    //    panelRoot.SetActive(true);
    //}

    ///// <summary>
    ///// Onay butonuna bas�ld���nda �al���r.
    ///// </summary>
    //private void OnConfirm()
    //{
    //    // Saklad���m�z onay eylemini �al��t�r.
    //    onConfirmAction?.Invoke();

    //    // Paneli kapat
    //    ClosePanel();
    //}

    ///// <summary>
    ///// Paneli kapat�r.
    ///// </summary>
    //private void ClosePanel()
    //{
    //    // Paneli gizle
    //    panelRoot.SetActive(false);
    //    onConfirmAction = null; // Haf�zada kalmas�n diye temizle
    //}
}