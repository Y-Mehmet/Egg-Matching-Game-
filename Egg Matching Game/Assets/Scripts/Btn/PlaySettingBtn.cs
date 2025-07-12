using UnityEngine;

public class PlaySettingBtn : MonoBehaviour
{
   
    private void OnEnable()
    {
        
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(TogglePlaySettingPanel);
        GameManager.instance.gameStart += PanelSetActiveFalse; // Oyun baþladýðýnda paneli aç
        GameManager.instance.continueGame += PanelSetActiveFalse; // Oyun devam ettiðinde paneli açma olayýný dinle
        GameManager.instance.gameReStart += PanelSetActiveFalse;
    }
    private void OnDisable()
    {
        // Butonun týklanma olayýný dinleme
        GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(TogglePlaySettingPanel);
        GameManager.instance.gameStart -= PanelSetActiveFalse; // Oyun baþladýðýnda paneli açmayý durdur
        GameManager.instance.continueGame -= PanelSetActiveFalse; // Oyun devam ettiðinde paneli açmayý durdur
        GameManager.instance.gameReStart -= PanelSetActiveFalse; // Oyun yeniden baþlatýldýðýnda paneli gizle
    }
    private void TogglePlaySettingPanel()
    {
        PanelManager.Instance.ShowPanel(PanelID.PlayPause, PanelShowBehavior.HIDE_PREVISE); // Paneli açma veya kapama iþlemi
        GameManager.instance.pauseGame?.Invoke(); // Oyun duraklatma olayýný tetikle


    }
    private void PanelSetActiveFalse()
    {
       PanelManager.Instance.HideAllPanel();
    }
}
