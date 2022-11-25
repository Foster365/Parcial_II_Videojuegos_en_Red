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

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Destroy(this);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T key pressed");
            MasterManager.Instance.HandleRPC("SpawnGrenade", PhotonNetwork.LocalPlayer);
            if (MasterManager.Instance.IsOkToSpawnGrenade) SpawnGrenade();
        }

        if (currentShootAmount == maxShootAmount)
        {
            MasterManager.Instance.HandleRPC("DropLauncher", PhotonNetwork.LocalPlayer);
            if (!MasterManager.Instance.IsOkToDestroyLauncher) PhotonNetwork.Destroy(this.gameObject);
            //pickUp.Drop();
        }
    }

    void SpawnGrenade()
    {
        GameObject grenadeGO = Instantiate(grenade, spawnPoint.position, spawnPoint.rotation);
        grenadeGO.GetComponent<Rigidbody>().AddForce(spawnPoint.forward * range, ForceMode.Impulse);
        currentShootAmount++;
    }
}
