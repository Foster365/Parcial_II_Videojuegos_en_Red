using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterMovementStatesHandler;

public class CharacterController : MonoBehaviour
{

    [SerializeField] Character character;
    [SerializeField] CharacterMovementStatesHandler charMoveStatesHandler;

    float horizontalInput;
    float verticalInput;

    // Update is called once per frame
    void Update()
    {
        InputHandler();
        character.SpeedControl();
        charMoveStatesHandler.StateHandler();
        character.MovePlayer(horizontalInput, verticalInput); //Puesto en Update a modo de testing

        //fixedUpdate slidemovement script

        if (character.sliding) character.CharacterSlideMovement.SlidingMovement(horizontalInput, verticalInput);
        //
        // handle drag
        if ((charMoveStatesHandler.state == MovementState.walking && !charMoveStatesHandler.activeGrapple) || (charMoveStatesHandler.state == MovementState.sprinting && !charMoveStatesHandler.activeGrapple) || (charMoveStatesHandler.state == MovementState.crouching && !charMoveStatesHandler.activeGrapple))
            character.Rb.drag = character.groundDrag;
        else
            character.Rb.drag = 0;
    }
    public void InputHandler()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //HandleGrappleInput();
        HandleJumpInput();
        HandleCrouchInputs();
        HandleDashInputs();
        HandleSlideInputs();
    }

    #region Movement Input Handlers

    void HandleJumpInput()
    {

        // when to jump
        if (Input.GetKey(character.jumpKey) && character.ReadyToJump && character.Grounded && (character.jumpsRemaining > 0))
        {
            character.ReadyToJump = false;

            character.Jump();

            character.JumpResetHandler();
            character.jumpsRemaining -= 1;
        }

    }
    void HandleCrouchInputs()
    {

        // start crouch
        if (Input.GetKeyDown(character.crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, character.crouchYScale, transform.localScale.z);
            character.Rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(character.crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, character.StartYScale, transform.localScale.z);
        }
    }

    void HandleSlideInputs()
    {

        if (Input.GetKeyDown(character.slideKey) && (horizontalInput != 0 || verticalInput != 0))
            character.CharacterSlideMovement.StartSlide();

        if (Input.GetKeyUp(character.slideKey) && character.sliding)
            character.CharacterSlideMovement.StopSlide();
    }

    void HandleDashInputs()
    {
        if (Input.GetKeyDown(character.dashKey))
            character.CharacterDash.Dash();

        if (character.CharacterDash.DashCdTimer > 0)
            character.CharacterDash.DashCdTimer -= Time.deltaTime;
    }

    void HandleGrappleInput()
    {

        if (Input.GetKeyDown(character.grappleKey)) character.CharacterGrapple.StartGrapple();

        if (character.CharacterGrapple.GrapplingCdTimer > 0)
            character.CharacterGrapple.GrapplingCdTimer -= Time.deltaTime;
    }
    #endregion;
}
