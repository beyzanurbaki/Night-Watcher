using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
public class NPCController : MonoBehaviour
{
    [Header("NPC Identity & AI")]
    public string npcName = "NPC";
    public OllamaManager ollamaManager;
    public DialogueManager dialogueManager;
    private string modelId;
    private bool modelReady = false;
    public bool isThinking { get; private set; } = false;

    [Header("AI Settings")]
    public float aiTemperature = 0.3f;

    [Header("Personality & Memory")]
    public Personality personality = new Personality();
    public List<NPCMemory> memories = new List<NPCMemory>();
    public int maxMemories = 50;

    [Header("Behavior & Movement")]
    public float moveSpeed = 2f;
    public float detectionRange = 5f;
    
    [Header("Emotion UI")]
    public SpriteRenderer emotionIcon;
    public Sprite hostileSprite;
    public Sprite uneasySprite;
    public Sprite neutralSprite;
    public Sprite warmSprite;
    public Sprite friendlySprite;

    private Transform player;
    private Vector2 startPosition;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveDirection;
    private Vector2 lastMoveDirection;

    private void Awake()
    {
        if (dialogueManager == null)
            dialogueManager = GetComponentInChildren<DialogueManager>(true);
    }

    private IEnumerator Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPosition = transform.position;

        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
            player = playerObject.transform;

        if (ollamaManager != null)
        {
            modelId = ollamaManager.SanitizeModelName(npcName);
            string systemPrompt = GenerateSystemPrompt();

            Debug.Log($"<color=cyan>{npcName}</color> brain is being prepared...");

            isThinking = true;
            bool createSuccess = false;
            yield return StartCoroutine(
                ollamaManager.CreateNPCModel(modelId, systemPrompt, success => createSuccess = success)
            );
            isThinking = false;

            modelReady = createSuccess;

            if (modelReady)
                Debug.Log($"<color=cyan>{npcName}</color> brain is ready.");
            else
                Debug.LogError($"<color=red>{npcName}</color> brain could not be created.");
        }
    }

    private void Update()
    {
        UpdateEmotionDisplay();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        UpdateBehavior();
    }

    private void UpdateAnimation()
    {
        if (anim != null)
        {
            anim.SetFloat("Horizontal", moveDirection.x);
            anim.SetFloat("Vertical", moveDirection.y);
            anim.SetFloat("Speed", moveDirection.sqrMagnitude);

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                anim.SetFloat("LastHorizontal", moveDirection.x);
                anim.SetFloat("LastVertical", moveDirection.y);
            }
        }
    }

    #region AI Interaction
    public void InteractWithPlayer(string playerMessage)
    {
        if (ollamaManager == null || string.IsNullOrEmpty(modelId) || !modelReady)
        {
            Debug.LogWarning($"{npcName}: Model is not ready!");
            return;
        }

        string memoryContext = GetMemoryContextForAI();

        string finalPrompt =
            $"You are {npcName}.\n" +
            $"Your current mood: {GetDispositionLabel()}.\n" +
            $"Recent events: {memoryContext}.\n" +
            $"What happened: {playerMessage}\n" +
            $"Respond as {npcName} would. ONLY 1 sentence, max 5 words. No explanations.";

        Debug.Log($"<color=yellow>{npcName}</color> is thinking...");

        isThinking = true;
        ollamaManager.SendMessageToNPC(modelId, finalPrompt, (reply) =>
        {
            isThinking = false;
            string shortReply = LimitReplyByWords(reply, 5);

            if (dialogueManager != null)
                dialogueManager.ShowMessage(shortReply);

            Debug.Log($"<color=cyan>{npcName}</color>: {shortReply}");
        }, aiTemperature);
    }

    private string LimitReplyByWords(string reply, int maxWords = 5)
    {
        if (string.IsNullOrWhiteSpace(reply))
            return "";

        string cleaned = reply.Replace("\n", " ").Trim();
        string[] words = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (words.Length <= maxWords)
            return cleaned;

        return string.Join(" ", words, 0, maxWords).Trim();
    }

    private string GetMemoryContextForAI()
    {
        var strongMemories = memories
            .Where(m => Mathf.Abs(m.GetStrength()) > 0.1f)
            .OrderByDescending(m => Mathf.Abs(m.GetStrength()))
            .Take(3)
            .Select(m => m.eventType);

        return strongMemories.Any() ? string.Join(", ", strongMemories) : "No significant memories.";
    }

    private string GenerateSystemPrompt()
    {
        string cleanName = npcName.Replace("-", " ").Replace("_", " ");

        if (cleanName.Contains("Ayse"))
        {
            return "You are Aunt Ayse, a warm and cheerful old woman. " +
                   "You love chatting and always speak kindly. " +
                   "You call people dear. " +
                   "Example replies: 'So sweet, dear.', 'Bless you, dear.', 'How lovely, dear.' " +
                   "Rules: Reply only in English. One short sentence. Maximum 5 words.";
        }
        else if (cleanName.Contains("Ahmet"))
        {
            return "You are Uncle Ahmet, a grumpy and suspicious old man. " +
                   "You dislike noise and trust people slowly. " +
                   "You sound annoyed and blunt. " +
                   "Example replies: 'Leave me alone.', 'What now?', 'Go away.' " +
                   "Rules: Reply only in English. One short sentence. Maximum 5 words.";
        }
        else if (cleanName.Contains("Mehmet"))
        {
            return "You are Uncle Mehmet, a calm and serious old man. " +
                   "You are polite, careful, and formal. " +
                   "You sound measured and reserved. " +
                   "Example replies: 'Thank you kindly.', 'I appreciate this.', 'Very well then.' " +
                   "Rules: Reply only in English. One short sentence. Maximum 5 words.";
        }

        return $"You are {cleanName}. Reply only in English. One short sentence. Maximum 5 words.";
    }
    #endregion

    #region Trigger System
    public void ActivateTrigger(string triggerType, float impact = 0f, List<string> tags = null)
    {
        if (impact == 0f)
            impact = GetTriggerImpact(triggerType);

        if (tags == null)
            tags = new List<string> { triggerType };

        AddMemory(triggerType, impact, tags);

        if (memories.Count > 0)
            StartCoroutine(TemporaryBoostMemory(memories[memories.Count - 1], 1f, 5f));

        Debug.Log($"{npcName} received trigger: {triggerType} ({impact:F2})");

        if (UnityEngine.Random.value < 0.4f)
        {
            string triggerMessage = TriggerToAIMessage(triggerType);
            InteractWithPlayer(triggerMessage);
        }
    }

    private string TriggerToAIMessage(string triggerType)
    {
        string t = triggerType.ToLower();

        if (t.Contains("night_time")) return "It just became night.";
        if (t.Contains("darkness")) return "It is very dark now.";
        if (t.Contains("night_patrol")) return "The patrol has started.";
        if (t.Contains("loud_noise")) return "A loud noise happened.";
        if (t.Contains("threat")) return "There is danger nearby.";
        if (t.Contains("safe")) return "Things feel safe now.";
        if (t.Contains("rain")) return "It started raining.";
        if (t.Contains("morning")) return "Morning has arrived.";

        return $"Something happened: {triggerType}.";
    }

    private float GetTriggerImpact(string triggerType)
    {
        string t = triggerType.ToLower();

        if (t.Contains("attack") || t.Contains("threat") || t.Contains("noise") || t.Contains("dark"))
            return -0.25f;

        if (t.Contains("gift") || t.Contains("help") || t.Contains("social") || t.Contains("safe"))
            return 0.25f;

        return 0.10f;
    }
    #endregion

    #region Movement & Memory
    void UpdateBehavior()
    {
        if (player == null || rb == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < detectionRange)
        {
            string disposition = GetDispositionLabel();

            switch (disposition)
            {
                case "Friendly":
                    MoveTowards(player.position, moveSpeed);
                    break;
                case "Warm":
                    MoveTowards(player.position, moveSpeed * 0.5f);
                    break;
                case "Neutral":
                    StopMovement();
                    break;
                case "Uneasy":
                    MoveAway(player.position, moveSpeed * 0.5f);
                    break;
                case "Hostile":
                    MoveAway(player.position, moveSpeed);
                    break;
            }
        }
        else if (Vector2.Distance(transform.position, startPosition) > 0.1f)
        {
            MoveTowards(startPosition, moveSpeed * 0.3f);
        }
        else
        {
            StopMovement();
        }
    }

    void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        moveDirection = Vector2.zero;
    }

    void MoveTowards(Vector2 target, float speed)
    {
        moveDirection = (target - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    void MoveAway(Vector2 target, float speed)
    {
        moveDirection = ((Vector2)transform.position - target).normalized;
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    public void AddMemory(string eventType, float impact, List<string> tags = null)
    {
        NPCMemory newMemory = new NPCMemory(eventType, impact, tags);
        newMemory.decayRate = personality.GetDecayRate(impact);

        memories.Add(newMemory);

        if (memories.Count > maxMemories)
            memories.RemoveAt(0);
    }

    public float GetOverallDisposition()
    {
        return memories.Count == 0 ? 0f : memories.Sum(m => m.GetStrength());
    }

    public string GetDispositionLabel()
    {
        float disp = GetOverallDisposition();

        if (disp < -0.5f) return "Hostile";
        if (disp < -0.2f) return "Uneasy";
        if (disp > 0.5f) return "Friendly";
        if (disp > 0.2f) return "Warm";
        return "Neutral";
    }

    void UpdateEmotionDisplay()
    {
        if (emotionIcon == null) return;

        string disposition = GetDispositionLabel();

        switch (disposition)
        {
            case "Hostile":
                if (hostileSprite != null) emotionIcon.sprite = hostileSprite;
                // Kırmızı kor gibi yavaşça parlama efekti (Intensity between 1.5 and 3.5)
                float hostileGlow = Mathf.Lerp(1.5f, 3.5f, (Mathf.Sin(Time.time * 2f) + 1f) / 2f);
                emotionIcon.color = new Color(1f, 0.1f, 0.1f) * hostileGlow;
                break;
                
            case "Uneasy":
                if (uneasySprite != null) emotionIcon.sprite = uneasySprite;
                emotionIcon.color = new Color(1f, 0.5f, 0f) * 1.5f; // Turuncu, hafif parlak
                break;
                
            case "Friendly":
                if (friendlySprite != null) emotionIcon.sprite = friendlySprite;
                // Yumuşak yeşil/altın sarısı parlama efekti
                float friendlyGlow = Mathf.Lerp(1.2f, 2.5f, (Mathf.Sin(Time.time * 1.5f) + 1f) / 2f);
                emotionIcon.color = new Color(0.6f, 1f, 0.2f) * friendlyGlow; // Soft yeşil/altın
                break;
                
            case "Warm":
                if (warmSprite != null) emotionIcon.sprite = warmSprite;
                emotionIcon.color = new Color(0f, 1f, 1f) * 1.5f; // Camgöbeği (Cyan)
                break;
                
            case "Neutral":
            default:
                if (neutralSprite != null) emotionIcon.sprite = neutralSprite;
                emotionIcon.color = new Color(1f, 1f, 0f) * 1f; // Normal sarı
                break;
        }
    }

    IEnumerator TemporaryBoostMemory(NPCMemory memory, float boostAmount, float duration)
    {
        float original = memory.emotionalImpact;
        memory.emotionalImpact *= 2f;

        UpdateEmotionDisplay();

        yield return new WaitForSeconds(duration);

        memory.emotionalImpact = original;
        UpdateEmotionDisplay();
    }
    #endregion
}