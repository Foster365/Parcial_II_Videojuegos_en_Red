using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] float secondsToStartGame;
    bool isGameStarted;
    Bomb _bomb;
    public void SetManager(CharacterModel _charModel)
    {
        _charModel.SetCharacterGameManager = this;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int playersCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("Players count: " + playersCount);
        if (PhotonNetwork.IsMasterClient && !isGameStarted && playersCount > 2) //> mínimo de players, >= playerCount -1 (Mínimo de players, descontando al MasterClient)
        {
            Debug.Log("Starting game");
            isGameStarted = true;
            StartCoroutine(WaitToStart());
        }
    }

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
        Debug.Log("Game Starts!"); //Lo que se puede hacer acá es habilitar el movimiento
        //de los personajes, y empezar a correr el timer, etc.
    }
    public Bomb GetBomb
    {
        get
        {
            return _bomb;
        }
    }
}
