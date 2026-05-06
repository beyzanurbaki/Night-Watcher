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

        foreach (var npc in allNPCs)
        {
            if (npc != null)
                npc.ActivateTrigger(triggerType);
        }
    }
}