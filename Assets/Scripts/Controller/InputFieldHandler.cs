using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldHandler : MonoBehaviourPun
{
    [SerializeField] TMP_InputField characterNickName;
    [SerializeField] TMP_InputField roomName;
    [SerializeField] TMP_InputField roomSize;
    public const string playerNamePrefKey = "Nickname_Pref_Key";
    public const string roomNamePrefKey = "Room_Pref_Key";
    public const string roomSizePrefKey = "Room_Size_Pref_Key";

    public TMP_InputField CharacterNickName { get => characterNickName; set => characterNickName = value; }
    public TMP_InputField RoomName { get => roomName; set => roomName = value; }
    public TMP_InputField RoomSize { get => roomSize; set => roomSize = value; }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetPlayerInputName();
            SetRoomInputName();

        }
    }

    public void HandleName(TMP_InputField textInputName, string defaultInputName)
    {

        string defaultName = string.Empty;
        TMP_InputField _inputField = textInputName;
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(defaultInputName))
            {
                defaultName = PlayerPrefs.GetString(defaultInputName);
                _inputField.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;

    }
    public void SetPlayerInputName()
    {
        PlayerPrefs.SetString(playerNamePrefKey, characterNickName.text);
        PhotonNetwork.NickName = characterNickName.text;
        // #Important
        if (string.IsNullOrEmpty(characterNickName.text))
        {
            return;
        }
        PlayerPrefs.SetString(playerNamePrefKey, characterNickName.text);
    }

    public void SetRoomInputName()
    {

        PlayerPrefs.SetString(roomNamePrefKey, roomName.text);
        // #Important
        if (string.IsNullOrEmpty(roomName.text))
        {
            return;
        }
        PlayerPrefs.SetString(roomNamePrefKey, roomName.text);
    }
    //public void SetRoomSizeInputName()
    //{
    //    //PlayerPrefs.SetString(roomSizePrefKey, roomSize.text);
    //    PhotonNetwork.CurrentRoom.MaxPlayers = byte.Parse(roomSize.text);
    //    // #Important
    //    if (string.IsNullOrEmpty(roomSize.text))
    //    {
    //        return;
    //    }
    //    //PlayerPrefs.SetString(roomSizePrefKey, roomSize.text);
    //}

}
