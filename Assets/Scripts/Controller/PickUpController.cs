using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviourPun
{
    public Launcher launcherScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;
    bool isColliding = false;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) Destroy(this);
        if (!equipped)
        {
            launcherScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
    }

    private void Update()
    {
        if (!equipped && isColliding && !slotFull && Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q code pressed");
            MasterManager.Instance.HandleRPC("RequestLaucherPickUp", PhotonNetwork.LocalPlayer);
            Equip();
            slotFull = true;
        }
    }

    void Equip()
    {
        Debug.Log("Attaching Launcher to player");
        equipped = true;
        transform.SetParent(FindObjectOfType<LauncherContainer>().transform);//(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        rb.isKinematic = true;
        coll.isTrigger = true;

        launcherScript.enabled = true;

    }

    public void Drop()
    {
        slotFull = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterModel>() != null)
        {
            player = collision.gameObject.GetComponent<CharacterModel>().transform;
            isColliding = true;
        }


    }

}
