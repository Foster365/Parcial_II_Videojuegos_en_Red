using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
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
    bool isSprinting;
    float timeLeft;

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

    private void Start()
    {
        isSprinting = false;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            HadleGameTimer();
            WaitToSync();
            foreach (var c in charactersDictionary)
            {
                CharacterModel charModel = c.Value;
                CheckGround(charModel);
                //charModel.HandleCameraValue();
                //HandleAnims(dir);
                if (charactersMovementDirections.ContainsKey(charModel))
                {
                    var dir = charactersMovementDirections[charModel];
                    charModel.MovePlayer(dir.x, dir.z);
                }
            }
        }

    }
    IEnumerator WaitToSync()
    {
        yield return new WaitForSeconds(2);
        photonView.RPC("UpdateGameTimer", RpcTarget.Others);
    }

    void HandleAnims(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            //animator.SetBool("isWalking", true);
            //Debug.Log("aaanimator");
        }
        else
        {
            //animator.SetBool("isWalking", false);
        }
    }

    public void CheckGround(CharacterModel _character)
    {
        gdd = Physics.Raycast(_character.transform.position, -Vector3.up, .1f, 1 << LayerMask.NameToLayer("Ground")) ? true : false;//Physics.Raycast(transform.position, Vector3.down, (_character.playerHeight * 0.5f) + 0.2f, _character.whatIsGround);
        _character.Grounded = gdd;
        if (_character.Grounded) _character.jumpsRemaining = _character.maxJumpCount;

    }

    #region Clients/Models Management
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
    #endregion

    #region RPC'S [PunRPC]
    [PunRPC]
    void UpdateGameTimer()
    {
        if (gameMgr != null) gameMgr.TimeLeft = timeLeft;

    }
    void HadleGameTimer()
    {
        timeLeft -= Time.deltaTime;

        //GameTimerCalc(timeLeft);

    }
    //public void GameTimerCalc(float currentTime)
    //{
    //    currentTime += 1;

    //    var minutes = Mathf.FloorToInt(currentTime / 60);
    //    var seconds = Mathf.FloorToInt(currentTime % 60);

    //    _gameTimerUI.text = String.Format("{0:00}:{1:00} ", minutes, seconds);
    //    Debug.Log("Gametimer timer: " + _gameTimerUI.text);
    //}
    [PunRPC]
    public void SetCameraControllerRotation(Player _client)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var characterModel = charactersDictionary[_client];
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

        dash.Cam = character.gameObject.GetComponent<PlayerCameraController>();
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
                Debug.Log("Jump Command");
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
            Debug.Log("Start Crouch Command");
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

            Debug.Log("Stop Crouch Command");
            //MasterManager.Instance.HandleRPC("RequestStopCrouch", PhotonNetwork.LocalPlayer);
            transform.localScale = new Vector3(transform.localScale.x, character.StartYScale, transform.localScale.z);
        }
    }

    [PunRPC]
    public void RequestStartSlide(Player _client, float _horizontalInput, float _verticalInput)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];
            if (_horizontalInput != 0 || _verticalInput != 0)
            {
                Debug.Log("Start Slide Command");
                character.CharacterSlideMovement.StartSlide();
            }
        }
    }

    [PunRPC]
    public void RequestStopSlide(Player _client)
    {
        if (charactersDictionary.ContainsKey(_client))
        {
            var character = charactersDictionary[_client];
            if (character.sliding)
            {
                Debug.Log("Stop Slide Command");
                character.CharacterSlideMovement.StopSlide();
            }
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
