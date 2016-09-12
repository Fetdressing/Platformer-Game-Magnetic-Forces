using UnityEngine;
using System.Collections;

public class ForcePusher : BaseRigidbody {
    public bool isContinuous = false;
    public float pushForce = 100;

    void OnTriggerEnter(Collider col)
    {
        if (isContinuous) return;
        Rigidbody rbody = col.GetComponent<Rigidbody>();

        if (rbody != null)
        {
            AddForceFastDrag(transform.forward * pushForce, ForceMode.Impulse, rbody);
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (!isContinuous) return;

        Rigidbody rbody = col.GetComponent<Rigidbody>();

        if (rbody != null)
        {
            AddForceFastDrag(transform.forward * pushForce * Time.deltaTime, ForceMode.Force, rbody); //vill kanske använda fixedupdate I guess
        }
    }
}
