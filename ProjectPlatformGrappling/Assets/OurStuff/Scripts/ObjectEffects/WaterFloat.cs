using UnityEngine;
using System.Collections;

public class WaterFloat : BaseRigidbody {
    public Vector3 upFloatVector = Vector3.up;
    public Vector3 streamFloatVector = new Vector3(0,0,0);

    public float streamForce = 1600;

    public GameObject splashParticle;
    // Use this for initialization

    void OnTriggerStay(Collider col)
    {
        Rigidbody colRig = col.GetComponent<Rigidbody>();

        if (colRig != null && colRig.isKinematic == false)
        {

            AddForceSlowDrag(streamFloatVector * streamForce, ForceMode.Force, colRig);
            AddForceSlowDrag(upFloatVector * streamForce * 0.5f, ForceMode.Force, colRig);
        }
        else
        {
            StagMovement stagM = col.GetComponent<StagMovement>();
            if(stagM != null)
            {
                stagM.ApplyYForce(streamForce * 0.01f, streamForce * 0.1f);
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //splash
        GameObject parTemp = GameObject.Instantiate(splashParticle.gameObject);
        parTemp.transform.position = col.transform.position;
        Destroy(parTemp, 3);
    }
}
