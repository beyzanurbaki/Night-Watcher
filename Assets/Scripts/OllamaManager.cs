using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class OllamaManager : MonoBehaviour
{
    [Header("Ollama Ayarları")]
    [SerializeField] private string ollamaUrl = "http://localhost:11434/api/chat";
    // Eğer hala saçmalıyorsa modelName kısmını "llama3:8b-instruct-q2_K" yapabilirsin.
    [SerializeField] private string modelName = "phi3";

    public void SendMessageToNPC(string systemPrompt, string playerMessage, Action<string> onReply = null)
    {
        StartCoroutine(CallOllama(systemPrompt, playerMessage, onReply));
    }

    private IEnumerator CallOllama(string systemPrompt, string playerMessage, Action<string> onReply)
    {
        OllamaRequest requestData = new OllamaRequest
        {
            model = modelName,
            stream = false,
            // Phi-3'ün saçmalamasını engellemek için kritik ayarlar:
            options = new OllamaOptions
            {
                temperature = 0.5f, // Düşük yaratıcılık = Daha az hata
                num_predict = 50,   // En fazla 50 kelime üret (sonsuz döngüyü engeller)
                top_k = 40,         // Kelime havuzunu daralt
                top_p = 0.9f       // Tutarlılığı artır
            },
            messages = new OllamaRequestMessage[]
            {
                new OllamaRequestMessage { role = "system", content = systemPrompt },
                new OllamaRequestMessage { role = "user", content = playerMessage }
            }
        };

        string jsonBody = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Kart yük altındayken timeout olmaması için süreyi uzun tuttuk.
            request.timeout = 120;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                OllamaResponse responseData = JsonUtility.FromJson<OllamaResponse>(responseText);

                if (responseData != null && responseData.message != null)
                {
                    // NPC cevabını döndürür
                    onReply?.Invoke(responseData.message.content.Trim());
                }
                else
                {
                    Debug.LogError("Ollama JSON Parse Hatası: " + responseText);
                }
            }
            else
            {
                Debug.LogError("Ollama Bağlantı Hatası: " + request.error);
            }
        }
    }
}

#region Data Structures (JSON Serialization)
[Serializable]
public class OllamaRequest
{
    public string model;
    public bool stream;
    public OllamaRequestMessage[] messages;
    public OllamaOptions options;
}

[Serializable]
public class OllamaOptions
{
    public float temperature;
    public int num_predict;
    public int top_k;
    public float top_p;
}

[Serializable]
public class OllamaRequestMessage
{
    public string role;
    public string content;
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
#endregion