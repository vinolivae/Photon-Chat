using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient chatclient;
    private string selectedChannelName;

    [SerializeField] private string[] chanels;

    [Header("login objects")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private InputField idInput;
    [SerializeField] private Button loginButton;

    [Header("chat objects")]
    [SerializeField] private GameObject chatPanel;
    [SerializeField] private InputField messageInputField;
    [SerializeField] private Text currentChannelMessages;
    [SerializeField] private Button sendButton;

    #region unity methods
    private void Awake()
    {
        idInput.text = PlayerPrefs.GetString("USERNAME");
    }
    private void Start()
    {
        chatclient = new ChatClient(this);

        if(string.IsNullOrEmpty(idInput.text)) idInput.text = "user" + UnityEngine.Random.Range(1, 100);

        loginButton.onClick.AddListener(OnLoginButtonClicked);
        sendButton.onClick.AddListener(OnSendButtonClicked);

        loginPanel.SetActive(true);
        chatPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        chatclient.Service();
    }
    #endregion

    #region private methods

    private void ConnectToPhotonChat()
    {
        Debug.Log("connecting to photon chat");
        chatclient.AuthValues = new AuthenticationValues(idInput.text);
        ChatAppSettings chatSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatclient.ConnectUsingSettings(chatSettings);

        chatPanel.SetActive(true);
        loginPanel.SetActive(false);
    }
    private void SendDirectMessage(string inputLine)
    {
        chatclient.PublishMessage("general", inputLine);
    }
    private void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName)) return;

        ChatChannel chatChannel = null;
        bool found = chatclient.TryGetChannel(channelName, out chatChannel);
        if (!found) return;

        selectedChannelName = channelName;
        currentChannelMessages.text = chatChannel.ToStringMessages();
        Debug.Log("Show Channel: " + selectedChannelName);
    }
    #endregion

    #region button methods
    private void OnLoginButtonClicked()
    {
        var playerName = idInput.text;
        ConnectToPhotonChat();
        if(!playerName.Equals("")) PlayerPrefs.SetString("USERNAME", playerName);
    }
    private void OnSendButtonClicked()
    {
        Debug.Log("fui chamado");
        if(messageInputField != null)
        {
            Debug.Log("fui chamado dentro do if");
            SendDirectMessage(messageInputField.text);
            messageInputField.text = "";
        }
    }
    #endregion

    #region IChatClientListener Contract
    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnConnected()
    {
        Debug.Log("You have connected to the photon chat");
        chatclient.Subscribe(chanels);
    }

    public void OnDisconnected()
    {
        Debug.Log("You have disconnected from the photon chat");
        
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            msgs = string.Format("{0}{1}={2}, ", msgs, senders[i], messages[i]);
        }
        Debug.Log($"OnGetMessages: {channelName} ({senders}) > {msgs}");

        ShowChannel("general");
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (!string.IsNullOrEmpty(message.ToString()))
        {
            string[] splitNames = channelName.Split(new char[] { ':' });
            string senderName = splitNames[0];
            if(!sender.Equals(senderName, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"{sender}: {message}");
            }
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }
    #endregion
}
