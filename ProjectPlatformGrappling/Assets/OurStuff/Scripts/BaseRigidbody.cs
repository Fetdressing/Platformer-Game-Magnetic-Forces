﻿using UnityEngine;
using System.Collections;

public class BaseRigidbody : BaseClass {
    [HideInInspector]
    public Rigidbody currRigidbody;
    [HideInInspector]
    public Rigidbody thisRigidbody; //rigidbodyn på detta object, disablas när man slår på ragdoll
    [HideInInspector]
    public Collider thisCollider; //colliderna på detta object, disablas när man slår på ragdoll

    public float SlowDrag = 0.8f;
    public float fastDrag = 0.04f;

    private float minTimer_ApplySlowDrag = 0.0f;
    public float minTime_ApplySlowDrag = 0.5f; //så att den inte bara kan slänga på slowdrag medans fastdrag force applyas

    [Header("Ground Check")]
    public float groundedCheckOffsetY = 0.5f;
    public float groundedCheckDistance = 1.9f;
    [HideInInspector]
    public bool isGrounded;
    public LayerMask groundCheckLM;
    private float groundedTimePoint = 0; //när man blev grounded
    private float ariseTime = 1.5f; //hur långt tid det tar för unitet att resa sig, dvs till groundedtime

    public override void Init()
    {
        base.Init();
        thisRigidbody = this.transform.GetComponent<Rigidbody>();
        currRigidbody = thisRigidbody;
        thisCollider = this.transform.GetComponent<Collider>();
    }

    public override void Reset()
    {
        base.Reset();
        isGrounded = false;
    }

    public bool AddForceSlowDrag(Vector3 dirForce, ForceMode forceMode, Rigidbody rigidbody)
    {
        bool slowDragApplied = false;
        if (minTimer_ApplySlowDrag < Time.time)
        {
            rigidbody.drag = SlowDrag;
            slowDragApplied = true;
        }
        rigidbody.AddForce(dirForce, forceMode);
        return slowDragApplied;
    }

    public void AddForceFastDrag(Vector3 dirForce, ForceMode forceMode, Rigidbody rigidbody)
    {
        minTimer_ApplySlowDrag = Time.time + minTime_ApplySlowDrag;
        rigidbody.drag = fastDrag;
        rigidbody.AddForce(dirForce, forceMode);
    }

    public bool GetGrounded()
    {
        RaycastHit rHit;
        if (Physics.Raycast(this.transform.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, groundedCheckDistance, groundCheckLM))
        {
            if (isGrounded == false) //om man inte var grounded innan
            {
                groundedTimePoint = Time.time;
            }
            isGrounded = true;
            return isGrounded;
        }
        else
        {
            groundedTimePoint = Time.time + 1000;
            isGrounded = false;
            return isGrounded;
        }
    }

    public bool GetGrounded(Transform tChecker) //från en annan utgångspunkt
    {
        RaycastHit rHit;
        if (Physics.Raycast(tChecker.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, groundedCheckDistance, groundCheckLM))
        {
            if (isGrounded == false) //om man inte var grounded innan
            {
                groundedTimePoint = Time.time;
            }
            isGrounded = true;
            return isGrounded;
        }
        else
        {
            groundedTimePoint = Time.time + 1000;
            isGrounded = false;
            return isGrounded;
        }
    }

    public float GetGroundedDuration()
    {
        //if (Time.time - groundedTimePoint > 2)
        //    Debug.Log((Time.time - groundedTimePoint).ToString());
        return Time.time - groundedTimePoint;
    }

    public bool IsStanding() //kolla ifall agenten har varit grounded tillräkligt länge
    {
        float groundedTime = GetGroundedDuration();
        if(groundedTime > ariseTime)
        {
            return true;
        }
        return false;
    }

    public float GetDistanceToGround()
    {
        RaycastHit rHit;
        if (Physics.Raycast(this.transform.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, Mathf.Infinity, groundCheckLM))
        {
            return Vector3.Distance(this.transform.position + new Vector3(0, groundedCheckOffsetY, 0), rHit.point);
        }
        else
        {
            return 10000000;
        }
    }

    public float GetDistanceToGround(Transform tChecker)
    {
        RaycastHit rHit;
        if (Physics.Raycast(tChecker.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, Mathf.Infinity, groundCheckLM))
        {
            return Vector3.Distance(tChecker.position + new Vector3(0, groundedCheckOffsetY, 0), rHit.point);
        }
        else
        {
            return 10000000;
        }
    }
}
