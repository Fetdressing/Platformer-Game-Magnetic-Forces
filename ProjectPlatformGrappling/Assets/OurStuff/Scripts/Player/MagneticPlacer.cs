using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagneticPlacer : BaseClass {
    private Transform thisTransform;
    public Transform playerTransform;

    public Transform[] projectilesPull;
    //private List<Transform> projectilesStartPositionsPull = new List<Transform>();
    private int currPullIndex = 0;

    public Transform[] projectilesPush;
    //private List<Transform> projectilesStartPositionsPush = new List<Transform>();
    private int currPushIndex = 0;
    // Use this for initialization
    void Start () {
        Init();
    }

    public override void Init()
    {
        base.Init();
        thisTransform = this.transform;
        for (int i = 0; i < projectilesPull.Length; i++)
        {
            Transform temp = new GameObject().transform;
            temp.SetParent(playerTransform);
            temp.position = projectilesPull[i].position;
            //projectilesStartPositionsPull.Add(temp);
            projectilesPull[i].SetParent(null);
            projectilesPull[i].GetComponent<MagneticBall>().SetStartTransform(temp);
        }

        for (int i = 0; i < projectilesPush.Length; i++)
        {
            Transform temp = new GameObject().transform;
            temp.SetParent(playerTransform);
            temp.position = projectilesPush[i].position;
            //projectilesStartPositionsPush.Add(temp);
            projectilesPush[i].SetParent(null);
            projectilesPush[i].GetComponent<MagneticBall>().SetStartTransform(temp);
        }
    }

    void HandleNextPullProjectile()
    {
        MagneticBall mBall = projectilesPull[currPullIndex].GetComponent<MagneticBall>();
        MagneticBallState state = mBall.magneticBallState;

        switch(state)
        {
            case MagneticBallState.HeadingHome:
                mBall.SetState(MagneticBallState.HeadingToTarget);
                mBall.thisRigidbody.AddForce(thisTransform.forward * 300, ForceMode.Impulse);
                break;
            case MagneticBallState.HeadingToTarget:
                mBall.SetState(MagneticBallState.ApplyingGravity);
                break;
            case MagneticBallState.ApplyingGravity:
                mBall.SetState(MagneticBallState.HeadingHome);
                break;
        }
        currPullIndex++;
        if(currPullIndex >= projectilesPull.Length)
        {
            currPullIndex = 0;
        }
    }

    // Update is called once per frame
    void Update () {
	    if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            HandleNextPullProjectile();
        }
	}
}
