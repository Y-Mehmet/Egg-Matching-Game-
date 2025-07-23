using UnityEngine;
using UnityEngine.UI;

public class ResultOfDragonEggPanel : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    
    private void OnEnable()
    {
        closeBtn.onClick.AddListener(OnCloseBtnClicked);
    }
    private void OnDisable()
    {
        closeBtn.onClick.RemoveListener(OnCloseBtnClicked);
    }
    private void OnCloseBtnClicked()
    {
        ResourceManager.Instance.AddResource(ResourceType.LevelIndex, 1);
        PanelManager.Instance.ShowPanel(PanelID.LevelUpPanel, PanelShowBehavior.HIDE_PREVISE);
    }
    
    

}
