using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Zaman Ayarlari")]
    public float dayDuration = 60f;

    [Header("Durum")]
    public float currentTime = 0f;
    public int currentDay = 1;
    public bool isNightActive = false;

    [Header("Gece Efekti")]
    public GameObject darkOverlay;

    [Header("Events")]
    public UnityEvent<string> OnTriggerActivated;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= dayDuration / 2f && !isNightActive)
        {
            StartNight();
        }

        if (currentTime >= dayDuration)
        {
            StartNewDay();
        }
    }

    void StartNight()
    {
        isNightActive = true;
        Debug.Log($"Gece {currentDay} basladi!");

        OnTriggerActivated?.Invoke("night_time");

        Camera.main.backgroundColor = new Color(0.05f, 0.05f, 0.15f);

        // Karanlik overlay ac
        if (darkOverlay != null)
        {
            darkOverlay.SetActive(true);
        }
    }

    void StartNewDay()
    {
        isNightActive = false;
        currentTime = 0f;
        currentDay++;
        Debug.Log($"Gun {currentDay} basladi!");

        Camera.main.backgroundColor = new Color(0.4f, 0.6f, 0.8f);

        // Karanlik overlay kapat
        if (darkOverlay != null)
        {
            darkOverlay.SetActive(false);
        }
    }
}