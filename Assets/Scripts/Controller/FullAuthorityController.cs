using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullAuthorityController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Destroy(Camera.main.gameObject);
            Destroy(this);
        }
        else MasterManager.Instance.HandleRPC("RequestConnectPlayer", PhotonNetwork.LocalPlayer, "Banana_Man");
    }

    private void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v).normalized;

        if (dir != Vector3.zero)
        {

            if (MasterManager.Instance.CharactersDictionary.ContainsKey(PhotonNetwork.LocalPlayer))
            {
                MasterManager.Instance.HandleRPC("RequestBoolAnimation", PhotonNetwork.LocalPlayer, "isCharacterMoving", true);
            }
            MasterManager.Instance.HandleRPC("RequestMove", PhotonNetwork.LocalPlayer, dir);
        }
        else
        {
            MasterManager.Instance.HandleRPC("RequestBoolAnimation", PhotonNetwork.LocalPlayer, "isCharacterMoving", false);
        }
    }
}
