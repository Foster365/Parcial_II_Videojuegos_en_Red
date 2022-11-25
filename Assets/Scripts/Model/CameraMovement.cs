using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Transform camPos;

    public float sensX;
    public float sensY;

    Transform orientation;
    Transform camHolder;

    float xRotation;
    float yRotation;

    public Transform CamPos { get => camPos; set => camPos = value; }
    public Transform Orientation { get => orientation; set => orientation = value; }
    public Transform CamHolder { get => camHolder; set => camHolder = value; }

    private void Start()
    {

        //charModel = GameObject.FindWithTag("Player").gameObject;
        //camHolder = charModel.transform;
        //orientation = charModel.transform;
        sensX = 400;
        sensY = 400;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    void Update()
    {
        //if (camHolder && orientation)
        //{
        //    SetCameraValues();
        //}
        if (camPos)
        {
            SetCameraValues();

            //transform.position = camPos.position;
            //transform.rotation = camPos.rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) Cursor.lockState = CursorLockMode.None;
    }
    void SetCameraValues()
    {

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        //Debug.Log("X ROT: " + xRotation);
        xRotation = Mathf.Clamp(xRotation, -7f, 7f);

        transform.position = camPos.position;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        //transform.rotation = Quaternion.Euler(0, yRotation, 0);

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
