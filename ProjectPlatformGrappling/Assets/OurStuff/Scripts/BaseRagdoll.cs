using UnityEngine;
using System.Collections;

public class BaseRagdoll : BaseRigidbody {
    bool lastState; //vad den senast var, så man inte sätter den till samma sak hela tiden

    public Rigidbody baseJointRigidbody;
    [HideInInspector]
    public Vector3 baseJointStartPos;

    public float toggleCooldown = 1.0f;
    private float toggleCooldownTimer = 0.0f;

    public override void Init()
    {
        base.Init();
        baseJointStartPos = baseJointRigidbody.transform.localPosition;

        lastState = true; //viktigt
        ToggleRagdoll(false);
    }

    public override void Reset()
    {
        base.Reset();
        toggleCooldownTimer = 0.0f;
    }

    public virtual bool ToggleRagdoll(bool b) //returnerar ifall den kunde gör detta nu
    {
        if (lastState == b) return false;

        lastState = b;

        if (Time.time < toggleCooldownTimer) return false;
        else
        {
            toggleCooldownTimer = Time.time + toggleCooldown;
        }
       

        foreach (Rigidbody rb in this.transform.GetComponentsInChildren<Rigidbody>())
        {
            if (rb != thisRigidbody)
            {
                rb.isKinematic = !b;
                rb.transform.GetComponent<Collider>().isTrigger = !b;
                rb.useGravity = b;
            }
        }

        if (b == true) //sätt igång ragdoll!
        {
            currRigidbody = baseJointRigidbody;
            thisCollider.isTrigger = true;
            baseJointRigidbody.isKinematic = false;
            thisRigidbody.isKinematic = true;
            thisRigidbody.useGravity = false;
        }
        else //stäng av ragdoll, flytta tillbaks childet
        {
            currRigidbody = thisRigidbody; //kan behöva flytta upp transformen så den inte buggar genom marken
            thisCollider.isTrigger = false;
            baseJointRigidbody.isKinematic = true;
            thisRigidbody.isKinematic = false;

            thisRigidbody.transform.position = baseJointRigidbody.transform.position + new Vector3(0, 3, 0);
            baseJointRigidbody.transform.localPosition = baseJointStartPos;
            thisRigidbody.useGravity = true;
        }

        return true;
    }
}
