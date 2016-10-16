using UnityEngine;
using System.Collections;

public class GroundChecker : BaseClass {
    public Transform stagObject;
    private CharacterController cController;
    private CameraShaker cameraShaker;


    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        cController = stagObject.GetComponent<CharacterController>();
        cameraShaker = GameObject.FindGameObjectWithTag("MainCamera").GetComponentsInChildren<Transform>()[1].transform.GetComponent<CameraShaker>();
    }

    //void LateUpdate()
    //{
    //    return;
    //    if(activePlatform != null)
    //    {
    //        Vector3 newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);
    //        Vector3 moveDistance = (newGlobalPlatformPoint - activeGlobalPlatformPoint);
    //        stagObject.position = stagObject.position + moveDistance;

    //        platformVelocity = moveDistance / Time.deltaTime;

    //        //if (moveDistance != Vector3.zero)
    //        //{
    //        //    stagObject.GetComponent<CharacterController>().Move(moveDistance);
    //        //}

    //    }
    //    else
    //    {
    //        platformVelocity = Vector3.zero;
    //    }

    //    activeGlobalPlatformPoint = stagObject.position;
    //    activeLocalPlatformPoint = activePlatform.InverseTransformPoint(stagObject.position);
        
    //}

    //void OnTriggerStay(Collider col)
    //{

    //    if (col.gameObject.tag == "MovingPlatform")
    //    {
    //        activePlatform = col.transform;
    //    }
    //}

    void OnTriggerEnter(Collider col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) != "Player")
        {
            cameraShaker.ShakeCamera(0.1f, 0.5f, true);
        }
    }

}
