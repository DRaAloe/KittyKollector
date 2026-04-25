using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ClaudeManager : MonoBehaviour
{
    public static ClaudeManager Instance;

    [SerializeField] private string apiKey = "";

    private const string API_URL = "https://api.groq.com/openai/v1/chat/completions";
    private const string MODEL = "llama3-8b-8192";

    private List<Dictionary<string, string>> conversationHistory = new List<Dictionary<string, string>>();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void Ask(string prompt, System.Action<string> onReply)
    {
        conversationHistory.Add(new Dictionary<string, string>
        {
            { "role", "user" },
            { "content", prompt }
        });

        StartCoroutine(SendRequest(onReply));
    }

    public void ClearHistory() => conversationHistory.Clear();

    IEnumerator SendRequest(System.Action<string> onReply)
    {
        // Add system message at the top
        var messages = new List<Dictionary<string, string>>();
        messages.Add(new Dictionary<string, string>
        {
            { "role", "system" },
            { "content", "You are an AI built into a 2D platformer game. You control NPCs, generate dialogue, give hints, and respond to game events. Keep responses short and punchy." }
        });
        messages.AddRange(conversationHistory);

        var body = new Dictionary<string, object>
        {
            { "model", MODEL },
            { "max_tokens", 1024 },
            { "messages", messages }
        };

        string json = JsonConvert.SerializeObject(body);
        Debug.Log("Sending: " + json);

        byte[] raw = System.Text.Encoding.UTF8.GetBytes(json);

        using var req = new UnityWebRequest(API_URL, "POST");
        req.uploadHandler = new UploadHandlerRaw(raw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + req.downloadHandler.text);

            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(req.downloadHandler.text);
            var choices = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response["choices"].ToString());
            var message = JsonConvert.DeserializeObject<Dictionary<string, string>>(choices[0]["message"].ToString());
            string reply = message["content"];

            conversationHistory.Add(new Dictionary<string, string>
            {
                { "role", "assistant" },
                { "content", reply }
            });

            onReply?.Invoke(reply);
            Debug.Log("AI Reply: " + reply);
        }
        else
        {
            Debug.LogError("Groq error: " + req.error);
            Debug.LogError("Response body: " + req.downloadHandler.text);
        }
    }
}