using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviourPun
{
    public float timeToExplode = 3;
    public float cooldown = .25f;
    bool _isInCooldown;
    public Vector3 offset;
    CharacterModel _target;
    private void Awake()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(TicTac());
        }
    }
    public void SetTarget(CharacterModel target)
    {
        if (!_isInCooldown)
        {
            _target = target;
            StartCoroutine(BombCooldown());
        }
    }
    public void SetRandomTarget()
    {
        CharacterModel[] characters = MasterManager.Instance.GetAllClientModels();
        if (characters.Length > 1)
        {
            List<CharacterModel> list = new List<CharacterModel>();
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != _target)
                {
                    list.Add(characters[i]);
                }
            }
            int index = Random.Range(0, list.Count - 1);
            SetTarget(list[index]);
        }
        else
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
    void Explotion()
    {
        _target.Die();
        SetRandomTarget();
    }
    IEnumerator TicTac()
    {
        yield return new WaitForSeconds(timeToExplode);
        Explotion();
    }
    IEnumerator BombCooldown()
    {
        _isInCooldown = true;
        yield return new WaitForSeconds(cooldown);
        _isInCooldown = false;
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (_target != null)
            {
                transform.position = _target.transform.position + offset;
            }
        }
    }
    public CharacterModel GetTarget
    {
        get
        {
            return _target;
        }
    }
}
