using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Zaman Ayarlarý")]
    public float dayDuration = 60f; // 1 dakika = 1 oyun günü (test için kýsa)

    [Header("Durum")]
    public float currentTime = 0f;
    public bool isNightActive = false;

    [Header("Events")]
    public UnityEvent<string> OnTriggerActivated;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        // Gece baþlýyor mu? (Günün yarýsý)
        if (currentTime >= dayDuration / 2f && !isNightActive)
        {
            StartNight();
        }

        // Yeni gün baþlýyor mu?
        if (currentTime >= dayDuration)
        {
            StartNewDay();
        }
    }

    void StartNight()
    {
        isNightActive = true;
        Debug.Log("Gece baþladý!");

        // Tetikleyiciyi yayýnla
        OnTriggerActivated?.Invoke("night_time");

        // Ekraný karart (opsiyonel)
        Camera.main.backgroundColor = new Color(0.1f, 0.1f, 0.2f);
    }

    void StartNewDay()
    {
        isNightActive = false;
        currentTime = 0f;
        Debug.Log("Yeni gün baþladý!");

        // Ekraný aydýnlat
        Camera.main.backgroundColor = new Color(0.3f, 0.4f, 0.6f);
    }
}