using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullAuthorityController : MonoBehaviourPun
{

    [SerializeField] Transform seed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode dashKey = KeyCode.E;
    public KeyCode slideKey = KeyCode.LeftControl;
    public KeyCode grappleKey = KeyCode.Mouse1;

    float horizontalInput;
    float verticalInput;

    // Start is called before the first frame update

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Camera.main.gameObject != null) Destroy(Camera.main.gameObject);
            Destroy(this);
        }
    }
    void Start()
    {
        MasterManager.Instance.HandleRPC("RequestConnectPlayer",
            PhotonNetwork.LocalPlayer, "Banana_Man", new Vector3(0, 10, 0), Quaternion.identity);
    }

    private void Update()
    {
        InputHandler();
    }

    Vector3 GetRandomSP(Transform _seed)
    {
        return new Vector3(_seed.position.x + Random.Range(0, 5), _seed.position.y, _seed.position.z + Random.Range(0, 5));
    }
    public void InputHandler()
    {

        MasterManager.Instance.HandleRPC("RequestGround", PhotonNetwork.LocalPlayer);

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (dir != Vector3.zero) HandleMovementInput(horizontalInput, verticalInput);
        HandleJumpInput();
        HandleCrouchInputs();
        HandleDashInputs();
        HandleSlideInputs();
    }
    #region Movement Input Handlers

    void HandleMovementInput(float _horizontal, float _vertical)
    {
        MasterManager.Instance.HandleRPC("RequestMove", PhotonNetwork.LocalPlayer, _horizontal, _vertical);
    }

    void HandleJumpInput()
    {

        // when to jump
        if (Input.GetKey(jumpKey)) MasterManager.Instance.HandleRPC("RequestJump", PhotonNetwork.LocalPlayer);//Poner keys en este scripts, y la lògica que toque a character. en el mastermanager

    }
    void HandleCrouchInputs()
    {

        // start crouch
        if (Input.GetKeyDown(crouchKey))
            MasterManager.Instance.HandleRPC("RequestStartCrouch", PhotonNetwork.LocalPlayer);

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
            MasterManager.Instance.HandleRPC("RequestStopCrouch", PhotonNetwork.LocalPlayer);

    }

    void HandleSlideInputs()
    {

        if (Input.GetKeyDown(slideKey))
            MasterManager.Instance.HandleRPC("RequestStartSlide", PhotonNetwork.LocalPlayer, horizontalInput, verticalInput);

        if (Input.GetKeyUp(slideKey))
            MasterManager.Instance.HandleRPC("RequestStopSlide", PhotonNetwork.LocalPlayer);
    }

    void HandleDashInputs()
    {
        if (Input.GetKeyDown(dashKey))
            MasterManager.Instance.HandleRPC("RequestDash", PhotonNetwork.LocalPlayer);

    }

    #endregion

}