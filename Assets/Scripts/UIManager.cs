using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton (her yerden erişim)

    [Header("UI Referansları")]
    public GameObject interactionPanel;

    private GameObject currentNPC; // Hangi NPC ile etkileşimde

    void Awake()
    {
        // Singleton ayarla
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Başlangıçta panel kapalı
        interactionPanel.SetActive(false);
    }

    /// <summary>
    /// Etkileşim menüsünü aç
    /// </summary>
    public void ShowInteractionMenu(GameObject npc)
    {
        currentNPC = npc;
        interactionPanel.SetActive(true);
        Time.timeScale = 0f; // Oyunu duraklat
        Debug.Log("Menü açıldı: " + npc.name);
    }

    /// <summary>
    /// Menüyü kapat
    /// </summary>
    public void CloseInteractionMenu()
    {
        interactionPanel.SetActive(false);
        Time.timeScale = 1f; // Oyunu devam ettir
        currentNPC = null;
        Debug.Log("Menü kapandı");
    }

    /// <summary>
    /// Butonlardan çağrılacak - Aksiyon gerçekleştir
    /// </summary>
    public void OnActionButton(string actionType)
    {
        if (currentNPC == null) return;

        // NPC'nin controller'ını al
        NPCController npc = currentNPC.GetComponent<NPCController>();
        if (npc == null)
        {
            Debug.Log("HATA: NPCController bulunamadı!");
            return;
        }

        // Aksiyona göre anı ekle
        float impact = 0f;
        List<string> tags = new List<string>();

        switch (actionType)
        {
            case "greet":
                impact = 0.3f;
                tags.Add("social");
                break;
            case "gift":
                impact = 0.6f;
                tags.Add("social");
                tags.Add("gift");
                break;
            case "help":
                impact = 0.5f;
                tags.Add("help");
                break;
            case "shout":
                impact = -0.4f;
                tags.Add("negative");
                tags.Add("noise");
                break;
            case "attack":
                impact = -0.7f;
                tags.Add("negative");
                tags.Add("threat");
                break;
        }

        // Hafızaya ekle
        npc.AddMemory(actionType, impact, tags);

        // Tutumu göster
        Debug.Log($"{npc.npcName} tutumu: {npc.GetDispositionLabel()} ({npc.GetOverallDisposition():F2})");

        CloseInteractionMenu();
    }
}