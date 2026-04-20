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

    void Update()
    {
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
                $"Ahmet Abi\n" +
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
                $"Ayse Teyze\n" +
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
                $"Mehmet Abi\n" +
                $"Tutum: {label} ({disposition:F2})\n" +
                $"Ani sayisi: {memoryCount}\n" +
                $"En guclu ani: {strongest}";
        }
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