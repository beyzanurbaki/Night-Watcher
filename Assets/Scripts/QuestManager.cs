using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Görevler")]
    public bool helpAhmetQuest;
    public bool helpAyseQuest;

    void Awake()
    {
        Instance = this;
    }

    public void CompleteQuest(string questName)
    {
        switch (questName)
        {
            case "helpAhmet":
                helpAhmetQuest = true;
                Debug.Log("Ahmet'i yardim etme görevi tamamlandi!");
                break;

            case "helpAyse":
                helpAyseQuest = true;
                Debug.Log("Ayþe'yi yardim etme görevi tamamlandi!");
                break;
        }
    }
}