using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviourPun
{
    [SerializeField] Nickname playerNickPrefab;
    Nickname playerNick;
    [SerializeField] bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        var gameCanvas = GameObject.Find("Canvas");
        playerNick = GameObject.Instantiate<Nickname>(playerNickPrefab, gameCanvas.transform);
        playerNick.SetTarget(transform);

        if (photonView.IsMine)
        {
            var charModel = GetComponent<CharacterModel>();
            var characterClient = MasterManager.Instance.GetClientFromModel(charModel);
            var nick = characterClient.NickName;
            UpdateNick(nick);
        }
        else photonView.RPC("RequestNick", photonView.Owner, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void RequestNick(Player player)
    {
        var charModel = GetComponent<CharacterModel>();
        var characterClient = MasterManager.Instance.GetClientFromModel(charModel);
        var nick = characterClient.NickName;
        photonView.RPC("UpdateNick", player, nick);
    }

    [PunRPC]
    void UpdateNick(string nickName)
    {
        if (playerNick != null) playerNick.SetName(nickName);
    }

    void OnDestroyNick()
    {
        PhotonNetwork.Destroy(playerNick.gameObject);
    }
}
