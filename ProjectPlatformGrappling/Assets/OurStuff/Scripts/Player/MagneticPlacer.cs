using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagneticPlacer : BaseClass {
    private Transform thisTransform;
    private Camera thisCamera;
    public Transform playerTransform;

    public Transform[] projectilesPull;
    //private List<Transform> projectilesStartPositionsPull = new List<Transform>();
    private int currPullIndex = 0;

    public Transform[] projectilesPush;
    //private List<Transform> projectilesStartPositionsPush = new List<Transform>();
    private int currPushIndex = 0;

    [HideInInspector]
    public float shootForce = 900;
    public LayerMask targetLayerMask;
    // Use this for initialization
    void Start () {
        Init();
    }

    public override void Init()
    {
        base.Init();
        thisTransform = this.transform;
        thisCamera = thisTransform.GetComponentsInChildren<Transform>()[1].GetComponent<Camera>();
        for (int i = 0; i < projectilesPull.Length; i++)
        {
            Transform temp = new GameObject().transform;
            temp.SetParent(playerTransform);
            temp.position = projectilesPull[i].position;
            //projectilesStartPositionsPull.Add(temp);
            projectilesPull[i].SetParent(null);
            projectilesPull[i].GetComponent<MagneticBall>().SetStartTransform(temp);
            projectilesPull[i].GetComponent<MagneticBall>().SetPlayer(playerTransform);
        }

        //for (int i = 0; i < projectilesPush.Length; i++)
        //{
        //    Transform temp = new GameObject().transform;
        //    temp.SetParent(playerTransform);
        //    temp.position = projectilesPush[i].position;
        //    //projectilesStartPositionsPush.Add(temp);
        //    projectilesPush[i].SetParent(null);
        //    projectilesPush[i].GetComponent<MagneticBall>().SetStartTransform(temp);
        //}
    }

    void HandleNextPullProjectile()
    {
        MagneticBall mBall = projectilesPull[currPullIndex].GetComponent<MagneticBall>();
        MagneticBallState state = mBall.magneticBallState;

        MagneticBallGetNextState(mBall);

        currPullIndex++;
        if(currPullIndex >= projectilesPull.Length)
        {
            currPullIndex = 0;
        }
    }

    void HandlePullProjectile(int index) //fast ett specifikt index istället
    {
        MagneticBall mBall = projectilesPull[index].GetComponent<MagneticBall>();
        MagneticBallState state = mBall.magneticBallState;

        MagneticBallGetNextState(mBall);

    }

    void HandleNextPushProjectile()
    {
        MagneticBall mBall = projectilesPush[currPushIndex].GetComponent<MagneticBall>();
        MagneticBallState state = mBall.magneticBallState;

        MagneticBallGetNextState(mBall);
        currPushIndex++;
        if (currPushIndex >= projectilesPush.Length)
        {
            currPushIndex = 0;
        }
    }


    void MagneticBallGetNextState(MagneticBall mBall)
    {
        MagneticBallState state = mBall.magneticBallState;

        switch (state)
        {
            case MagneticBallState.HeadingHome:
                FireMagneticBall(mBall.thisRigidbody);
                break;
            case MagneticBallState.HeadingToTarget:
                mBall.OrderHeadHome();
                break;
            case MagneticBallState.ApplyingGravity:
                mBall.OrderHeadHome();
                break;
        }
    }

    void FireMagneticBall(Rigidbody rb)
    {
        MagneticBall mBall = rb.transform.GetComponent<MagneticBall>();
        mBall.SetState(MagneticBallState.HeadingToTarget);
        RaycastHit raycastHit;
        if (Physics.Raycast(thisCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f)), out raycastHit, targetLayerMask)) //kasta från mitten av skärmen!
        {
            if (Vector3.Dot(raycastHit.normal, thisCamera.transform.position - raycastHit.point) > 0) //normalen mot eller från sig?
            {
                rb.transform.LookAt(raycastHit.point);
                mBall.OrderFire(shootForce, 4);
            }
            else
            {
                rb.transform.forward = thisTransform.forward;
                mBall.OrderFire(shootForce, 4);
            }
        }
        else
        {
            rb.transform.forward = thisTransform.forward;
            mBall.OrderFire(shootForce, 4);
        }
    }

    // Update is called once per frame
    void Update () {
	    if(Input.GetButtonDown("Fire1"))
        {
            HandlePullProjectile(0);
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            HandlePullProjectile(1);
        }
    }
}
