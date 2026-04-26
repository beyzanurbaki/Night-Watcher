using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Puan")]
    public int totalScore = 0;

    [Header("Gorev Durumu")]
    public bool[] questCompleted = new bool[7];
    public string[] questDescriptions = new string[7];
    public int[] questRewards = new int[7];

    [Header("Gece Takibi")]
    public List<string> greetedNPCs = new List<string>();
    public List<string> visitedNPCs = new List<string>();
    public bool nightEventHandled = false;
    public bool didSomethingBad = false;

    [Header("UI")]
    public TextMeshProUGUI questText;
    public TextMeshProUGUI scoreText;

    [Header("NPC Referanslari")]
    public NPCController ahmetNPC;
    public NPCController ayseNPC;
    public NPCController mehmetNPC;

    void Awake()
    {
        Instance = this;
        SetupQuests();
    }

    void SetupQuests()
    {
        questDescriptions[0] = "En az 2 NPC'yi selamla";
        questDescriptions[1] = "Ahmet Abi'ye hediye ver ve baska birini selamla";
        questDescriptions[2] = "Gurultu olayina mudahale et ve bir NPC'yi sakinlestir";
        questDescriptions[3] = "Ayse'ye veya Mehmet'e yardim et (birini sec)";
        questDescriptions[4] = "En dusuk tutumlu NPC'yi iyilestir";
        questDescriptions[5] = "Alarm olayina mudahale et ve kimseye kotu davranma";
        questDescriptions[6] = "Tum NPC'ler en az Sicak tutumda olsun";

        questRewards[0] = 15;
        questRewards[1] = 20;
        questRewards[2] = 25;
        questRewards[3] = 20;
        questRewards[4] = 15;
        questRewards[5] = 30;
        questRewards[6] = 50;
    }

    void Update()
    {
        UpdateQuestUI();
        CheckActiveQuest();
    }

    void CheckActiveQuest()
    {
        if (TimeManager.Instance == null) return;

        int night = TimeManager.Instance.currentDay;

        switch (night)
        {
            case 1: CheckQuest1(); break;
            case 2: CheckQuest2(); break;
            case 3: CheckQuest3(); break;
            case 4: CheckQuest4(); break;
            case 5: CheckQuest5(); break;
            case 6: CheckQuest6(); break;
            case 7: CheckQuest7(); break;
        }
    }

    // Gece 1: En az 2 NPC'yi selamla
    void CheckQuest1()
    {
        if (questCompleted[0]) return;
        if (greetedNPCs.Count >= 2)
        {
            CompleteQuest(0);
        }
    }

    // Gece 2: Ahmet'e hediye ver VE baska birini selamla
    void CheckQuest2()
    {
        // Gift ve greet ayri ayri UIManager'dan tetiklenir
    }

    // Gece 3: Gurultu olayina mudahale et VE bir NPC'yi sakinlestir
    void CheckQuest3()
    {
        // nightEventHandled + greet/gift yapilmis mi?
        if (questCompleted[2]) return;
        if (nightEventHandled && greetedNPCs.Count >= 1)
        {
            CompleteQuest(2);
        }
    }

    // Gece 4: Ayse'ye VEYA Mehmet'e yardim et
    void CheckQuest4()
    {
        // UIManager'dan tetiklenir
    }

    // Gece 5: En dusuk tutumlu NPC'yi iyilestir
    void CheckQuest5()
    {
        if (questCompleted[4]) return;

        NPCController weakest = GetWeakestNPC();
        if (weakest == null) return;

        string weakestShort = weakest.npcName.Contains("Ahmet") ? "Ahmet" :
                              weakest.npcName.Contains("Ayse") ? "Ayse" : "Mehmet";

        if (greetedNPCs.Contains(weakestShort) || visitedNPCs.Contains(weakestShort))
        {
            if (weakest.GetOverallDisposition() > -0.2f)
            {
                CompleteQuest(4);
            }
        }
    }

    // Gece 6: Alarm olayina mudahale et VE kimseye kotu davranma
    void CheckQuest6()
    {
        if (questCompleted[5]) return;
        if (nightEventHandled && !didSomethingBad)
        {
            CompleteQuest(5);
        }
    }

    // Gece 7: Tum NPC'ler en az Sicak tutumda olsun
    void CheckQuest7()
    {
        if (questCompleted[6]) return;
        if (ahmetNPC == null || ayseNPC == null || mehmetNPC == null) return;

        string ahmetTutum = ahmetNPC.GetDispositionLabel();
        string ayseTutum = ayseNPC.GetDispositionLabel();
        string mehmetTutum = mehmetNPC.GetDispositionLabel();

        bool ahmetOK = ahmetTutum == "Sicak" || ahmetTutum == "Dostca";
        bool ayseOK = ayseTutum == "Sicak" || ayseTutum == "Dostca";
        bool mehmetOK = mehmetTutum == "Sicak" || mehmetTutum == "Dostca";

        if (ahmetOK && ayseOK && mehmetOK)
        {
            CompleteQuest(6);
        }
    }

    NPCController GetWeakestNPC()
    {
        NPCController weakest = ahmetNPC;
        float weakestScore = ahmetNPC.GetOverallDisposition();

        if (ayseNPC.GetOverallDisposition() < weakestScore)
        {
            weakest = ayseNPC;
            weakestScore = ayseNPC.GetOverallDisposition();
        }

        if (mehmetNPC.GetOverallDisposition() < weakestScore)
        {
            weakest = mehmetNPC;
        }

        return weakest;
    }

    void CompleteQuest(int questIndex)
    {
        if (questCompleted[questIndex]) return;

        questCompleted[questIndex] = true;
        totalScore += questRewards[questIndex];

        Debug.Log($"GOREV TAMAMLANDI: {questDescriptions[questIndex]} (+{questRewards[questIndex]} puan)");
        Debug.Log($"Toplam Puan: {totalScore}");
    }

    // Disaridan cagrilan fonksiyonlar
    public void OnNPCGreeted(string npcName)
    {
        if (!greetedNPCs.Contains(npcName))
        {
            greetedNPCs.Add(npcName);
        }
    }

    public void OnNPCVisited(string npcName)
    {
        if (!visitedNPCs.Contains(npcName))
        {
            visitedNPCs.Add(npcName);
        }
    }

    public void OnGiftGiven(string npcName)
    {
        int night = TimeManager.Instance.currentDay;

        if (night == 2 && npcName == "Ahmet" && greetedNPCs.Count >= 1 && !questCompleted[1])
        {
            CompleteQuest(1);
        }
    }

    public void OnHelpGiven(string npcName)
    {
        int night = TimeManager.Instance.currentDay;

        if (night == 4 && (npcName == "Ayse" || npcName == "Mehmet") && !questCompleted[3])
        {
            CompleteQuest(3);
        }
    }

    public void OnBadAction()
    {
        didSomethingBad = true;
    }

    public void OnNoiseEventHandled()
    {
        nightEventHandled = true;

        int night = TimeManager.Instance.currentDay;
        if (night == 3)
        {
            // Quest 3 kontrolu Update'de yapilir
        }
    }

    public void OnAlarmEventHandled()
    {
        nightEventHandled = true;

        int night = TimeManager.Instance.currentDay;
        if (night == 6)
        {
            // Quest 6 kontrolu Update'de yapilir
        }
    }

    public void OnNewNight()
    {
        greetedNPCs.Clear();
        visitedNPCs.Clear();
        nightEventHandled = false;
        didSomethingBad = false;
    }

    void UpdateQuestUI()
    {
        if (TimeManager.Instance == null) return;

        int night = TimeManager.Instance.currentDay;

        if (scoreText != null)
        {
            scoreText.text = $"Puan: {totalScore}";
        }

        if (questText != null)
        {
            if (night >= 1 && night <= 7)
            {
                int questIndex = night - 1;
                string status = questCompleted[questIndex] ? "(TAMAMLANDI)" : "(Devam Ediyor)";
                int kalan = UIManager.Instance != null ? UIManager.Instance.remainingInteractions : 0;
                questText.text = $"{night}. Gorev:\n{questDescriptions[questIndex]}\n{status}\nKalan Hak: {kalan}";
            }
        }
    }
}