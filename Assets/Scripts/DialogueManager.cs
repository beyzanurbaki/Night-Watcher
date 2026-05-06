using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject bubbleBackground;
    public float showDuration = 5f;

    private Coroutine currentRoutine;

    void Awake()
    {
        if (bubbleBackground != null)
            bubbleBackground.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";
    }

    public void ShowMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        Debug.Log("Text to display: " + message);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(DisplayRoutine(message));
    }

    private IEnumerator DisplayRoutine(string message)
    {
        if (bubbleBackground != null)
            bubbleBackground.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = message;

        yield return new WaitForSecondsRealtime(showDuration);

        if (bubbleBackground != null)
            bubbleBackground.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";

        currentRoutine = null;
    }
}