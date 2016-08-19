using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {
    private Rigidbody thisRigidbody;
    private Transform thisTransform;


    private Transform currTarget;
    private int currIndex = 0;
    public Transform[] keyPoints;
    public float force = 40000;
	// Use this for initialization
	void Start () {
        thisTransform = this.transform;
        thisRigidbody = thisTransform.GetComponent<Rigidbody>();

        GetCurrentTarget();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        CheckReached();

        Vector3 vecToTarget = (currTarget.position - thisTransform.position).normalized;
        thisRigidbody.AddForce(vecToTarget * force * Time.deltaTime);
	}

    void CheckReached()
    {
        if(Vector3.Distance(currTarget.position, thisTransform.position) < 2)
        {
            GetNextIndex();
            GetCurrentTarget();
        }
    }

    void GetNextIndex()
    {
        currIndex++;
        if(currIndex >= keyPoints.Length)
        {
            currIndex = 0;
        }
    }

    void GetCurrentTarget()
    {
        currTarget = keyPoints[currIndex];
    }
}
