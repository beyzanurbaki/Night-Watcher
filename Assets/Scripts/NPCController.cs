using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("NPC Bilgileri")]
    public string npcName = "NPC";

    [Header("Kisilik")]
    public Personality personality = new Personality();

    [Header("Hafiza")]
    public List<NPCMemory> memories = new List<NPCMemory>();
    public int maxMemories = 50;

    [Header("UI")]
    public SpriteRenderer emotionIcon;

    [Header("Davranis Ayarlari")]
    public float moveSpeed = 2f;
    public float detectionRange = 5f;

    private Transform player;
    private Vector2 startPosition;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        startPosition = transform.position;
    }

    void Update()
    {
        UpdateEmotionDisplay();
        UpdateBehavior();
    }

    void UpdateBehavior()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Oyuncu yakinsa tepki ver
        if (distance < detectionRange)
        {
            string disposition = GetDispositionLabel();

            switch (disposition)
            {
                case "Dostca":
                    MoveTowards(player.position, moveSpeed);
                    break;

                case "Sicak":
                    MoveTowards(player.position, moveSpeed * 0.5f);
                    break;

                case "Notr":
                    // Yerinde dur
                    break;

                case "Tedirgin":
                    MoveAway(player.position, moveSpeed * 0.5f);
                    break;

                case "Dusmanca":
                    MoveAway(player.position, moveSpeed);
                    break;
            }
        }
        else
        {
            // Oyuncu uzaktaysa baslangic noktasina don
            MoveTowards(startPosition, moveSpeed * 0.3f);
        }
    }

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    void MoveAway(Vector2 target, float speed)
    {
        Vector2 direction = ((Vector2)transform.position - target).normalized;
        Vector2 destination = (Vector2)transform.position + direction * 2f;
        transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    public void AddMemory(string eventType, float impact, List<string> tags = null)
    {
        NPCMemory newMemory = new NPCMemory(eventType, impact, tags);
        newMemory.decayRate = personality.GetDecayRate(impact);
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

    public void ActivateTrigger(string triggerType)
    {
        bool hasTriggeredMemory = false;

        foreach (var memory in memories)
        {
            if (memory.tags.Contains(triggerType) && Mathf.Abs(memory.GetStrength()) > 0.1f)
            {
                StartCoroutine(TemporaryBoostMemory(memory, 0.5f, 10f));
                hasTriggeredMemory = true;
            }
        }

        if (hasTriggeredMemory)
        {
            Debug.Log($"{npcName}: '{triggerType}' tetikleyicisi gecmis anilari canlandirdi!");
        }
    }

    IEnumerator TemporaryBoostMemory(NPCMemory memory, float boostAmount, float duration)
    {
        float originalImpact = memory.emotionalImpact;
        float originalTimestamp = memory.timestamp;

        memory.emotionalImpact = originalImpact * 2f;
        memory.timestamp = Time.time;

        UpdateEmotionDisplay();

        yield return new WaitForSeconds(duration);

        memory.emotionalImpact = originalImpact;
        memory.timestamp = originalTimestamp;

        UpdateEmotionDisplay();
    }
}