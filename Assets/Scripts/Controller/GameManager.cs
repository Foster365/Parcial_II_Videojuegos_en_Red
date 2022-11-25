using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] float secondsToStartGame;
    [SerializeField] TextMeshProUGUI gameStartTimer;
    [SerializeField] TextMeshProUGUI gameTimer;
    float timeLeft = 200;
    int initTimer = 3;
    bool isGameStarted;
    bool isVictory = false, isDefeat = false;
    Bomb _bomb;

    public Bomb GetBomb
    {
        get
        {
            return _bomb;
        }
    }

    public bool IsGameStarted { get => isGameStarted; set => isGameStarted = value; }
    public TextMeshProUGUI GameTimer { get => gameTimer; set => gameTimer = value; }
    public float TimeLeft { get => timeLeft; set => timeLeft = value; }

    public void SetManager(CharacterModel _charModel)
    {
        _charModel.SetCharacterGameManager = this;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int playersCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (!isGameStarted && playersCount > 2) //> mínimo de players, >= playerCount -1 (Mínimo de players, descontando al MasterClient)
        {
            Debug.Log("Starting game");
            isGameStarted = true;
            //StartCoroutine(WaitToStart());
        }
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 2 && !PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Init counter game manager");
            UpdateGameTimer();

            //photonView.RPC("UpdateGameTimer", PhotonNetwork.MasterClient, RpcTarget.Others);
            //if (isGameStarted) UpdateGameTimer();//photonView.RPC("UpdateGameTimer", RpcTarget.All);


            //CheckVictory();
            //CheckDefeat();
        }
        else CheckPlayerDisconnected();
    }

    void CheckPlayerDisconnected()
    {
        if (!PhotonNetwork.InRoom && !PhotonNetwork.IsConnected)
        {
            Debug.Log("Quitting");
            Application.Quit();
        }
    }

    [PunRPC]
    void StartGameInitCountdown()
    {
        StartCoroutine(InitCountdown());
    }

    IEnumerator InitCountdown()
    {
        while (initTimer > 0)
        {
            gameStartTimer.enabled = true;
            gameStartTimer.text = "Game starts in " + initTimer.ToString();
            yield return new WaitForSeconds(1f);
            initTimer--;
        }
        isGameStarted = true;
        gameStartTimer.text = "Game starts!";
        StartCoroutine(WaitToStartCoroutine());
    }

    IEnumerator WaitToStartCoroutine()
    {
        yield return new WaitForSeconds(2);
        gameStartTimer.enabled = false;
    }

    public void UpdateGameTimer()
    {
        Debug.Log("Ok to run timer");
        gameTimer.enabled = true;
        timeLeft -= Time.deltaTime;

        HandleGameTimer(timeLeft);

    }
    public void HandleGameTimer(float currentTime)
    {
        currentTime += 1;

        var minutes = Mathf.FloorToInt(currentTime / 60);
        var seconds = Mathf.FloorToInt(currentTime % 60);

        gameTimer.text = String.Format("{0:00}:{1:00} ", minutes, seconds);
        Debug.Log("Gametimer timer: " + gameTimer.text);
    }


    #region Start Game. PARA SETEAR BOMBA
    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(secondsToStartGame);
        StartGame();
    }
    void StartGame()
    {
        var obj = PhotonNetwork.Instantiate("Bomb", Vector3.zero, Quaternion.identity);
        var bomb = obj.GetComponent<Bomb>();
        _bomb = bomb;
        _bomb.SetRandomTarget();
        Debug.Log("Game Starts!");
    }
    #endregion

    #region RPC's

    [PunRPC]
    void LoadWinScene()
    {
        SceneManager.LoadScene("Win");
    }

    [PunRPC]
    void LoadGameOverScene()
    {
        SceneManager.LoadScene("Game_Over");
    }

    #endregion
}
