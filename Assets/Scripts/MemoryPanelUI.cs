using UnityEngine;
using TMPro;

public class MemoryPanelUI : MonoBehaviour
{
    [Header("NPC Referanslari")]
    public NPCController ahmetNPC;
    public NPCController ayseNPC;
    public NPCController mehmetNPC;

    [Header("UI Text Referanslari")]
    public TextMeshProUGUI ahmetInfoText;
    public TextMeshProUGUI ayseInfoText;
    public TextMeshProUGUI mehmetInfoText;
    public TextMeshProUGUI statsText; // FPS ve CPU değerlerini gösterecek olan Text

    [Header("Ollama Referans")]
    public OllamaManager ollamaManager;

    private float deltaTime = 0.0f;

    void Start()
    {
        if (ollamaManager == null)
        {
            ollamaManager = FindObjectOfType<OllamaManager>();
        }
    }

    void Update()
    {
        // FPS hesaplaması için geçen süreyi yumuşatarak alıyoruz
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        UpdatePanel();
    }

    void UpdatePanel()
    {
        // Ahmet bilgileri
        if (ahmetNPC != null && ahmetInfoText != null)
        {
            float disposition = ahmetNPC.GetOverallDisposition();
            string label = ahmetNPC.GetDispositionLabel();
            int memoryCount = ahmetNPC.memories.Count;
            string strongest = GetStrongestMemory(ahmetNPC);

            ahmetInfoText.text =
                $"Durum: {GetAIStatus(ahmetNPC)}\n" +
                $"Tutum: {label} ({disposition:F2})\n" +
                $"Ani sayisi: {memoryCount}\n" +
                $"En guclu ani: {strongest}";
        }

        if (ayseNPC != null && ayseInfoText != null)
        {
            float disposition = ayseNPC.GetOverallDisposition();
            string label = ayseNPC.GetDispositionLabel();
            int memoryCount = ayseNPC.memories.Count;
            string strongest = GetStrongestMemory(ayseNPC);

            ayseInfoText.text =
                $"Durum: {GetAIStatus(ayseNPC)}\n" +
                $"Tutum: {label} ({disposition:F2})\n" +
                $"Ani sayisi: {memoryCount}\n" +
                $"En guclu ani: {strongest}";
        }

        if (mehmetNPC != null && mehmetInfoText != null)
        {
            float disposition = mehmetNPC.GetOverallDisposition();
            string label = mehmetNPC.GetDispositionLabel();
            int memoryCount = mehmetNPC.memories.Count;
            string strongest = GetStrongestMemory(mehmetNPC);

            mehmetInfoText.text =
                $"Durum: {GetAIStatus(mehmetNPC)}\n" +
                $"Tutum: {label} ({disposition:F2})\n" +
                $"Ani sayisi: {memoryCount}\n" +
                $"En guclu ani: {strongest}";
        }

        // FPS ve CPU (ms) değerlerini arayüze yazdırıyoruz
        if (statsText != null)
        {
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            statsText.text = $"Graphics:    {fps:F1} FPS ({msec:F1}ms)\nCPU: main {msec:F1}ms";
        }
    }

    string GetAIStatus(NPCController npc)
    {
        if (npc.isThinking)
        {
            int dotsCount = (int)(Time.time * 3f) % 4; 
            string dots = new string('.', dotsCount);
            return $"Düşünüyor{dots}";
        }
        return "Hazır";
    }

    string GetStrongestMemory(NPCController npc)
    {
        if (npc.memories.Count == 0) return "Yok";

        NPCMemory strongest = npc.memories[0];
        float strongestValue = Mathf.Abs(strongest.GetStrength());

        foreach (var memory in npc.memories)
        {
            float value = Mathf.Abs(memory.GetStrength());
            if (value > strongestValue)
            {
                strongest = memory;
                strongestValue = value;
            }
        }

        return $"{strongest.eventType} ({strongest.GetStrength():F2})";
    }
}