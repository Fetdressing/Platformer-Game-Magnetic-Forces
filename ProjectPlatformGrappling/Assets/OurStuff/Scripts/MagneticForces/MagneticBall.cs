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
        thisRigidbody = thisTransform.GetComponent<Rigidbody>();
        thisRigidbody.isKinematic = false;
        SetState(MagneticBallState.HeadingHome);
    }

    public void SetStartTransform(Transform t)
    {
        homeTransform = t;
        initTimes++;
    }

    // Update is called once per frame
    void Update () {
        UpdateLoop();
    }

    public override void UpdateLoop()
    {
        base.UpdateLoop();
        if (initTimes == 0) return;
        switch (magneticBallState)
        {
            case MagneticBallState.HeadingHome:
                HeadHome();
                break;
            case MagneticBallState.HeadingToTarget:
                break;
            case MagneticBallState.ApplyingGravity:
                ApplyForce();
                break;
        }
    }

    public override void FixedUpdateLoop()
    {
        //ingenting, för tydligen körs Updates via arv
    }

    public void SetState(MagneticBallState bS)
    {
        magneticBallState = bS;
        switch (magneticBallState)
        {
            case MagneticBallState.HeadingHome:
                holoRangeTransform.gameObject.SetActive(false);
                ps.enableEmission = false;
                pLight.enabled = false;
                thisRigidbody.isKinematic = true;
                break;
            case MagneticBallState.HeadingToTarget:
                holoRangeTransform.gameObject.SetActive(false);
                ps.enableEmission = false;
                pLight.enabled = true;
                thisRigidbody.isKinematic = false; //forces ska applyas
                break;
            case MagneticBallState.ApplyingGravity:
                holoRangeTransform.gameObject.SetActive(true);
                ps.enableEmission = true;
                pLight.enabled = true;
                thisRigidbody.isKinematic = true;
                break;
        }
    }

    void HeadHome()
    {
        thisTransform.position = Vector3.Slerp(thisTransform.position, homeTransform.position, Time.deltaTime * 10);
    }

    void OnTriggerEnter(Collider col)
    {
        if (magneticBallState == MagneticBallState.HeadingToTarget)
        {
            SetState(MagneticBallState.ApplyingGravity);
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
