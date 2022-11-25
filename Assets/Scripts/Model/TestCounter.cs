using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestCounter : MonoBehaviourPun
{
    [SerializeField] GameManager gameMgr;
    [SerializeField] TextMeshProUGUI gameTimer;
    float timeLeft = 200;
    private void Awake()
    {
        gameMgr = GetComponent<GameManager>();
    }
    private void Update()
    {
        //if (PhotonNetwork.CurrentRoom.PlayerCount > 2)
        //{
        //    Debug.Log("Entered on testCounter buenas buenas");
        //    UpdateGameTimer();
        //}

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

}
