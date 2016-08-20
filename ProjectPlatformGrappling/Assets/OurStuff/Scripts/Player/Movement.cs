using UnityEngine;
using System.Collections;

public class Movement : BaseClass {
    public Transform cameraObj;
    private Transform thisTransform;
    private Rigidbody thisRigidbody;

    [HideInInspector]
    public float speed = 20;
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        thisTransform = this.transform;
        thisRigidbody = thisTransform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        Vector3 hor = Input.GetAxis("Horizontal") * cameraObj.right;
        Vector3 ver = Input.GetAxis("Vertical") * cameraObj.forward;

        thisRigidbody.MovePosition(thisTransform.position + ((hor + ver) * Time.deltaTime * speed));
        //thisRigidbody.MovePosition(thisTransform.position + (cameraObj.right * Time.deltaTime * speed * hor));
    }
}
