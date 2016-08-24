using UnityEngine;
using System.Collections;

public class BaseRigidbody : BaseClass {
    public float SlowDrag = 0.8f;
    public float fastDrag = 0.04f;

    private float minTimer_ApplySlowDrag = 0.0f;
    public float minTime_ApplySlowDrag = 0.5f; //så att den inte bara kan slänga på slowdrag medans fastdrag force applyas

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
}
