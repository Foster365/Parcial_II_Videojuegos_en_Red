using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static LevelsManager;

public class GM : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI gameStartTimer;
    [SerializeField] TextMeshProUGUI gameTimer;
    float timeLeft = 200;
    int initTimer = 3;

    bool isGameOn = false;
    [SerializeField] bool isVictory = false;
    [SerializeField] bool isDefeat = false;

    #region Singleton

    public GM GameManagerInstance { get; private set; }

    #endregion;

    private void Awake()
    {
        if (GameManagerInstance != null && GameManagerInstance != this) Destroy(this);
        else GameManagerInstance = this;
    }
    private void Start()
    {

        gameTimer.enabled = false;
        gameStartTimer.enabled = false;

        if (PhotonNetwork.PlayerList.Length == 3) photonView.RPC("StartGameInitCountdown", RpcTarget.All);
        //if (photonView.IsMine) UpdateGameTimer();
        //photonView.RPC("StartGameTimer", RpcTarget.All);
        gameTimer.text = timeLeft.ToString();
    }

    private void Update()
    {
        if (isGameOn) UpdateGameTimer();//photonView.RPC("UpdateGameTimer", RpcTarget.All);
        CheckPlayerDisconnected();
        CheckVictory();
        CheckDefeat();
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
        isGameOn = true;
        gameStartTimer.text = "Game starts!";
        StartCoroutine(WaitToStartCoroutine());
    }

    IEnumerator WaitToStartCoroutine()
    {
        yield return new WaitForSeconds(2);
        gameStartTimer.enabled = false;
        StartGameTest();
    }

    IEnumerator WaitToSync()
    {
        yield return new WaitForSeconds(2);
    }

    void StartGameTest()
    {
        photonView.RPC("SetGameOnBoolean", RpcTarget.All, true);
    }
    [PunRPC]
    bool SetGameOnBoolean(bool isOn)
    {
        return isGameOn = isOn;
    }
    [PunRPC]
    void StartGameTimer()
    {
        UpdateGameTimer();
    }
    //[PunRPC]
    void UpdateGameTimer()
    {

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
    }

    void CheckVictory()
    {

        if (PhotonNetwork.PlayerList.Length > 0) photonView.RPC("LoadWinScene", RpcTarget.All);
    }

    void CheckDefeat()
    {

        if (PhotonNetwork.PlayerList.Length == 0 || timeLeft <= 0) photonView.RPC("LoadGameOverScene", RpcTarget.All);
    }

}
