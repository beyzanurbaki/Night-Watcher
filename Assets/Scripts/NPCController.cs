using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("NPC Bilgileri")]
    public string npcName = "NPC";

    [Header("Kisilik")]
    public Personality personality = new Personality();  // EKLENDI

    [Header("Hafiza")]
    public List<NPCMemory> memories = new List<NPCMemory>();
    public int maxMemories = 50;

    [Header("UI")]
    public SpriteRenderer emotionIcon;

    void Update()
    {
        UpdateEmotionDisplay();
    }

    public void AddMemory(string eventType, float impact, List<string> tags = null)
    {
        NPCMemory newMemory = new NPCMemory(eventType, impact, tags);

        // Kisillige gore decay rate ayarla
        newMemory.decayRate = personality.GetDecayRate(impact);  // EKLENDI

        memories.Add(newMemory);

        Debug.Log($"{npcName}: Yeni ani eklendi - {eventType} ({impact:F1}) - Decay: {newMemory.decayRate:F4}");
        Debug.Log($"{npcName}: Toplam ani sayisi: {memories.Count}");

        if (memories.Count > maxMemories)
        {
            memories.RemoveAt(0);
        }
    }

    public float GetOverallDisposition()
    {
        if (memories.Count == 0) return 0f;

        float total = 0f;
        foreach (var memory in memories)
        {
            total += memory.GetStrength();
        }

        return total;
    }

    public string GetDispositionLabel()
    {
        float disposition = GetOverallDisposition();

        if (disposition < -0.5f) return "Dusmanca";
        if (disposition < -0.2f) return "Tedirgin";
        if (disposition > 0.5f) return "Dostca";
        if (disposition > 0.2f) return "Sicak";
        return "Notr";
    }

    void UpdateEmotionDisplay()
    {
        if (emotionIcon == null) return;

        float disposition = GetOverallDisposition();

        if (disposition < -0.5f)
            emotionIcon.color = Color.red;
        else if (disposition < -0.2f)
            emotionIcon.color = new Color(1f, 0.5f, 0f);
        else if (disposition > 0.5f)
            emotionIcon.color = Color.green;
        else if (disposition > 0.2f)
            emotionIcon.color = Color.cyan;
        else
            emotionIcon.color = Color.yellow;
    }
}