using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Transform camPos;

    public Transform CamPos { get => camPos; set => camPos = value; }

    void Update()
    {
        if (camPos)
        {
            transform.position = camPos.position;
            transform.rotation = camPos.rotation;
        }
    }
}
