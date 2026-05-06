using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class OllamaManager : MonoBehaviour
{
    [Header("Ollama Server Settings")]
    [SerializeField] private string baseUrl = "http://localhost:11434/api";
    [SerializeField] private string baseModel = "phi3";
    [SerializeField] private float temperature = 0.1f;
    [SerializeField] private int maxPredict = 12;

    public IEnumerator CreateNPCModel(string npcModelName, string systemPrompt)
    {
        string safeModelName = SanitizeModelName(npcModelName);

        OllamaCreateRequest requestData = new OllamaCreateRequest
        {
            model = safeModelName,
            @from = baseModel,
            system = systemPrompt,
            stream = false
        };

        string jsonBody = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/create", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 120;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"<color=green>Ollama:</color> {safeModelName} model created.");
            }
            else
            {
                Debug.LogError($"Model Creation Error ({safeModelName}): {request.downloadHandler.text}");
            }
        }
    }

    public void SendMessageToNPC(string npcModelName, string playerMessage, Action<string> onReply = null)
    {
        if (string.IsNullOrWhiteSpace(playerMessage))
            return;

        string safeModelName = SanitizeModelName(npcModelName);
        StartCoroutine(CallOllama(safeModelName, playerMessage, onReply));
    }

    private IEnumerator CallOllama(string modelName, string playerMessage, Action<string> onReply)
    {
        OllamaChatRequest requestData = new OllamaChatRequest
        {
            model = modelName,
            messages = new OllamaRequestMessage[]
            {
                new OllamaRequestMessage
                {
                    role = "user",
                    content = playerMessage
                }
            },
            stream = false,
            options = new OllamaOptions
            {
                temperature = temperature,
                num_predict = maxPredict
            }
        };

        string jsonBody = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/chat", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 60;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                OllamaResponse responseData = JsonUtility.FromJson<OllamaResponse>(request.downloadHandler.text);

                if (responseData != null && responseData.message != null)
                {
                    onReply?.Invoke(responseData.message.content.Trim());
                }
                else
                {
                    Debug.LogError($"Ollama Parse Error ({modelName}): {request.downloadHandler.text}");
                }
            }
            else
            {
                Debug.LogError($"Ollama Chat Error ({modelName}): {request.downloadHandler.text}");
            }
        }
    }

    public string SanitizeModelName(string rawName)
    {
        string clean = rawName.ToLower().Trim();
        StringBuilder sb = new StringBuilder();

        foreach (char c in clean)
        {
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '-')
                sb.Append(c);
            else if (c == ' ' || c == '_')
                sb.Append('-');
        }

        return sb.ToString();
    }
}

[Serializable]
public class OllamaCreateRequest
{
    public string model;
    public string @from;
    public string system;
    public bool stream;
}

[Serializable]
public class OllamaChatRequest
{
    public string model;
    public OllamaRequestMessage[] messages;
    public bool stream;
    public OllamaOptions options;
}

[Serializable]
public class OllamaRequestMessage
{
    public string role;
    public string content;
}

[Serializable]
public class OllamaOptions
{
    public float temperature;
    public int num_predict;
}

[Serializable]
public class OllamaResponse
{
    public OllamaMessage message;
}

[Serializable]
public class OllamaMessage
{
    public string role;
    public string content;
}