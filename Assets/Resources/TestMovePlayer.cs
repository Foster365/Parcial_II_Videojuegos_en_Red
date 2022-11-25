using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovePlayer : MonoBehaviour
{
    public float moveSpeed, rotSpeed;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 movementDir = new Vector3(horizontalInput, 0, verticalInput).normalized;

        movementDir *= moveSpeed;
        movementDir.y = rb.velocity.y;
        rb.velocity = movementDir;

        if (movementDir != Vector3.zero)
        {
            //Quaternion toRotation = Quaternion.LookRotation(movementDir, Vector3.up);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotSpeed * Time.deltaTime);
            transform.forward = Vector3.Lerp(transform.forward, movementDir, rotSpeed * Time.deltaTime);
        }
    }
}

