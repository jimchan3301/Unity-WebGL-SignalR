using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text;

public class TestScript : MonoBehaviour
{
    public string signalRHubURL = "http://localhost:5000/mainhub";

    public string hubMethodAll = "SendPayloadAll";
    public string hubMethodCaller = "SendPayloadCaller";

    public string messageToSendAll = "Hello World All";
    public string messageToSendCaller = "Hello World Caller";

    public string statusText = "Awaiting Connection...";
    public string connectedText = "Connection Started";
    public string disconnectedText = "Connection Disconnected";

    private const string HANDLER_ALL = "ReceivePayloadAll";
    private const string HANDLER_CALLER = "ReceivePayloadCaller";

    // Binary data handlers
    private const string BINARY_HANDLER_ALL = "ReceiveBinaryAll";
    private const string BINARY_HANDLER_CALLER = "ReceiveBinaryCaller";
    private const string BINARY_METADATA_HANDLER_ALL = "ReceiveBinaryWithMetadataAll";
    private const string BINARY_METADATA_HANDLER_CALLER = "ReceiveBinaryWithMetadataCaller";

    private Text uiText;
    private string currentText = "";

    void Start()
    {
        uiText = GameObject.Find("Text").GetComponent<Text>();

        DisplayMessage(statusText);

        var signalR = new SignalR();
        signalR.Init(signalRHubURL);

        signalR.On(HANDLER_ALL, (string payload) =>
        {
            var json = JsonUtility.FromJson<JsonPayload>(payload);
            DisplayMessage($"{HANDLER_ALL}: {json.message}");
        });
        signalR.On(HANDLER_CALLER, (string payload) =>
        {
            var json = JsonUtility.FromJson<JsonPayload>(payload);
            DisplayMessage($"{HANDLER_CALLER}: {json.message}");
        });

        // Binary data handlers
        signalR.On(BINARY_HANDLER_ALL, (byte[] binaryData) =>
        {
            DisplayMessage($"{BINARY_HANDLER_ALL}: {Encoding.UTF8.GetString(binaryData)}");
        });
        signalR.On(BINARY_HANDLER_CALLER, (byte[] binaryData) =>
        {
            DisplayMessage($"{BINARY_HANDLER_CALLER}: {Encoding.UTF8.GetString(binaryData)}");
        });

        signalR.ConnectionStarted += (object sender, ConnectionEventArgs e) =>
        {
            Debug.Log($"Connected: {e.ConnectionId}");
            DisplayMessage(connectedText);

            var json1 = new JsonPayload
            {
                message = messageToSendAll
            };
            signalR.Invoke(hubMethodAll, JsonUtility.ToJson(json1));
            var json2 = new JsonPayload
            {
                message = messageToSendCaller
            };
            signalR.Invoke(hubMethodCaller, JsonUtility.ToJson(json2));

            // Send simple binary data to all clients
            signalR.InvokeBinary("SendBinaryAll", Encoding.UTF8.GetBytes(messageToSendAll));

            // Send simple binary data to caller only
            signalR.InvokeBinary("SendBinaryCaller", Encoding.UTF8.GetBytes(messageToSendCaller));
        };
        signalR.ConnectionClosed += (object sender, ConnectionEventArgs e) =>
        {
            Debug.Log($"Disconnected: {e.ConnectionId}");
            DisplayMessage(disconnectedText);
        };

        signalR.Connect();
    }

    void Update()
    {
        if (uiText.text != currentText)
        {
            StartCoroutine(RebuildLayout());
            currentText = uiText.text;
        }
    }

    void DisplayMessage(string message)
    {
        uiText.text += $"{message}\n";
    }

    IEnumerator RebuildLayout()
    {
        yield return null;

        LayoutRebuilder.MarkLayoutForRebuild(uiText.rectTransform);
    }

    [Serializable]
    public class JsonPayload
    {
        public string message;
    }
}
