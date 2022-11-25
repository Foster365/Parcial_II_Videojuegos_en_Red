using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviourPun
{
    public Transform spawnPoint;
    public GameObject grenade;
    public GameObject me;

    public float range = 15f;
    public int currentShootAmount;
    public int maxShootAmount;
    //public PickUpController pickUp;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            MasterManager.Instance.HandleRPC("RequestBoolAnimation", PhotonNetwork.LocalPlayer, "isShooting", true);
            MasterManager.Instance.HandleRPC("SpawnGrenade", spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
            currentShootAmount++;
            MasterManager.Instance.HandleRPC("RequestBoolAnimation", PhotonNetwork.LocalPlayer, "isShooting", false);
        }

        if (currentShootAmount == maxShootAmount)
        {
            currentShootAmount = 0;
            //MasterManager.Instance.HandleRPC("DropLauncher", PhotonNetwork.LocalPlayer);
            //if (!MasterManager.Instance.IsOkToDestroyLauncher) PhotonNetwork.Destroy(this.gameObject);
            //pickUp.Drop();
        }
    }

    void SpawnGrenade()
    {
        GameObject grenadeGO = Instantiate(grenade, transform.position, transform.rotation);//PhotonNetwork.Instantiate("Grenade", spawnPoint.position, spawnPoint.rotation);
        grenadeGO.GetComponent<Rigidbody>().AddForce(spawnPoint.forward * range, ForceMode.Impulse);
        currentShootAmount++;
    }
}
