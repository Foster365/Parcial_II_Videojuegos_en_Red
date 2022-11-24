using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovementStatesHandler : MonoBehaviourPun
{
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        wallrunning,
        freeze,
        crouching,
        dashing,
        air
    }

    public bool freeze;
    public bool dashing;
    public bool wallrunning;
    public bool activeGrapple;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;

    Character playerMovement;

    public float DesiredMoveSpeed { get => desiredMoveSpeed; set => desiredMoveSpeed = value; }
    public bool KeepMomentum { get => keepMomentum; set => keepMomentum = value; }

    private void Awake()
    {
        if (photonView.IsMine)
        {
            playerMovement = GetComponent<Character>();
        }
    }

    public void StateHandler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            desiredMoveSpeed = 0;
            playerMovement.Rb.velocity = Vector3.zero;
        }

        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = playerMovement.wallRunSpeed;
        }

        // Mode - Dashing
        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = playerMovement.dashSpeed;
            playerMovement.SpeedChangeFactor = playerMovement.dashSpeedChangeFactor;
        }

        // Mode - Crouching
        else if (Input.GetKey(playerMovement.crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = playerMovement.crouchSpeed;
        }

        // Mode - Sprinting
        else if (playerMovement.Grounded && Input.GetKey(playerMovement.sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = playerMovement.sprintSpeed;
        }

        // Mode - Walking
        else if (playerMovement.Grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = playerMovement.walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            if (desiredMoveSpeed < playerMovement.sprintSpeed)
                desiredMoveSpeed = playerMovement.walkSpeed;
            else
                desiredMoveSpeed = playerMovement.sprintSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                playerMovement.moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }
    public IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - playerMovement.moveSpeed);
        float startValue = playerMovement.moveSpeed;

        float boostFactor = playerMovement.SpeedChangeFactor;

        while (time < difference)
        {
            playerMovement.moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        playerMovement.moveSpeed = desiredMoveSpeed;
        playerMovement.SpeedChangeFactor = 1f;
        keepMomentum = false;
    }

    //
}
