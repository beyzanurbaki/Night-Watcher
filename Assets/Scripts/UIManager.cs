using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Referanslari")]
    public GameObject interactionPanel;

    [Header("Memory Panel")]
    public GameObject memoryPanel;

    [Header("Etkilesim Hakki")]
    public int maxInteractions = 2;
    public int remainingInteractions = 2;

    [Header("Uyari")]
    public TextMeshProUGUI warningText;

    private GameObject currentNPC;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        interactionPanel.SetActive(false);
        memoryPanel.SetActive(false);

        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    public void ToggleMemoryPanel()
    {
        bool isActive = memoryPanel.activeSelf;
        memoryPanel.SetActive(!isActive);
    }

    public void ResetInteractions()
    {
        remainingInteractions = maxInteractions;
        Debug.Log($"Etkilesim hakki sifirlandi: {remainingInteractions}");
    }

    public void ShowInteractionMenu(GameObject npc)
    {
        if (remainingInteractions <= 0)
        {
            Debug.Log("Etkilesim hakkin kalmadi! Yeni gunu bekle.");
            StartCoroutine(ShowWarning("Etkilesim hakkin kalmadi! Yeni gunu bekle."));
            return;
        }

        currentNPC = npc;
        interactionPanel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log($"Menu acildi: {npc.name} (Kalan hak: {remainingInteractions})");
    }

    IEnumerator ShowWarning(string message)
    {
        if (warningText == null) yield break;

        warningText.text = message;
        warningText.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f); // 4 saniye

        warningText.gameObject.SetActive(false);
    }

    public void CloseInteractionMenu()
    {
        interactionPanel.SetActive(false);
        Time.timeScale = 1f;
        currentNPC = null;
        Debug.Log("Menu kapandi");
    }

    public void OnActionButton(string actionType)
    {
        if (currentNPC == null) return;

        NPCController npc = currentNPC.GetComponent<NPCController>();
        if (npc == null)
        {
            Debug.Log("HATA: NPCController bulunamadi!");
            return;
        }

        if (actionType != "ignore")
        {
            remainingInteractions--;
            Debug.Log($"Etkilesim hakki kullanildi. Kalan: {remainingInteractions}");
        }

        float impact = 0f;
        List<string> tags = new List<string>();

        switch (actionType)
        {
            case "greet":
                impact = 0.3f;
                tags.Add("social");
                tags.Add("daytime");
                tags.Add("location_park");
                break;

            case "gift":
                impact = 0.6f;
                tags.Add("social");
                tags.Add("gift_item");
                tags.Add("daytime");
                break;

            case "help":
                impact = 0.5f;
                tags.Add("help");
                tags.Add("threat_nearby");
                tags.Add("night_patrol");
                break;

            case "ignore":
                impact = 0.0f;
                tags.Add("neutral");
                tags.Add("ignore");
                break;

            case "shout":
                impact = -0.4f;
                tags.Add("negative");
                tags.Add("noise");
                tags.Add("loud_noise");
                tags.Add("night_time");
                tags.Add("darkness");
                break;

            case "attack":
                impact = -0.7f;
                tags.Add("negative");
                tags.Add("threat_nearby");
                tags.Add("loud_noise");
                tags.Add("night_time");
                tags.Add("darkness");
                break;
        }

        npc.AddMemory(actionType, impact, tags);

        Debug.Log($"{npc.npcName} tutumu: {npc.GetDispositionLabel()} ({npc.GetOverallDisposition():F2})");

        if (QuestManager.Instance != null)
        {
            string npcShortName = npc.npcName.Contains("Ahmet") ? "Ahmet" :
                                  npc.npcName.Contains("Ayse") ? "Ayse" : "Mehmet";

            QuestManager.Instance.OnNPCVisited(npcShortName);

            switch (actionType)
            {
                case "greet":
                    QuestManager.Instance.OnNPCGreeted(npcShortName);
                    break;
                case "gift":
                    QuestManager.Instance.OnGiftGiven(npcShortName);
                    break;
                case "help":
                    QuestManager.Instance.OnHelpGiven(npcShortName);
                    break;
            }
        }

        if (actionType == "shout" || actionType == "attack")
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnBadAction();
            }
        }

        CloseInteractionMenu();
    }
}