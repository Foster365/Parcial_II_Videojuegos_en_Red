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
            photonView.RPC("Disconnect", RpcTarget.Others);// MasterManager.Instance.HandleRPC("SendToWinScreen", PhotonNetwork.LocalPlayer);
            if (timesCollided == 2) PhotonNetwork.Destroy(gameObject);

        }

    }
    [PunRPC]
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Win");
    }
}
