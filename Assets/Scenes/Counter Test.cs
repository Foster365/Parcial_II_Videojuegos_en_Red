using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CounterTest : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gameTimer;
    [SerializeField] TextMeshProUGUI btn;
    float timeLeft = 200;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    void UpdateGameTimer()
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
