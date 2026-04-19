using UnityEngine;
using TMPro;

public class DayDisplayUI : MonoBehaviour
{
    public TextMeshProUGUI dayText;

    void Update()
    {
        if (dayText == null || TimeManager.Instance == null) return;

        string zaman = TimeManager.Instance.isNightActive ? "Gece" : "Gunduz";
        dayText.text = $"Gun: {TimeManager.Instance.currentDay} - {zaman}";
    }
}