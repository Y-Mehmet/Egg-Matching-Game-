using UnityEngine;

public class PlaySettingBtn : MonoBehaviour
{
    public GameObject playSettingPanel; // Ayarlar paneli referansý
    private void OnEnable()
    {
        // Butonun týklanma olayýný dinle
        playSettingPanel.SetActive(false); // Baþlangýçta paneli gizle
        
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
        // Panelin aktiflik durumunu deðiþtir
        playSettingPanel.SetActive(!playSettingPanel.activeSelf);
        GameManager.instance.pauseGame?.Invoke(); // Oyun duraklatma olayýný tetikle


    }
    private void PanelSetActiveFalse()
    {
        playSettingPanel.SetActive(false); // Oyun baþladýðýnda paneli gizle
    }
}
