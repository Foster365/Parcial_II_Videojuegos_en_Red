using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CharacterMovementStatesHandler;

public class Character : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float wallRunSpeed;

    public float dashSpeed, dashSpeedChangeFactor;

    public float maxYSpeed;

    public float groundDrag;
    float speedChangeFactor;

    [Header("Jumping")]
    public float jumpForce, jumpCooldown, airMultiplier;
    public int maxJumpCount = 2, jumpsRemaining = 0;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed, crouchYScale, startYScale;
    public bool sliding;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode dashKey = KeyCode.E;
    public KeyCode slideKey = KeyCode.LeftControl;
    public KeyCode grappleKey = KeyCode.Mouse1;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;


    public Transform orientation;

    [Header("Character Inputs float")]
    float horizontalInput, verticalInput;

    Vector3 moveDirection, velocityToSet;

    //Components attached to Character
    CharacterMovementStatesHandler charMoveStatesHandler;
    DashMovement characterDash;
    SlideMovement characterSlideMovement;
    Grapple characterGrapple;
    Rigidbody rb;

    #region Encapsulated variables
    public Rigidbody Rb { get => rb; set => rb = value; }
    public float SpeedChangeFactor { get => speedChangeFactor; set => speedChangeFactor = value; }
    public bool Grounded { get => grounded; set => grounded = value; }
    public bool ReadyToJump { get => readyToJump; set => readyToJump = value; }
    public float StartYScale { get => startYScale; set => startYScale = value; }
    public DashMovement CharacterDash { get => characterDash; set => characterDash = value; }
    public SlideMovement CharacterSlideMovement { get => characterSlideMovement; set => characterSlideMovement = value; }
    public Grapple CharacterGrapple { get => characterGrapple; set => characterGrapple = value; }

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        charMoveStatesHandler = GetComponent<CharacterMovementStatesHandler>();
        characterDash = GetComponent<DashMovement>();
    }

    private void Start()
    {
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
    }

    #region Character Movement

    public void MovePlayer(float _horizontalInput, float _verticalInput)
    {
        if (charMoveStatesHandler.state == CharacterMovementStatesHandler.MovementState.dashing) return;

        // calculate movement direction
        moveDirection = (orientation.forward * _verticalInput) + (orientation.right * _horizontalInput);

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }
    #endregion

    #region Character Speed/Velocity Handling
    public void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
    }
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }
    #endregion

    #region Character Jump Handling
    public void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    public void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }
    public void JumpResetHandler()
    {
        Invoke(nameof(ResetJump), jumpCooldown);
    }
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        charMoveStatesHandler.activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }
    #endregion

    #region Slopes Movement Handling
    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, (playerHeight * 0.5f) + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
    private bool enableMovementOnNextTouch;
    #endregion

    #region Grapple (Not implemented yet)
    public void ResetRestrictions()
    {
        charMoveStatesHandler.activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grapple>().StopGrapple();
        }
    }
    #endregion
}