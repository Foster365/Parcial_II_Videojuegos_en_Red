using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviourPun
{
    public float sensX;
    public float sensY;

    [SerializeField] Transform orientation;
    [SerializeField] Transform camHolder;

    float xRotation;
    float yRotation;
    GameObject charModel;

    private void Start()
    {

        //charModel = GameObject.FindWithTag("Player").gameObject;
        //camHolder = charModel.transform;
        //orientation = charModel.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Update()
    {
        if (camHolder && orientation)
        {
            SetCameraValues();
        }
    }

    void SetCameraValues()
    {

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -45, 45f);

        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

    }
    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}
