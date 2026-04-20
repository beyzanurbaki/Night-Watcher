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
    private Rigidbody2D rb;

    void Start()
    {
        GameObject playerObject = GameObject.Find("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateEmotionDisplay();
    }

    void FixedUpdate()
    {
        UpdateBehavior();
    }

    void UpdateBehavior()
    {
        if (player == null || rb == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

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
                    rb.linearVelocity = Vector2.zero;
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
            float distToStart = Vector2.Distance(transform.position, startPosition);

            if (distToStart > 0.1f)
            {
                MoveTowards(startPosition, moveSpeed * 0.3f);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    void MoveAway(Vector2 target, float speed)
    {
        Vector2 direction = ((Vector2)transform.position - target).normalized;
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
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
            if (memory.tags == null) continue;

            if (memory.tags.Contains(triggerType) && Mathf.Abs(memory.GetStrength()) > 0.02f)
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