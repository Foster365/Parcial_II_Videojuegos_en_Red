using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeFBX : MonoBehaviour
{
    public float timeLeft = 2f;

    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }
}
