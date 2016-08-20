using UnityEngine;
using System.Collections;

public class Movement : BaseClass {
    public Transform cameraObj;
    private Transform thisTransform;
    private Rigidbody thisRigidbody;
    [HideInInspector]
    public bool isGrounded;
    private float groundedCheckOffsetY = 0.5f;
    private LayerMask groundCheckLM;

    private float distanceToGround = 100000000;

    private float speed = 8000;
    [HideInInspector]
    private float jumpForce = 30;
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        thisTransform = this.transform;
        thisRigidbody = thisTransform.GetComponent<Rigidbody>();
        isGrounded = false;
        groundCheckLM = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("MagneticBall"));
    }

    // Update is called once per frame
    void FixedUpdate () {

        Vector3 hor = Input.GetAxis("Horizontal") * cameraObj.right;
        Vector3 ver = Input.GetAxis("Vertical") * cameraObj.forward;

        //thisRigidbody.MovePosition(thisTransform.position + ((hor + ver) * Time.deltaTime * speed));
        float finalSpeed;

        finalSpeed = speed * Mathf.Clamp(1 - (distanceToGround/100), 0.01f, 1.0f);

        thisRigidbody.AddForce(((hor + ver) * Time.deltaTime * finalSpeed), ForceMode.Force);
        //thisRigidbody.MovePosition(thisTransform.position + (cameraObj.right * Time.deltaTime * speed * hor));
    }

    void Update()
    {
        isGrounded = GetGrounded();
        distanceToGround = GetDistanceToGround();

        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                thisRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    public bool GetGrounded()
    {
        RaycastHit rHit;
        float groundedCheckDistance = 1.9f;
        if (Physics.Raycast(thisTransform.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, groundedCheckDistance, groundCheckLM))
        {
            return true;
        }
        else
        {

            return false;
        }
    }

    public float GetDistanceToGround()
    {
        RaycastHit rHit;
        if (Physics.Raycast(thisTransform.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, Mathf.Infinity, groundCheckLM))
        {
            return Vector3.Distance(thisTransform.position + new Vector3(0, groundedCheckOffsetY, 0), rHit.point);
        }
        else
        {
            return 10000000;
        }
    }
}
