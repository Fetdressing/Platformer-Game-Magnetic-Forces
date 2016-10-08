using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class StagMovement : BaseClass
{
    public Transform cameraObj;
    public AudioSource movementAudioSource;
    private CharacterController characterController;
    private PowerManager powerManager;

    private float distanceToGround = 100000000;
    public Transform stagRootJoint; //den ska röra på sig i y-led
    private float stagRootJointStartY; //krävs att animationen börjar i bottnen isåfall
    public Transform stagObject; //denna roteras så det står korrekt

    private float startSpeed = 100;
    private float jumpSpeed = 100;
    private float gravity = 140;
    private float stagSpeedMultMax = 1.5f;
    private float stagSpeedMultMin = 0.85f;

    private float currSpeed; //movespeeden, kan påverkas av slows
    private float ySpeed; //aktiv variable för vad som händer med gravitation/jump
    private float jumpTimePoint; //när man hoppas så den inte ska resetta stuff dirr efter man hoppat

    private float dashTimePoint;
    private float dashCooldown = 0.8f;
    private float dashSpeed = 1200;
    private float currDashTime;
    private float maxDashTime = 0.1f;
    private float dashPowerCost = 0.03f; //hur mycket power det drar varje gång man dashar
    public GameObject dashEffectObject;

    private Vector3 horVector = new Vector3(0, 0, 0); //har dem här så jag kan hämta värdena via update
    private Vector3 verVector = new Vector3(0, 0, 0);
    private float hor, ver;
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

    [Header("Animation")]
    public Animation animationH;

    public AnimationClip runForward;
    public AnimationClip runForwardAngle;
    public AnimationClip idle;
    public AnimationClip idleAir;
    public AnimationClip jump;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        characterController = transform.GetComponent<CharacterController>();
        powerManager = transform.GetComponent<PowerManager>();
        isGrounded = false;
        groundCheckLM = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("MagneticBall") | 1 << LayerMask.NameToLayer("Ragdoll"));
        layermaskForces = groundCheckLM;

        stagRootJointStartY = stagRootJoint.localPosition.y;

        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        ToggleDashEffect(false);
        currSpeed = startSpeed;
        dashVel = new Vector3(0, 0, 0);
        dashTimePoint = 0;
        jumpTimePoint = 0;
        ToggleInfiniteGravity(false);
        slideGroundParticleSystem.GetComponent<ParticleTimed>().isReady = true;
    }

    void LateUpdate()
    {
        stagObject.forward = new Vector3(cameraObj.forward.x ,0, cameraObj.forward.z);
    }

    void Update()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
        horVector = hor * cameraObj.right;
        verVector = ver * cameraObj.forward;

        isGrounded = characterController.isGrounded;

        distanceToGround = GetDistanceToGround();

        float stagSpeedMultiplier = 1.0f;
        if (isGrounded)
        {
            stagSpeedMultiplier = Mathf.Max(Mathf.Abs(stagRootJointStartY - stagRootJoint.localPosition.y), stagSpeedMultMin); //min värde
            stagSpeedMultiplier = Mathf.Min(stagSpeedMultiplier, stagSpeedMultMax); //max värde
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (stagSpeedMultiplier > 0)
            {
                if ((horVector + verVector).magnitude > 0.1f)
                {
                    Dash((horVector + verVector).normalized);
                }
                else
                {
                    Dash(cameraObj.forward);
                }
            }
        }

        verVector = new Vector3(verVector.x, 0, verVector.z); //denna behöver vara under dash så att man kan dasha upp/ned oxå

        Vector3 finalMoveDir = (horVector + verVector).normalized * stagSpeedMultiplier * currSpeed;
        //characterController.Move(finalMoveDir * speed * stagSpeedMultiplier * Time.deltaTime);

        if (isGrounded || GetGrounded()) //använd endast GetGrounded här, annars kommer man få samma problem när gravitationen slutar verka pga lång raycast
        {
            if (jumpTimePoint < Time.time - 0.8f) //så den inte ska fucka och resetta dirr efter man hoppat
            {
                if (Input.GetButtonDown("Jump"))
                {
                    jumpTimePoint = Time.time;
                    ySpeed = jumpSpeed;
                    //animationH.Play(jump.name);
                    //animationH[jump.name].weight = 1.0f;
                }
            }
        }
        if (isGrounded) //dessa if-satser skall vara separata
        {
            if (jumpTimePoint < Time.time - 0.8f) //så den inte ska fucka och resetta dirr efter man hoppat
            {
                ySpeed = 0; // grounded character has vSpeed = 0...
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

        PlayAnimationStates();
    }

    void PlayAnimationStates()
    {
        if (animationH == null) return;

        if (isGrounded || GetGrounded())
        {
            if (ver > 0.1f || ver < -0.1f) //för sig frammåt/bakåt
            {

                if (hor > 0.1f || hor < -0.1f) //rär sig sidledes
                {
                    animationH.CrossFade(runForwardAngle.name);
                }
                else
                {
                    animationH.CrossFade(runForward.name);
                }
            }
            else if (hor > 0.1f || hor < -0.1f) //bara rör sig sidledes
            {
                animationH.CrossFade(runForwardAngle.name);
            }
            else
            {
                animationH.CrossFade(idle.name);
            }
        }
        else //air
        {
            if (ySpeed > 0.1f)
            {
                animationH.CrossFade(jump.name);
            }
            else
            {
                animationH.CrossFade(idleAir.name);
            }
        }
    }

    void Dash(Vector3 dir)
    {
        StartCoroutine(MoveDash(dir));
    }

    IEnumerator MoveDash(Vector3 dir)
    {
        if (dashTimePoint + dashCooldown > Time.time) yield break;
        ToggleDashEffect(true);
        powerManager.AddPower(-dashPowerCost);
        dashTimePoint = Time.time;
        currDashTime = 0.0f;
        float startDashTime = Time.time;
        while(currDashTime < maxDashTime)
        {
            dashVel = dir * dashSpeed;
            currDashTime = Time.time - startDashTime;
            yield return new WaitForSeconds(0.01f);
        }
        ToggleDashEffect(false);
        dashVel = Vector3.zero;

    }

    void ToggleDashEffect(bool b)
    {
        dashEffectObject.transform.rotation = cameraObj.rotation;
        float trailOriginalTime = 2.0f;
        TrailRenderer[] tR = dashEffectObject.GetComponentsInChildren<TrailRenderer>();
        ParticleSystem[] pS = dashEffectObject.GetComponentsInChildren<ParticleSystem>();

        for(int i = 0; i < tR.Length; i++)
        {
            if(b)
            {
                tR[i].time = trailOriginalTime;
            }
            else
            {
                StartCoroutine(ShutDownTrail(tR[i]));
            }
        }

        for(int i = 0; i < pS.Length; i++)
        {
            if (b)
            {
                pS[i].Play();
            }
            else
            {
                pS[i].Stop();
            }
        }
    }
    IEnumerator ShutDownTrail(TrailRenderer tR)
    {
        while(tR.time > 0.0f)
        {
            tR.time -= 4 * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        //if (GetGrounded() && col.contacts[0].point.y < transform.position.y)
        //{
        //    float speedHit = col.relativeVelocity.magnitude;

        //    if (speedHit > thisHealth.speedDamageThreshhold * 0.7f)
        //    {
        //        //ForcePush(speedHit);
        //    }
        //}
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
            return true;
        }
        else
        {
            groundedTimePoint = Time.time + 1000;
            return false;
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
            return true;
        }
        else
        {
            groundedTimePoint = Time.time + 1000;
            return false;
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