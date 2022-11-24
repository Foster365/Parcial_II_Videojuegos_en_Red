using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class NetManager : MonoBehaviourPunCallbacks
{

    [SerializeField] TMP_InputField characterNickName;
    [SerializeField] TMP_InputField roomName;
    [SerializeField] TMP_InputField roomSize;
    [SerializeField] Button btnConnection;
    [SerializeField] TextMeshProUGUI connectionStatus;
    [SerializeField] TextMeshProUGUI playersCount;
    [SerializeField] InputFieldHandler inputFieldHandler;
    string playersMaxNumber = "2";
    string[] genericNicknames = { "Menem", "Chinchulancha", "SinNombre" };
    string genericNickName = "Carlos";
    bool isRoomCreated = false;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        btnConnection.interactable = false;

        connectionStatus.text = "Connecting to Master";

    }

    public override void OnConnectedToMaster()
    {
        btnConnection.interactable = false;
        PhotonNetwork.JoinLobby();

        connectionStatus.text = "Connecting to Lobby";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        connectionStatus.text = "Connection with master has failed, cause was: " + cause;
    }

    public override void OnJoinedLobby()
    {
        btnConnection.interactable = true;

        connectionStatus.text = "Connected to Lobby";
    }
    public override void OnLeftLobby()
    {
        base.OnLeftLobby();

        connectionStatus.text = "Disconnected from lobby";
    }

    public void Connect()
    {
        if (string.IsNullOrEmpty(roomName.text) || string.IsNullOrWhiteSpace(roomName.text)) return;
        if (string.IsNullOrEmpty(characterNickName.text) || string.IsNullOrWhiteSpace(characterNickName.text)) return;
        if (string.IsNullOrEmpty(roomSize.text) || string.IsNullOrWhiteSpace(roomSize.text)) return;

        PhotonNetwork.NickName = characterNickName.text;

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = byte.Parse(roomSize.text);
        options.IsOpen = true;
        options.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom(roomName.text, options, TypedLobby.Default);

        btnConnection.interactable = false;
    }

    public override void OnCreatedRoom()
    {
        isRoomCreated = true;

        connectionStatus.text = "Room " + inputFieldHandler.RoomName.text + " was created!";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {

        connectionStatus.text = "Failed to create room " + inputFieldHandler.RoomName.text;

        btnConnection.interactable = true;

    }

    public override void OnJoinedRoom()
    {
        connectionStatus.text = "Joined room";

        PhotonNetwork.LoadLevel("Full_Authority");

        Debug.Log("Max amount of players is: " + PhotonNetwork.CurrentRoom.MaxPlayers + ", and the current amount of players connected is : " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        connectionStatus.text = "Failed to join room " + inputFieldHandler.RoomName.text;
        btnConnection.interactable = true;
    }

}
