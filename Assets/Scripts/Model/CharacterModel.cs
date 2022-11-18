using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviourPun
{

    [SerializeField] float speed, rotationSpeed, jumpForce;
    Rigidbody rb;

    CharacterAnimations charAnimations;
    GameManager characterGameManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        charAnimations = GetComponent<CharacterAnimations>();
    }

    public GameManager SetCharacterGameManager
    {
        set
        {
            characterGameManager = value;
        }
    }

    public CharacterAnimations CharAnimations { get => charAnimations; set => charAnimations = value; }
    public float Speed { get => speed; set => speed = value; }

    public void Move(Vector3 _dir)
    {
        _dir *= speed;
        _dir.y = rb.velocity.y;
        rb.velocity = _dir;

    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void LookDir(Vector3 _dir)
    {
        transform.forward = Vector3.Lerp(transform.forward, _dir, rotationSpeed * Time.deltaTime);
    }

    [PunRPC]
    public void Die()
    {
        MasterManager.Instance.RemoveCharacterModel(this);
        PhotonNetwork.Destroy(gameObject);
    }
    public void TouchBomb()
    {
        photonView.RPC("UpdateBomb", photonView.Owner);
    }
    [PunRPC]
    public void UpdateBomb()
    {
        characterGameManager.GetBomb.SetTarget(this);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)
        {
            var character = collision.gameObject.GetComponent<CharacterModel>();
            if (character != null)
            {
                if (characterGameManager == null) return;
                if (characterGameManager.GetBomb == null) return;
                if (characterGameManager.GetBomb.GetTarget == this)
                {
                    character.TouchBomb();
                }
            }
        }
    }
}
