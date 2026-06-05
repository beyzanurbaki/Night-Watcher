using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    public static TriggerManager Instance;

    [Header("All NPCs")]
    public NPCController[] allNPCs;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTriggerActivated.AddListener(HandleTrigger);
        }

        RefreshNPCList();
    }

    public void RefreshNPCList()
    {
        allNPCs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
    }

    public void HandleTrigger(string triggerType)
    {
        if (allNPCs == null || allNPCs.Length == 0)
            RefreshNPCList();

        Debug.Log($"Trigger activated: {triggerType}");

        // Translate trigger type to Turkish and display as UI notification
        if (UIManager.Instance != null)
        {
            string translatedMessage = TranslateTrigger(triggerType);
            if (!string.IsNullOrEmpty(translatedMessage))
            {
                UIManager.Instance.ShowEventNotification(translatedMessage);
            }
        }

        foreach (var npc in allNPCs)
        {
            if (npc != null)
                npc.ActivateTrigger(triggerType);
        }
    }

    private string TranslateTrigger(string triggerType)
    {
        string t = triggerType.ToLower();
        int day = TimeManager.Instance != null ? TimeManager.Instance.currentDay : 1;

        if (t.Contains("night_time")) return $"Gece {day} başladı!";
        if (t.Contains("darkness")) return "Karanlık her yeri kapladı.";
        if (t.Contains("night_patrol")) return "Gece devriyesi başladı!";
        if (t.Contains("daytime") || t.Contains("morning")) return $"Gün {day} başladı!";
        if (t.Contains("loud_noise") || t.Contains("noise")) return "Mahallede büyük bir gürültü duyuldu!";
        if (t.Contains("location_park") || t.Contains("park")) return "Parkın yakınlarından şüpheli sesler geliyor...";
        if (t.Contains("threat_nearby") || t.Contains("threat")) return "Yakınlarda tehlikeli bir durum var!";
        if (t.Contains("safe")) return "Etraf tekrar sakinleşti.";
        if (t.Contains("rain")) return "Yağmur yağmaya başladı.";
        if (t.Contains("quiet_night")) return "Sakin bir gece...";

        return null; // Return null to skip UI notification for unmapped/internal triggers
    }
}