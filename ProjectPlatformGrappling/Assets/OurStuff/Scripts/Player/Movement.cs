using UnityEngine;
using System.Collections;

public class Movement : BaseRigidbody {
    public Transform cameraObj;
    private Transform thisTransform;

    private float distanceToGround = 100000000;

    private float speed = 8000;
    [HideInInspector]
    private float jumpForce = 30;
    private float maxSpeed = 55;

    public ParticleSystem slideGroundParticleSystem;
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

        //thisRigidbody.AddForce(((hor + ver) * Time.deltaTime * finalSpeed), ForceMode.Force);
        if (isGrounded)
        {
            bool slowDragApplied = AddForceSlowDrag(((hor + ver) * Time.deltaTime * finalSpeed * 1.9f), ForceMode.Force, thisRigidbody);
            if(slowDragApplied && thisRigidbody.velocity.magnitude > maxSpeed)
            {
                Break();
            }
        }
        else
        {
            AddForceFastDrag(((hor + ver) * Time.deltaTime * finalSpeed), ForceMode.Force, thisRigidbody);
        }
        //thisRigidbody.MovePosition(thisTransform.position + (cameraObj.right * Time.deltaTime * speed * hor));
    }

    void Break()
    {
        thisRigidbody.velocity *= 0.96f;
    }

    void Update()
    {
        isGrounded = GetGrounded();
        distanceToGround = GetDistanceToGround();

        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                //thisRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                AddForceFastDrag(Vector3.up * jumpForce, ForceMode.Impulse, thisRigidbody);
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (GetGrounded() && col.contacts[0].point.y < thisTransform.position.y)
        {
            float speedHit = col.relativeVelocity.magnitude;

            if (speedHit > 70)
            {
                if(slideGroundParticleSystem != null)
                {
                    //Vector3 vecDir = (thisTransform.position- col.contacts[0].point).normalized;
                    //Vector3 dir = Vector3.RotateTowards(thisTransform.position, col.contacts[0].point,1,1);
                    //Vector3 vecDir = thisRigidbody.velocity.normalized + col.contacts[0].normal * 0.1f;
                    //Quaternion rotation = Quaternion.LookRotation(vecDir);
                    //slideGroundParticleSystem.transform.rotation = rotation;
                    float baseParSpeed = 1;
                    slideGroundParticleSystem.startSpeed = baseParSpeed * speedHit;
                    //slideGroundParticleSystem.transform.LookAt(col.contacts[0].point + new Vector3(0,1,0));
                    //slideGroundParticleSystem.transform.Rotate(dir);
                    if (slideGroundParticleSystem.GetComponent<ParticleTimed>().isReady)
                    {
                        slideGroundParticleSystem.GetComponent<ParticleTimed>().StartParticleSystem();
                    }
                }
            }
        }
    }
}
