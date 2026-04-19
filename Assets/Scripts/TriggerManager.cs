using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    public static TriggerManager Instance;

    [Header("Tüm NPC'ler")]
    public NPCController[] allNPCs;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // TimeManager'ýn event'ine abone ol
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTriggerActivated.AddListener(HandleTrigger);
        }

        // Sahnedeki tüm NPC'leri otomatik bul
        allNPCs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
    }

    void HandleTrigger(string triggerType)
    {
        Debug.Log($"Tetikleyici Aktif: {triggerType}");

        // Tüm NPC'lere tetikleyiciyi gönder
        foreach (var npc in allNPCs)
        {
            npc.ActivateTrigger(triggerType);
        }
    }
}