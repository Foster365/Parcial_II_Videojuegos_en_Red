using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;

public class ChatPhoton : MonoBehaviour, IChatClientListener
{

    public TextMeshProUGUI content;
    public TMP_InputField inputField;
    ChatClient _chatClient;

    string _command = "w/";

    string _channel;
    private void Start()
    {
        _chatClient = new ChatClient(this);
        _chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion,
            new AuthenticationValues(PhotonNetwork.NickName));
    }

    private void Update()
    {
        _chatClient.Service();

        if (Input.GetKeyDown(KeyCode.K))
        {
            inputField.Select();
        }
    }

    public void ChatSendMessage()
    {
        var message = inputField.text;
        if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message)) return;
        string[] words = message.Split(' ');

        if(words.Length > 2 && words[0] == _command)
        {
            var target = words[1];
            foreach(var currPlayer in PhotonNetwork.PlayerList)
            {
                if (target == currPlayer.NickName)
                {
                    var currMessage = string.Join(" ", words, 2, words.Length - 2);
                    _chatClient.SendPrivateMessage(target, currMessage);
                    return;
                }
            }
            content.text += "<color=pink>" + "No existe gilazo" + "</color>" + "\n";
            inputField.text = "";
        }
        else
        {
            _chatClient.PublishMessage(_channel, message);
            inputField.text = "";
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnConnected()
    {
        content.text += "SI GATO" + "\n";
        _channel = PhotonNetwork.CurrentRoom.Name;
        _chatClient.Subscribe(_channel);
    }

    public void OnDisconnected()
    {
        content.text = "NO GATO" + "\n";
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            var currSender = senders[i];

            string color;
            if (PhotonNetwork.NickName == currSender)
            {
                color = "<color=blue>";
            }
            else
            {
                color = "<color=red>";
            }

            content.text += color + currSender + ": " + "</color>" + messages[i] + "\n";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {

        content.text += "<color=yellow>" + sender + ": " + "</color>" + message + "\n";
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            content.text += "Subscribe to" + channels[i] + "\n";
        }
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
}
