using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    public GameObject interactionPanel;

    [Header("Memory Panel")]
    public GameObject memoryPanel;

    [Header("Interaction Rights")]
    public int maxInteractions = 2;
    public int remainingInteractions = 2;

    [Header("Warning")]
    public TextMeshProUGUI warningText;

    [Header("Event Notification")]
    public TextMeshProUGUI eventNotificationText;
    public GameObject eventNotificationPanel;
    public float eventNotificationDuration = 3f;

    private Coroutine eventNotificationCoroutine;
    private Queue<string> eventNotificationQueue = new Queue<string>();
    private bool isDisplayingNotification = false;
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
        memoryPanel.SetActive(true); // Hafıza paneli başlangıçta hep açık kalsın

        if (warningText != null)
            warningText.gameObject.SetActive(false);

        if (eventNotificationText != null)
        {
            eventNotificationText.text = "";
            eventNotificationText.gameObject.SetActive(false);
        }

        if (eventNotificationPanel != null)
            eventNotificationPanel.SetActive(false);
    }

    public void ToggleMemoryPanel()
    {
        bool isActive = memoryPanel.activeSelf;
        memoryPanel.SetActive(!isActive);
    }

    public void ResetInteractions()
    {
        remainingInteractions = maxInteractions;
        Debug.Log($"Interaction rights reset: {remainingInteractions}");
    }

    public void ShowInteractionMenu(GameObject npc)
    {
        if (remainingInteractions <= 0)
        {
            Debug.Log("No interactions left! Wait for the next day.");
            StartCoroutine(ShowWarning("No interactions left! Wait for the next day."));
            return;
        }

        currentNPC = npc;
        interactionPanel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log($"Menu opened: {npc.name} (Remaining: {remainingInteractions})");
    }

    IEnumerator ShowWarning(string message)
    {
        if (warningText == null) yield break;

        warningText.text = message;
        warningText.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(4f);

        warningText.gameObject.SetActive(false);
    }

    public void CloseInteractionMenu()
    {
        interactionPanel.SetActive(false);
        Time.timeScale = 1f;
        currentNPC = null;
        Debug.Log("Menu closed");
    }

    public void OnActionButton(string actionType)
    {
        if (currentNPC == null) return;

        NPCController npc = currentNPC.GetComponent<NPCController>();
        if (npc == null)
        {
            Debug.Log("ERROR: NPCController not found!");
            return;
        }

        if (actionType != "ignore")
        {
            remainingInteractions--;
            Debug.Log($"Interaction used. Remaining: {remainingInteractions}");
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

        Debug.Log($"{npc.npcName} disposition: {npc.GetDispositionLabel()} ({npc.GetOverallDisposition():F2})");

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
                QuestManager.Instance.OnBadAction();
        }

        string aiMessage = ActionToAIMessage(actionType);
        npc.InteractWithPlayer(aiMessage);

        CloseInteractionMenu();
    }

    private string ActionToAIMessage(string actionType)
    {
        switch (actionType)
        {
            case "greet": return "The player greeted you.";
            case "gift": return "The player gave you a gift.";
            case "help": return "The player helped you.";
            case "ignore": return "The player ignored you.";
            case "shout": return "The player shouted at you.";
            case "attack": return "The player attacked you.";
            default: return "The player is talking to you.";
        }
    }

    public void ShowEventNotification(string message)
    {
        if (eventNotificationText == null)
        {
            Debug.LogWarning("EventNotificationText reference is missing in UIManager!");
            return;
        }

        eventNotificationQueue.Enqueue(message);

        if (!isDisplayingNotification)
        {
            eventNotificationCoroutine = StartCoroutine(ProcessNotificationQueue());
        }
    }

    private IEnumerator ProcessNotificationQueue()
    {
        isDisplayingNotification = true;

        while (eventNotificationQueue.Count > 0)
        {
            string nextMessage = eventNotificationQueue.Dequeue();
            yield return StartCoroutine(FadeNotificationRoutine(nextMessage));
            yield return new WaitForSecondsRealtime(0.2f); // Short pause between notifications
        }

        isDisplayingNotification = false;
        eventNotificationCoroutine = null;
    }

    private IEnumerator FadeNotificationRoutine(string message)
    {
        eventNotificationText.gameObject.SetActive(true);
        eventNotificationText.text = message;
        
        if (eventNotificationPanel != null)
            eventNotificationPanel.SetActive(true);

        Color originalColor = eventNotificationText.color;
        
        // Fade In (0.5 seconds)
        float elapsed = 0f;
        float fadeDuration = 0.5f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            eventNotificationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        eventNotificationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        // Stay on screen
        yield return new WaitForSecondsRealtime(eventNotificationDuration);

        // Fade Out (0.5 seconds)
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsed / fadeDuration));
            eventNotificationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        eventNotificationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        if (eventNotificationPanel != null)
            eventNotificationPanel.SetActive(false);

        eventNotificationText.text = "";
        eventNotificationText.gameObject.SetActive(false);
        eventNotificationText.color = originalColor;
    }
}