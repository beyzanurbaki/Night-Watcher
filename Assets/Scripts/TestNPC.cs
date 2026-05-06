using UnityEngine;

public class TestNPC : MonoBehaviour
{
    public OllamaManager ollamaManager;
    public DialogueManager dialogueManager; // DialogueManager'ı buraya bağla

    void Start()
    {
        string prompt = "ROLE: Aunt Ayşe. PERSONALITY: Warm, talkative. RULES: Speak ONLY in English, max 2 sentences.";

        // NPC etkileşimi başlar
        ollamaManager.SendMessageToNPC(prompt, "Hello Aunt Ayşe!", OnNPCReply);
    }

    void OnNPCReply(string reply)
    {
        // Konsol yerine artık UI'a gönderiyoruz
        dialogueManager.ShowMessage(reply);
    }
}