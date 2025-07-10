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
        if(AbilityManager.Instance!= null && AbilityManager.Instance.abilityDataHolder != null)
        {
            // AbilityManager'dan gerekli verileri al
            var abilityData = AbilityManager.Instance.abilityDataHolder.abilities.Find(a => a.Type == AbilityManager.Instance.currentAbilityType);
            if (abilityData != null)
            {
                // UI bileþenlerini güncelle
                nameText.text = abilityData.Title;
                desciriptionText.text = abilityData.Description;
                abilityImage.sprite = abilityData.Icon;
            }
            else
            {
                Debug.LogWarning("Ability data not found for the current ability type.");
            }
        }
        else
        {
            Debug.LogError("AbilityManager or its abilityDataHolder is not set.");
        }
        
    }


}