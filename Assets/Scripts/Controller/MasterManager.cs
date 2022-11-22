using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MasterManager : MonoBehaviourPunCallbacks
{
    GameManager gameMgr;
    Dictionary<Player, CharacterModel> charactersDictionary = new Dictionary<Player, CharacterModel>();
    Dictionary<CharacterModel, Player> clientsDictionary = new Dictionary<CharacterModel, Player>();
    static MasterManager instance;
    public static MasterManager Instance
    {
        get
        {
            return instance;
        }
    }

    public Dictionary<Player, CharacterModel> CharactersDictionary { get => charactersDictionary; set => charactersDictionary = value; }

    private void Awake()
    {
        if (instance != null) Destroy(this);
        else instance = this;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (charactersDictionary.ContainsKey(otherPlayer))
            {
                var character = charactersDictionary[otherPlayer];
                RemoveClient(otherPlayer);
                PhotonNetwork.Destroy(character.gameObject);
            }
        }
    }

    public Player GetClientFromModel(CharacterModel _charModel)
    {
        if (clientsDictionary.ContainsKey(_charModel))
        {
            return clientsDictionary[_charModel];
        }
        return null;
    }

    public CharacterModel[] GetAllClientModels()
    {
        var characterModels = new CharacterModel[charactersDictionary.Count];
        int count = 0;
        foreach (var character in charactersDictionary)
        {
            characterModels[count] = character.Value;
            count++;
        }
        return characterModels;
    }

    public void RemoveCharacterModel(CharacterModel _charModel)
    {
        if (clientsDictionary.ContainsKey(_charModel))
        {
            var client = clientsDictionary[_charModel];
            clientsDictionary.Remove(_charModel);
            charactersDictionary.Remove(client);
        }
    }
    public void RemoveClient(Player _client)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var characterModel = charactersDictionary[_client];
            charactersDictionary.Remove(_client);
            clientsDictionary.Remove(characterModel);
        }
    }

    #region RPC'S
    [PunRPC]
    public void RequestConnectPlayer(Player _client, string _clientPrefName)
    {
        GameObject playerGO = PhotonNetwork.Instantiate("Player_New", Vector3.zero, Quaternion.identity);
        var character = playerGO.GetComponent<CharacterModel>();
        charactersDictionary[_client] = character;
        clientsDictionary[character] = _client;
        if (gameMgr != null) gameMgr.SetManager(character);
        character.photonView.RPC("ActivateCamaraGIl", _client);
    }

    [PunRPC]
    public void RequestMove(Player _client, Vector3 _dir) //client: Player who reproduces this rpc. (MasterClient in this case)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];

            character.Move(_dir);
            character.LookDir(_dir);
        }
    }

    [PunRPC]
    public void RequestJump(Player _client)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];

            character.Jump();
            //character.LookAt(_dir);
        }
    }

    [PunRPC]
    public void RequestTriggerAnimation(Player _client, string _animName)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];
            character.GetComponent<Animator>().SetTrigger(_animName);
        }
    }

    [PunRPC]
    public void RequestBoolAnimation(Player _client, string _animName, bool _animBool)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];
            character.GetComponent<Animator>().SetBool(_animName, _animBool);
        }
    }

    [PunRPC]
    public void HandleRPC(string rpcMethodName, params object[] rpcParameters)
    {
        photonView.RPC(rpcMethodName, PhotonNetwork.MasterClient, rpcParameters);
    }
    #endregion
}
