using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public float delay = 1f;

    public float explosionForce = 10f;
    public float radius = 10f;
    private void Start()
    {
        Invoke("Explode", delay);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Explode()
    {
        //Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        //foreach (Collider near in colliders)
        //{
        //    Rigidbody rb = near.GetComponent<Rigidbody>();

        //    if(rb!= null)
        //    {
        //        rb.AddExplosionForce(explosionForce, transform.position, radius, 1f, ForceMode.Impulse);
        //    }
        //}
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
        Destroy(gameObject);
    }
}
