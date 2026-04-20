using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("Olay Ayarlari")]
    public float eventCheckInterval = 15f;
    private float eventTimer = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTriggerActivated.AddListener(OnTimeTrigger);
        }
    }

    void Update()
    {
        if (!TimeManager.Instance.isNightActive) return;

        eventTimer += Time.deltaTime;

        if (eventTimer >= eventCheckInterval)
        {
            eventTimer = 0f;
            SpawnRandomEvent();
        }
    }

    void OnTimeTrigger(string triggerType)
    {
        // Gece basladiginda olay zamanlayicisini sifirla
        if (triggerType == "night_time")
        {
            eventTimer = 0f;
        }
    }

    void SpawnRandomEvent()
    {
        int random = Random.Range(0, 4);

        switch (random)
        {
            case 0:
                Debug.Log("Mahallede gurultu duyuldu!");
                TriggerManager.Instance.HandleTrigger("loud_noise");
                break;

            case 1:
                Debug.Log("Parktan sesler geliyor!");
                TriggerManager.Instance.HandleTrigger("location_park");
                break;

            case 2:
                Debug.Log("Suphe cekici bi durum var!");
                TriggerManager.Instance.HandleTrigger("threat_nearby");
                break;

            case 3:
                // Sessiz gece, olay yok
                Debug.Log("Sakin bir gece...");
                break;
        }
    }
}