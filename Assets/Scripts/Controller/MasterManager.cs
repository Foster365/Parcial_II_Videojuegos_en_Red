using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MasterManager : MonoBehaviourPunCallbacks
{
    GameManager gameMgr;
    Dictionary<Player, CharacterModel> charactersDictionary = new Dictionary<Player, CharacterModel>();
    Dictionary<CharacterModel, Player> clientsDictionary = new Dictionary<CharacterModel, Player>();
    Dictionary<CharacterModel, Vector3> charactersMovementDirections = new Dictionary<CharacterModel, Vector3>();
    static MasterManager instance;
    Vector3 movementDir;
    bool gdd;

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

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            foreach (var c in charactersDictionary)
            {
                CharacterModel charModel = c.Value;
                CheckGround(charModel);
                if (charactersMovementDirections.ContainsKey(charModel))
                {
                    var dir = charactersMovementDirections[charModel];
                    charModel.MovePlayer(dir.x, dir.z);
                }
            }
        }

    }


    [PunRPC]
    public void SetCharacterMovementDirection(Player _client, Vector3 _dir)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var characterModel = charactersDictionary[_client];
            charactersMovementDirections[characterModel] = _dir;
        }
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

    public void CheckGround(CharacterModel _character)
    {
        gdd = Physics.Raycast(_character.transform.position, -Vector3.up, .1f, 1 << LayerMask.NameToLayer("Ground")) ? true : false;//Physics.Raycast(transform.position, Vector3.down, (_character.playerHeight * 0.5f) + 0.2f, _character.whatIsGround);
        _character.Grounded = gdd;
        if (_character.Grounded) _character.jumpsRemaining = _character.maxJumpCount;

    }

    #region RPC'S
    [PunRPC]
    public void RequestConnectPlayer(Player _client, string _clientPrefName, Vector3 _pos, Quaternion _rot)
    {
        GameObject playerGO = PhotonNetwork.Instantiate(_clientPrefName, _pos, _rot);
        var character = playerGO.GetComponent<CharacterModel>();
        charactersDictionary[_client] = character;
        clientsDictionary[character] = _client;
        if (gameMgr != null) gameMgr.SetManager(character);
        character.photonView.RPC("ActivateCamaraGIl", _client);
        DashMovement dash = character.gameObject.GetComponent<DashMovement>();

        dash.Cam = character.gameObject.GetComponent<PlayerCameraController>();//transform;
        Debug.Log("DASH COMP: " + dash);
        //dash.photonView.RPC("SetCameraMovementGameObjectValue", _client);
        Debug.Log("Cam: " + dash.Cam);
    }

    [PunRPC]
    public void RequestMovementDir(Player _client, Vector3 _dir)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];

            movementDir = _dir;

        }
    }

    [PunRPC]
    public void RequestMove(Player _client, float _horizontalInput, float _verticalInput) //client: Player who reproduces this rpc. (MasterClient in this case)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];
            character.MovePlayer(_horizontalInput, _verticalInput);
        }
    }

    [PunRPC]
    public void RequestJump(Player _client)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];
            if (character.ReadyToJump && character.Grounded && (character.jumpsRemaining > 0))
            {
                character.ReadyToJump = false;

                character.Jump();

                character.JumpResetHandler();
                character.jumpsRemaining -= 1;
            }
        }
    }

    [PunRPC]
    public void RequestStartCrouch(Player _client)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];
            transform.localScale = new Vector3(transform.localScale.x, character.crouchYScale, transform.localScale.z);
            character.Rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
    }

    [PunRPC]
    public void RequestStopCrouch(Player _client)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];

            MasterManager.Instance.HandleRPC("RequestStopCrouch", PhotonNetwork.LocalPlayer);
            transform.localScale = new Vector3(transform.localScale.x, character.StartYScale, transform.localScale.z);
        }
    }

    [PunRPC]
    public void RequestStartSlide(Player _client, float _horizontalInput, float _verticalInput)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];
            Debug.Log("Horiz: " + _horizontalInput + "Vertic: " + _verticalInput + "Character Slide: " + character.CharacterSlideMovement);
            if (_horizontalInput != 0 || _verticalInput != 0) character.CharacterSlideMovement.StartSlide();
        }
    }

    [PunRPC]
    public void RequestStopSlide(Player _client)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];
            if (character.sliding) character.CharacterSlideMovement.StopSlide();
        }
    }

    [PunRPC]
    public void RequestDash(Player _client)
    {
        if (CharactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];
            character.CharacterDash.Dash();

            if (character.CharacterDash.DashCdTimer > 0)
                character.CharacterDash.DashCdTimer -= Time.deltaTime;
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
