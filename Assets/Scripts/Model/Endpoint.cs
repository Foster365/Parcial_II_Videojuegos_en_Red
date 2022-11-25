using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Endpoint : MonoBehaviourPun
{

    int timesCollided = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterModel>() != null)
        {
            timesCollided++;
            Debug.Log("Collided with playerrrrrr" + other.gameObject);
            photonView.RPC("Win", RpcTarget.Others);// MasterManager.Instance.HandleRPC("SendToWinScreen", PhotonNetwork.LocalPlayer);
            if (timesCollided == 2)
            {
                photonView.RPC("Lose", RpcTarget.Others);
                PhotonNetwork.Destroy(gameObject);
            }

        }

    }

    [PunRPC]
    public void Lose()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Game_Over");
    }

    [PunRPC]
    public void Win()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Win");
    }
}
