using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class StagMovement : BaseClass
{
    public Transform cameraHolder; //den som förflyttas när man rör sig med musen
    private Transform cameraObj; //kameran själv
    private CameraShaker cameraShaker;
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

    private float currMovementSpeed; //movespeeden, kan påverkas av slows
    [HideInInspector]public float currMoveSpeedEffector = 1.0f; //100% movespeed, påverkar slows n shit
    [HideInInspector]public float ySpeed; //aktiv variable för vad som händer med gravitation/jump
    private float jumpTimePoint = -5; //när man hoppas så den inte ska resetta stuff dirr efter man hoppat
    private int jumpAmount = 2; //hur många hopp man får
    private int jumpsAvaible = 0; //så man kan hoppa i luften also, förutsatt att man resettat den på marken
    private float jumpCooldown = 0.3f;

    [HideInInspector]public float dashTimePoint; //mud påverkar denna så att man inte kan dasha
    private float dashCooldown = 0.3f;
    private float dashSpeed = 1200;
    private float currDashTime;
    private float maxDashTime = 0.1f;
    private float dashPowerCost = 0.03f; //hur mycket power det drar varje gång man dashar
    private bool dashUsed = false; //så att man måste bli grounded innan man kan använda den igen
    public GameObject dashEffectObject;

    private float knockForceMovingPlatform = 700; //om man hamnar på fel sidan av moving platform så knuffas man bort lite

    //moving platform

    private Transform activePlatform;

    private Vector3 activeGlobalPlatformPoint;
    private Vector3 activeLocalPlatformPoint;

    private float airbourneTime = 0.0f;
    //moving platform

    private Vector3 horVector = new Vector3(0, 0, 0); //har dem här så jag kan hämta värdena via update
    private Vector3 verVector = new Vector3(0, 0, 0);
    private float hor, ver;
    private Vector3 dashVel = new Vector3(0, 0, 0);
    private Vector3 finalMoveDir = new Vector3(0,0,0);
    private Vector3 externalVel = new Vector3(0, 0, 0);

    private LayerMask layermaskForces;
    public ParticleSystem slideGroundParticleSystem;
    public AudioClip slideGroundSound;

    public PullField pullField; //som drar till sig grejer till spelaren, infinite gravity!

    [Header("Ground Check")]
    public Transform groundCheckObject;
    public float groundedCheckOffsetY = 0.5f;
    public float groundedCheckDistance = 3.5f;
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
        layermaskForces = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("MagneticBall") | 1 << LayerMask.NameToLayer("Ragdoll"));
        cameraObj = cameraHolder.GetComponentsInChildren<Transform>()[1].transform;
        cameraShaker = cameraObj.GetComponent<CameraShaker>();

        stagRootJointStartY = stagRootJoint.localPosition.y;

        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        ToggleDashEffect(false);
        currMovementSpeed = startSpeed;

        dashVel = new Vector3(0, 0, 0);
        externalVel = new Vector3(0, 0, 0);
        ySpeed = 0;
        currMoveSpeedEffector = 1.0f;

        dashTimePoint = 0;
        jumpTimePoint = -5; //behöver vara under 0 så att man kan hoppa dirr när spelet börjar
        ToggleInfiniteGravity(false);
        slideGroundParticleSystem.GetComponent<ParticleTimed>().isReady = true;
        dashUsed = false;
        jumpsAvaible = jumpAmount;
    }

    void LateUpdate()
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(cameraHolder.forward.x, 0, cameraHolder.forward.z));
        stagObject.rotation = Quaternion.Slerp(stagObject.rotation, lookRotation, Time.deltaTime * 20);

        currMoveSpeedEffector = 1.0f; //resetta den varje frame, så ifall någon vill påverka så får den oxå sätta varje frame
    }

    void Update()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
        horVector = hor * cameraHolder.right;
        verVector = ver * cameraHolder.forward;
        
        isGrounded = characterController.isGrounded;

        distanceToGround = GetDistanceToGround(groundCheckObject);

        //FUNKAAAAAAR EJ?!?!?!? kallas bara när man rör på sig wtf, kan funka ändå
        if (isGrounded) //dessa if-satser skall vara separata
        {
            dashUsed = false; //när man blir grounded så kan man använda dash igen
            if (jumpTimePoint < Time.time - 0.4f) //så den inte ska fucka och resetta dirr efter man hoppat
            {
                ySpeed = 0; // grounded character has vSpeed = 0...
            }
        }

        if (isGrounded || GetGrounded(groundCheckObject)) //använd endast GetGrounded här, annars kommer man få samma problem när gravitationen slutar verka pga lång raycast
        {
            if (jumpTimePoint < Time.time - 0.4f) //så den inte ska fucka och resetta dirr efter man hoppat
            {
                jumpsAvaible = jumpAmount;
            }
        }

        if (jumpsAvaible > 0)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (Time.time > jumpTimePoint + jumpCooldown)
                {
                    jumpsAvaible = Mathf.Max(0, (jumpsAvaible - 1));
                    dashUsed = false; //när man blir grounded så kan man använda dash igen, men oxå när man hoppar, SKILLZ!!!
                    activePlatform = null; //när man hoppar så är man ej längre attached till movingplatform
                    jumpTimePoint = Time.time;

                    if (ySpeed < 0) //ska motverka gravitationen
                        ySpeed = 0;

                    ySpeed = jumpSpeed;
                    //animationH.Play(jump.name);
                    //animationH[jump.name].weight = 1.0f;
                }
            }
        }
        // apply gravity acceleration to vertical speed:
        ySpeed -= gravity * Time.deltaTime;
        Vector3 yVector = new Vector3(0, ySpeed, 0);
        characterController.Move((yVector) * Time.deltaTime);

        if (activePlatform != null)
        {
            ySpeed = 0; //behöver inte lägga på gravity när man står på moving platform, varför funkar inte grounded? lol
            Vector3 newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);
            Vector3 moveDistance = (newGlobalPlatformPoint - activeGlobalPlatformPoint);

            if (activeLocalPlatformPoint != Vector3.zero)
            {
                //transform.position = transform.position + moveDistance;
                characterController.Move(moveDistance);
            }
        }

        if (activePlatform != null)
        {
            activeGlobalPlatformPoint = transform.position;
            activeLocalPlatformPoint = activePlatform.InverseTransformPoint(transform.position);
        }

        if(GetGroundedTransform(groundCheckObject) != activePlatform)
            activePlatform = null; //kolla om platformen fortfarande finns under mig eller ej

        externalVel = Vector3.Lerp(externalVel, Vector3.zero, Time.deltaTime * 10);

        HandleMovement(); //moddar finalMoveDir
        characterController.Move((finalMoveDir + dashVel + externalVel) * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleInfiniteGravity(!pullField.enabled);
        }

        PlayAnimationStates();

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.5f)
        {
            ySpeed = 0;
        }

        if (hit.gameObject.tag == "MovingPlatform")
        {
            if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.5f)
            {
                if (activePlatform != hit.transform)
                {
                    activeGlobalPlatformPoint = Vector3.zero;
                    activeLocalPlatformPoint = Vector3.zero;
                }
                activePlatform = hit.transform;
            }
            else //knuffa spelaren lite för denne kom emot en kant, kolla ifall den rör sig mot en? då knockar den ju bort en, bättre än att den alltid gör det?
            {
                ApplyExternalForce((transform.position - hit.transform.position).normalized * knockForceMovingPlatform); //knocked away
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "BreakerObject")
        {
            col.GetComponent<BreakerObject>().Break();
        }
    }

    void HandleMovement()
    {
        float stagSpeedMultiplier = 1.0f;
        if (isGrounded)
        {
            stagSpeedMultiplier = Mathf.Max(Mathf.Abs(stagRootJointStartY - stagRootJoint.localPosition.y), stagSpeedMultMin); //min värde
            stagSpeedMultiplier = Mathf.Min(stagSpeedMultiplier, stagSpeedMultMax); //max värde
        }

        currMovementSpeed = startSpeed * currMoveSpeedEffector;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (stagSpeedMultiplier > 0)
            {
                //if ((horVector + verVector).magnitude > 0.1f)
                //{
                //    //Dash((horVector + verVector).normalized);
                //    Dash(cameraHolder.forward);
                //}
                if (ver < 0.0f) //bakåt
                {
                    Dash(-cameraHolder.forward);
                }
                else
                {
                    Dash(cameraHolder.forward);
                }
            }
        }

        verVector = new Vector3(verVector.x, 0, verVector.z); //denna behöver vara under dash så att man kan dasha upp/ned oxå

        finalMoveDir = (horVector + verVector).normalized * stagSpeedMultiplier * currMovementSpeed * (Mathf.Max(0.8f, powerManager.currPower) * 1.2f);
    }

    void PlayAnimationStates()
    {
        if (animationH == null) return;
        float fadeLengthA = 0.1f;

        if (isGrounded || GetGrounded(groundCheckObject))
        {
            if (ver > 0.1f || ver < -0.1f) //för sig frammåt/bakåt
            {

                if (hor > 0.1f || hor < -0.1f) //rär sig sidledes
                {
                    animationH.CrossFade(runForwardAngle.name, fadeLengthA);
                }
                else
                {
                    animationH.CrossFade(runForward.name, fadeLengthA);
                }
            }
            else if (hor > 0.1f || hor < -0.1f) //bara rör sig sidledes
            {
                animationH.CrossFade(runForwardAngle.name, fadeLengthA);
            }
            else
            {
                animationH.CrossFade(idle.name, fadeLengthA);
            }
        }
        else //air
        {
            if (ySpeed > 0.01f)
            {
                animationH.CrossFade(jump.name, fadeLengthA);
            }
            else
            {
                animationH.CrossFade(idleAir.name, fadeLengthA);
            }
        }
    }

    public void ApplyYForce(float velY) //till characterscontrollern, inte rigidbody
    {
        jumpTimePoint = Time.time;
        ySpeed += velY;
    }
    public void ApplyYForce(float velY, float maxVel) //till characterscontrollern, inte rigidbody, med ett max värde
    {
        if (ySpeed >= maxVel) return;
        jumpTimePoint = Time.time;
        ySpeed += velY;
    }

    void Dash(Vector3 dir)
    {
        if (!powerManager.SufficentPower(-dashPowerCost)) return;
        StartCoroutine(MoveDash(dir));
    }

    IEnumerator MoveDash(Vector3 dir)
    {
        if (dashTimePoint + dashCooldown > Time.time) yield break;
        if (dashUsed) yield break;
        ySpeed = 0;
        dashUsed = true;
        ToggleDashEffect(true);
        powerManager.AddPower(-dashPowerCost);
        dashTimePoint = Time.time;
        currDashTime = 0.0f;
        float startDashTime = Time.time;
        while(currDashTime < maxDashTime)
        {
            dashVel = dir * dashSpeed;
            currDashTime = Time.time - startDashTime;
            yield return new WaitForEndOfFrame();
        }
        ToggleDashEffect(false);
        dashVel = Vector3.zero;

    }

    public void ApplyExternalForce(Vector3 moveDir)
    {
        externalVel = moveDir;
    }

    void ToggleDashEffect(bool b)
    {
        if (gameObject.activeSelf == false) return;
        dashEffectObject.transform.rotation = cameraHolder.rotation;
        float trailOriginalTime = 2.0f;
        float startWidth = 1;
        float endWidth = 0.1f;
        TrailRenderer[] tR = dashEffectObject.GetComponentsInChildren<TrailRenderer>();
        ParticleSystem[] pS = dashEffectObject.GetComponentsInChildren<ParticleSystem>();

        for(int i = 0; i < tR.Length; i++)
        {
            if(b)
            {
                tR[i].time = trailOriginalTime;
                tR[i].startWidth = startWidth;
                tR[i].endWidth = endWidth;
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
            tR.time -= 3 * Time.deltaTime;
            tR.startWidth -= Time.deltaTime;
            tR.endWidth -= Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
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


    public Transform GetGroundedTransform(Transform tChecker) //får den transformen man står på, från en annan utgångspunkt
    {
        RaycastHit rHit;
        if (Physics.Raycast(tChecker.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, groundedCheckDistance, groundCheckLM))
        {
            if (rHit.transform == this.transform) { Debug.Log(this.transform.name); return transform; } //MEH DEN SKA EJ COLLIDA MED SIG SJÄLV

            return rHit.transform;
        }
        else
        {
            groundedTimePoint = Time.time + 1000;
            return transform;
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