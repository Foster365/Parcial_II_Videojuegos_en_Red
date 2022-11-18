using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject grenade;

    public float range = 15f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Launch();
        }
    }

    private void Launch()
    {
        GameObject grenadeInstance = Instantiate(grenade, spawnPoint.position, spawnPoint.rotation);
        grenadeInstance.GetComponent<Rigidbody>().AddForce(spawnPoint.forward * range, ForceMode.Impulse);
    }
}
