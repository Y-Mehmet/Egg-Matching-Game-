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
    [SerializeField] private Image abilityImage;    // Coin/Gem ikonunu gösterecek resim
 




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

    //    // Onaylandýðýnda ne yapýlacaðýný sakla
    //    this.onConfirmAction = onConfirm;

    //    // Paneli göster
    //    panelRoot.SetActive(true);
    //}

    ///// <summary>
    ///// Onay butonuna basýldýðýnda çalýþýr.
    ///// </summary>
    //private void OnConfirm()
    //{
    //    // Sakladýðýmýz onay eylemini çalýþtýr.
    //    onConfirmAction?.Invoke();

    //    // Paneli kapat
    //    ClosePanel();
    //}

    ///// <summary>
    ///// Paneli kapatýr.
    ///// </summary>
    //private void ClosePanel()
    //{
    //    // Paneli gizle
    //    panelRoot.SetActive(false);
    //    onConfirmAction = null; // Hafýzada kalmasýn diye temizle
    //}
}