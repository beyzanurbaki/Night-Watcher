using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // Canvas içindeki yazı objesi
    public GameObject bubbleBackground; // Arka plan görseli (isteğe bağlı)

    void Start()
    {
        // Başlangıçta gizle
        bubbleBackground.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return; // Boş mesaj gelirse hiçbir şey yapma

        Debug.Log("UI'da Gösterilecek: " + message);
        StopAllCoroutines(); // Önceki zamanlayıcıları durdur
        StartCoroutine(DisplayRoutine(message));
    }

    IEnumerator DisplayRoutine(string message)
    {
        bubbleBackground.SetActive(true);
        dialogueText.text = message;

        // Süreyi 10 saniyeye çıkaralım ki test ederken rahatça görebil
        yield return new WaitForSeconds(10f);

        bubbleBackground.SetActive(false);
        dialogueText.text = "";
    }
}