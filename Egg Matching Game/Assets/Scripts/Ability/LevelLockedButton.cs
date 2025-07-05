using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Ensures that a Button component is present on the GameObject this script is attached to.
[RequireComponent(typeof(Button))]
public class LevelLockedButton : MonoBehaviour
{
    [Header("Ability Data")]
    public AbilityData abilityData;
    [Header("Level Settings")]
    [Tooltip("The minimum level required to unlock this button.")]
    public int requiredLevel = 1; // You will set this value for each button in the Inspector.

    [Tooltip("The object to be displayed when the button is locked (usually a lock icon).")]
    public GameObject lockObject; // Drag the 'close' GameObject here from the Inspector.

    [Header("Optional Components")]
    [Tooltip("(Optional) The text field where we will display the required level.")]
    public TMP_Text levelText; // The 'Level 4' Text (TMP) object.

    private Button button;
    private SaveGameData gameData;

    private void Awake()
    {
        // Get the Button component once and store it in a variable.
        button = GetComponent<Button>();
    }

    // This function is called every time the object becomes active. This ensures it stays updated when UI panels are opened/closed.
    private void OnEnable()
    {
        // Load the saved game data.
        // IMPORTANT: Ensure your SaveSystem and SaveGameData classes exist in your project.
        gameData = SaveSystem.Load();

        // Check for null to prevent errors if no save file exists (e.g., first time playing).
        if (gameData == null)
        {
            Debug.LogError("Saved game data (SaveGameData) not found! The button will remain locked.");
            // You could also create a new SaveGameData here with default values.
            // e.g., gameData = new SaveGameData(); gameData.levelIndex = 0;
            SetButtonState(false); // Lock the button if there's no data.
            return;
        }

        // The player's current level (assuming levelIndex starts from 0, so we add 1).
        int currentLevel = gameData.levelIndex + 1;

        // Check if the player's level is sufficient.
        if (currentLevel >= requiredLevel)
        {
            // If the player's level is high enough, unlock the button.
            SetButtonState(true);
        }
        else
        {
            // If the level is not high enough, lock the button.
            SetButtonState(false);
        }

        // If a levelText component is assigned, update it with the required level.
        if (levelText != null)
        {
            levelText.text = "Level " + requiredLevel;
        }

        // Add a listener for the button's click event.
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        // It's good practice to remove the listener when the object is disabled to prevent memory leaks.
        button.onClick.RemoveListener(OnButtonClick);
    }

    /// <summary>
    /// Sets the state of the button (locked/unlocked).
    /// </summary>
    /// <param name="isUnlocked">True to unlock the button, False to lock it.</param>
    private void SetButtonState(bool isUnlocked)
    {
        // Set the button's interactability.
        button.interactable = isUnlocked;

        // Set the visibility of the lock object.
        // If isUnlocked is true, the lockObject should be inactive (hidden).
        // If isUnlocked is false, the lockObject should be active (visible).
        if (lockObject != null)
        {
            lockObject.SetActive(!isUnlocked);
        }
    }

    /// <summary>
    /// This function is executed when the button is clicked.
    /// </summary>
    private void OnButtonClick()
    {
        abilityData.action.Execute();
    }
}