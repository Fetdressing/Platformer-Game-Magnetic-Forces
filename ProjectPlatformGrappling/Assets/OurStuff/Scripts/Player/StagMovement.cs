using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class StagMovement : BaseClass
{
    public Transform cameraObj;
    public AudioSource movementAudioSource;
    private CharacterController characterController;
    private Health thisHealth;

    private float distanceToGround = 100000000;
    public Transform stagRootJoint; //den ska röra på sig i y-led
    private float stagRootJointStartY; //krävs att animationen börjar i bottnen isåfall

    private float startSpeed = 100;
    private float jumpSpeed = 100;
    private float gravity = 100;

    private float currSpeed; //movespeeden, kan påverkas av slows
    private float ySpeed; //aktiv variable för vad som händer med gravitation/jump
    private float jumpTimePoint; //när man hoppas så den inte ska resetta stuff dirr efter man hoppat

    private float dashSpeed = 1600;
    private float currDashTime;
    private float maxDashTime = 0.03f;

    private Vector3 hor = new Vector3(0, 0, 0); //har dem här så jag kan hämta värdena via update
    private Vector3 ver = new Vector3(0, 0, 0);
    private Vector3 dashVel = new Vector3(0, 0, 0);

    private LayerMask layermaskForces;
    public ParticleSystem slideGroundParticleSystem;
    public AudioClip slideGroundSound;

    public PullField pullField; //som drar till sig grejer till spelaren, infinite gravity!

    [Header("Ground Check")]
    public float groundedCheckOffsetY = 0.5f;
    public float groundedCheckDistance = 1.9f;
    [HideInInspector]
    public bool isGrounded;
    public LayerMask groundCheckLM;
    private float groundedTimePoint = 0; //när man blev grounded

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        characterController = transform.GetComponent<CharacterController>();
        thisHealth = transform.GetComponent<Health>();
        isGrounded = false;
        groundCheckLM = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("MagneticBall") | 1 << LayerMask.NameToLayer("Ragdoll"));
        layermaskForces = groundCheckLM;

        stagRootJointStartY = stagRootJoint.localPosition.y;

        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        currSpeed = startSpeed;
        dashVel = new Vector3(0, 0, 0);
        ToggleInfiniteGravity(false);
        slideGroundParticleSystem.GetComponent<ParticleTimed>().isReady = true;
    }

    void LateUpdate()
    {
        stagRootJoint.forward = cameraObj.forward;
    }

    void Update()
    {
        hor = Input.GetAxis("Horizontal") * cameraObj.right;
        ver = Input.GetAxis("Vertical") * cameraObj.forward;
        ver = new Vector3(ver.x, 0, ver.z);

        isGrounded = characterController.isGrounded;
        distanceToGround = GetDistanceToGround();

        float stagSpeedMultiplier = 1.0f;
        if (isGrounded)
        {
            stagSpeedMultiplier = Mathf.Max(Mathf.Abs(stagRootJointStartY - stagRootJoint.localPosition.y), 0.85f);
        }

        Vector3 finalMoveDir = (hor + ver).normalized * stagSpeedMultiplier * currSpeed;
        //characterController.Move(finalMoveDir * speed * stagSpeedMultiplier * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (stagSpeedMultiplier > 0)
            {
                if ((hor + ver).magnitude > 0.1f)
                {
                    Dash((hor + ver).normalized);
                }
                else
                {
                    Dash(cameraObj.forward);
                }
            }
        }


        if (isGrounded || stagSpeedMultiplier > 1.2f)
        {
            if (jumpTimePoint < Time.time + 0.8f) //så den inte ska fucka och resetta dirr efter man hoppat
            {
                ySpeed = 0; // grounded character has vSpeed = 0...

                if (Input.GetButtonDown("Jump"))
                {
                    jumpTimePoint = Time.time;
                    ySpeed = jumpSpeed;
                    
                }
            }
        }


        // apply gravity acceleration to vertical speed:
        ySpeed -= gravity * Time.deltaTime;
        finalMoveDir.y = ySpeed; // include vertical speed in vel
        characterController.Move((finalMoveDir + dashVel) * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleInfiniteGravity(!pullField.enabled);
        }
    }

    void Dash(Vector3 dir)
    {
        StartCoroutine(MoveDash(dir));
    }

    IEnumerator MoveDash(Vector3 dir)
    {
        currDashTime = 0.0f;
        float startDashTime = Time.time;
        while(currDashTime < maxDashTime)
        {
            dashVel = dir * dashSpeed;
            currDashTime = Time.time - startDashTime;
            yield return new WaitForSeconds(0.01f);
        }

        dashVel = Vector3.zero;

    }

    void OnCollisionEnter(Collision col)
    {
        if (GetGrounded() && col.contacts[0].point.y < transform.position.y)
        {
            float speedHit = col.relativeVelocity.magnitude;

            if (speedHit > thisHealth.speedDamageThreshhold * 0.7f)
            {
                //ForcePush(speedHit);
            }
        }
    }

    void ToggleInfiniteGravity(bool b)
    {
        pullField.enabled = b;
        ParticleSystem pullps = pullField.gameObject.GetComponent<ParticleSystem>();

        //pullps.emission.enabled = b;

        if (b)
        {
            pullps.Play();
        }
        else
        {
            pullps.Stop();
        }


        if (b)
        {

        }
        else
        {

        }
    }



    public bool GetGrounded()
    {
        RaycastHit rHit;
        if (Physics.Raycast(this.transform.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, groundedCheckDistance, groundCheckLM))
        {
            if (isGrounded == false) //om man inte var grounded innan
            {
                groundedTimePoint = Time.time;
            }
            isGrounded = true;
            return isGrounded;
        }
        else
        {
            groundedTimePoint = Time.time + 1000;
            isGrounded = false;
            return isGrounded;
        }
    }

    public bool GetGrounded(Transform tChecker) //från en annan utgångspunkt
    {
        RaycastHit rHit;
        if (Physics.Raycast(tChecker.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, groundedCheckDistance, groundCheckLM))
        {
            if (rHit.transform == this.transform) { Debug.Log(this.transform.name); return false; } //MEH DEN SKA EJ COLLIDA MED SIG SJÄLV

            if (isGrounded == false) //om man inte var grounded innan
            {
                groundedTimePoint = Time.time;
            }
            isGrounded = true;
            return isGrounded;
        }
        else
        {
            groundedTimePoint = Time.time + 1000;
            isGrounded = false;
            return isGrounded;
        }
    }

    public float GetGroundedDuration()
    {
        //if (Time.time - groundedTimePoint > 2)
        //    Debug.Log((Time.time - groundedTimePoint).ToString());
        return Time.time - groundedTimePoint;
    }

    public float GetDistanceToGround()
    {
        RaycastHit rHit;
        if (Physics.Raycast(this.transform.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, Mathf.Infinity, groundCheckLM))
        {
            return Vector3.Distance(this.transform.position + new Vector3(0, groundedCheckOffsetY, 0), rHit.point);
        }
        else
        {
            return 10000000;
        }
    }

    public float GetDistanceToGround(Transform tChecker)
    {
        RaycastHit rHit;
        if (Physics.Raycast(tChecker.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, Mathf.Infinity, groundCheckLM))
        {
            return Vector3.Distance(tChecker.position + new Vector3(0, groundedCheckOffsetY, 0), rHit.point);
        }
        else
        {
            return 10000000;
        }
    }
}