using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [Header("References")]
    private Character pm;
    CharacterMovementStatesHandler charStateMovementHandler;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    private bool grappling;

    public float GrapplingCdTimer { get => grapplingCdTimer; set => grapplingCdTimer = value; }

    private void Awake()
    {
        charStateMovementHandler = GetComponent<CharacterMovementStatesHandler>();
    }
    private void Start()
    {
        pm = GetComponent<Character>();
    }

    private void LateUpdate()
    {
        // if (grappling)
        //    lr.SetPosition(0, gunTip.position);
    }

    public void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        grappling = true;

        charStateMovementHandler.freeze = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + (cam.forward * maxGrappleDistance);

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        //lr.enabled = true;
        //lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        charStateMovementHandler.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        charStateMovementHandler.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        //lr.enabled = false;
    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
