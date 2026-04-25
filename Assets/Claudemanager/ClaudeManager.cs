using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ClaudeManager : MonoBehaviour
{
    public static ClaudeManager Instance;

    [Header("API Settings")]
    [SerializeField] private string apiKey = "YOUR_API_KEY_HERE";

    private const string API_URL = "https://api.anthropic.com/v1/messages";
    private const string MODEL = "claude-sonnet-4-20250514";

    // Keeps conversation history so Claude remembers context
    private List<Message> conversationHistory = new List<Message>();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    // Call this from anywhere in your game
    public void Ask(string prompt, System.Action<string> onReply)
    {
        conversationHistory.Add(new Message { role = "user", content = prompt });
        StartCoroutine(SendRequest(onReply));
    }

    public void ClearHistory() => conversationHistory.Clear();

    IEnumerator SendRequest(System.Action<string> onReply)
    {
        var body = new RequestBody
        {
            model = MODEL,
            max_tokens = 1024,
            system = "You are an AI built into a 2D platformer game. You control NPCs, generate dialogue, give hints, and respond to game events. Keep responses short and relevant to the game.",
            messages = conversationHistory
        };

        string json = JsonConvert.SerializeObject(body);
        byte[] raw = System.Text.Encoding.UTF8.GetBytes(json);

        using var req = new UnityWebRequest(API_URL, "POST");
        req.uploadHandler = new UploadHandlerRaw(raw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("x-api-key", apiKey);
        req.SetRequestHeader("anthropic-version", "2023-06-01");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<ClaudeResponse>(req.downloadHandler.text);
            string reply = response.content[0].text;

            // Add Claude's reply to history so it remembers the conversation
            conversationHistory.Add(new Message { role = "assistant", content = reply });

            onReply?.Invoke(reply);
        }
        else
        {
            Debug.LogError("Claude error: " + req.error);
        }
    }

    // Data classes
    [System.Serializable] class RequestBody
    {
        public string model;
        public int max_tokens;
        public string system;
        public List<Message> messages;
    }

    [System.Serializable] public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable] class ClaudeResponse
    {
        public List<ContentBlock> content;
    }

    [System.Serializable] class ContentBlock
    {
        public string text;
    }
}