using UnityEngine;
using System.Collections;

public class MagneticBall : MagneticForce
{
    [HideInInspector]
    public Transform homeTransform;

    [HideInInspector]
    public MagneticBallState magneticBallState = MagneticBallState.HeadingHome;
    [HideInInspector]
    public Rigidbody thisRigidbody;
	// Use this for initialization
	void Start () {
        Init();
    }

    public override void Init()
    {
        base.Init();
        magneticBallState = MagneticBallState.HeadingHome;
        thisRigidbody = thisTransform.GetComponent<Rigidbody>();
        thisRigidbody.isKinematic = false;
    }

    public void SetStartTransform(Transform t)
    {
        homeTransform = t;
        initTimes++;
    }

    // Update is called once per frame
    void Update () {
        if (initTimes == 0) return;
        switch(magneticBallState)
        {
            case MagneticBallState.HeadingHome:
                holoRangeTransform.gameObject.SetActive(false);
                ps.Stop();
                pLight.enabled = false;
                thisRigidbody.isKinematic = true;
                HeadHome();
                break;
            case MagneticBallState.HeadingToTarget:
                holoRangeTransform.gameObject.SetActive(false);
                ps.Stop();
                pLight.enabled = true;
                thisRigidbody.isKinematic = false; //forces ska applyas
                break;
            case MagneticBallState.ApplyingGravity:
                holoRangeTransform.gameObject.SetActive(true);
                ps.Play();
                pLight.enabled = true;
                thisRigidbody.isKinematic = true;
                ApplyForce();
                break;
        }
	}

    public void SetState(MagneticBallState bS)
    {
        magneticBallState = bS;
        switch (magneticBallState)
        {
            case MagneticBallState.HeadingHome:
                holoRangeTransform.gameObject.SetActive(false);
                ps.Stop();
                pLight.enabled = false;
                thisRigidbody.isKinematic = true;
                HeadHome();
                break;
            case MagneticBallState.HeadingToTarget:
                holoRangeTransform.gameObject.SetActive(false);
                ps.Stop();
                pLight.enabled = true;
                thisRigidbody.isKinematic = false; //forces ska applyas
                break;
            case MagneticBallState.ApplyingGravity:
                holoRangeTransform.gameObject.SetActive(true);
                ps.Play();
                pLight.enabled = true;
                thisRigidbody.isKinematic = true;
                ApplyForce();
                break;
        }
    }

    void HeadHome()
    {
        thisTransform.position = Vector3.Slerp(thisTransform.position, homeTransform.position, Time.deltaTime * 10);
    }

    void ApplyForce()
    {
        Collider[] colliders;
        colliders = Physics.OverlapSphere(thisTransform.position, range);
        foreach (Collider col in colliders)
        {
            Transform tr = col.transform;

            if(tr.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rigidbodyTemp = tr.GetComponent<Rigidbody>();
                Vector3 dir;
                float distanceMultiplier = Vector3.Distance(thisTransform.position, tr.position);
                switch (forceType)
                {
                    case ForceType.Push:
                        dir = (tr.transform.position - thisTransform.position).normalized;
                        rigidbodyTemp.AddForce(force * distanceMultiplier * dir * Time.deltaTime, ForceMode.Force);
                        break;
                    case ForceType.Pull:
                        dir = (thisTransform.position - tr.transform.position).normalized;
                        rigidbodyTemp.AddForce(force * distanceMultiplier * dir * Time.deltaTime, ForceMode.Force);
                        break;
                }
            }            
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (magneticBallState == MagneticBallState.HeadingToTarget)
        {
            magneticBallState = MagneticBallState.ApplyingGravity;
        }
        return;
        if(magneticBallState == MagneticBallState.ApplyingGravity)
        {
            if(col.gameObject.tag == "MagneticBall") //en annan kula
            {
                MagneticBall mForce = col.gameObject.GetComponent<MagneticBall>();
                if(mForce.magneticBallState == MagneticBallState.HeadingToTarget)
                {
                    //slå ihop dem
                }
            }
        }
    }

    
}
public enum MagneticBallState { HeadingHome, HeadingToTarget, ApplyingGravity };
