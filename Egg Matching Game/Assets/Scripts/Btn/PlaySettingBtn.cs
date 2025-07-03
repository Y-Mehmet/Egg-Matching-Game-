using UnityEngine;

public class PlaySettingBtn : MonoBehaviour
{
    public GameObject playSettingPanel; // Ayarlar paneli referans�
    private void OnEnable()
    {
        // Butonun t�klanma olay�n� dinle
        playSettingPanel.SetActive(false); // Ba�lang��ta paneli gizle
        
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(TogglePlaySettingPanel);
        GameManager.instance.gameStart += PanelSetActiveFalse; // Oyun ba�lad���nda paneli a�
        GameManager.instance.continueGame += PanelSetActiveFalse; // Oyun devam etti�inde paneli a�ma olay�n� dinle
        GameManager.instance.gameReStart += PanelSetActiveFalse;
    }
    private void OnDisable()
    {
        // Butonun t�klanma olay�n� dinleme
        GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(TogglePlaySettingPanel);
        GameManager.instance.gameStart -= PanelSetActiveFalse; // Oyun ba�lad���nda paneli a�may� durdur
        GameManager.instance.continueGame -= PanelSetActiveFalse; // Oyun devam etti�inde paneli a�may� durdur
        GameManager.instance.gameReStart -= PanelSetActiveFalse; // Oyun yeniden ba�lat�ld���nda paneli gizle
    }
    private void TogglePlaySettingPanel()
    {
        // Panelin aktiflik durumunu de�i�tir
        playSettingPanel.SetActive(!playSettingPanel.activeSelf);
        GameManager.instance.pauseGame?.Invoke(); // Oyun duraklatma olay�n� tetikle


    }
    private void PanelSetActiveFalse()
    {
        playSettingPanel.SetActive(false); // Oyun ba�lad���nda paneli gizle
    }
}
