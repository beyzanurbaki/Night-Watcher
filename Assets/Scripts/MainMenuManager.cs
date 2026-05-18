using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel;

    private void Start()
    {
        // Başlangıçta ayarlar panelinin kapalı olduğundan emin oluyoruz
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // Başla butonuna tıklandığında çalışacak fonksiyon
    public void StartGame()
    {
        // "Tutorial" sahnesini yüklüyoruz. Sahne adının Build Settings'dekiyle tam eşleştiğinden emin olun.
        Debug.Log("Tutorial sahnesi yükleniyor...");
        SceneManager.LoadScene("Tutorial");
    }

    // Ayarlar butonuna tıklandığında çalışacak fonksiyon
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            Debug.Log("Ayarlar paneli açıldı.");
        }
        else
        {
            Debug.LogWarning("Settings Panel atanmamış!");
        }
    }

    // Ayarlar panelindeki Kapat (veya Geri) butonuna tıklandığında çalışacak fonksiyon
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Debug.Log("Ayarlar paneli kapatıldı.");
        }
    }

    // Çıkış butonuna tıklandığında çalışacak fonksiyon
    public void QuitGame()
    {
        Debug.Log("Oyundan çıkılıyor...");
        Application.Quit();
    }
}
