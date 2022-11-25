using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class Grenade : MonoBehaviourPun
{
    public float delay = .5f;

    public float explosionForce = 10f;
    public float radius = 10f;
    public GameObject effect;
    private void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 15f, ForceMode.Impulse);
        Invoke("Explode", delay);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPos, radius, 3.0f, ForceMode.Impulse);
            }
            Debug.Log(hit.name);
        }
        MasterManager.Instance.HandleRPC("InstantiateFBX", explosionPos.x, explosionPos.y, explosionPos.z);
        //MasterManager.Instance.HandleRPC("InstantiateGrenadeFBX", explosionPos);
        Destroy(gameObject);
    }

    void InstantiateFBX(Vector3 _position)
    {
        Instantiate(effect, _position, Quaternion.identity);
    }
}
