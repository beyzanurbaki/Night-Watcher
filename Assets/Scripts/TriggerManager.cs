using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    public static TriggerManager Instance;

    [Header("Tum NPC'ler")]
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

        allNPCs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
    }

    void HandleTrigger(string triggerType)
    {
        Debug.Log($"Tetikleyici Aktif: {triggerType}");

        foreach (var npc in allNPCs)
        {
            npc.ActivateTrigger(triggerType);
        }
    }
}