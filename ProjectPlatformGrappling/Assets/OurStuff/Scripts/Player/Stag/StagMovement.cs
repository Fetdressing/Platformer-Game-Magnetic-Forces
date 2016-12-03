using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//[RequireComponent(typeof(CharacterController))]
public class StagMovement : BaseClass
{
    [HideInInspector]
    //public bool isLocked = false; //förhindrar alla actions, ligger i baseclass
    public Transform cameraHolder; //den som förflyttas när man rör sig med musen
    protected Transform cameraObj; //kameran själv
    protected CameraShaker cameraShaker;
    protected CharacterController characterController;
    protected PowerManager powerManager;

    protected StagSpeedBreaker speedBreaker;
    protected float speedBreakerActiveSpeed = 1.8f; //vid vilken fart den går igång
    protected float speedBreakerTime = 0.5f;
    protected float speedBreakerTimer = 0.5f;

    protected float distanceToGround = Mathf.Infinity;
    public Transform stagRootJoint; //den ska röra på sig i y-led
    protected float stagRootJointStartY; //krävs att animationen börjar i bottnen isåfall
    public Transform stagObject; //denna roteras så det står korrekt

    protected float startSpeed = 190;
    protected float jumpSpeed = 85;
    protected float gravity = 140;
    protected float stagSpeedMultMax = 1.5f;
    protected float stagSpeedMultMin = 0.85f;

    protected float currMovementSpeed; //movespeeden, kan påverkas av slows
    [HideInInspector]public float currExternalSpeedMult = 1.0f; //100% movespeed, påverkar slows n shit
    protected float moveSpeedMultTimePoint = -5; //när extern slow/speed-up var applyat
    protected float moveSpeedMultDuration;
    [HideInInspector]public float ySpeed; //aktiv variable för vad som händer med gravitation/jump
    protected float jumpTimePoint = -5; //när man hoppas så den inte ska resetta stuff dirr efter man hoppat
    protected int jumpAmount = 2; //hur många hopp man får
    protected int jumpsAvaible = 0; //så man kan hoppa i luften also, förutsatt att man resettat den på marken
    protected float jumpCooldown = 0.15f;
    public GameObject jumpEffectObject;
    [HideInInspector] public float currFrameMovespeed = 0; //hur snabbt man rört sig denna framen
    protected Vector3 lastFramePos = Vector3.zero;

    [HideInInspector]public float dashTimePoint; //mud påverkar denna så att man inte kan dasha
    protected float dashCooldown = 0.3f;
    protected float dashSpeed = 380;
    protected float currDashTime;
    protected float startMaxDashTime = 0.05f; //den går att utöka
    [HideInInspector] public float maxDashTime;
    protected float dashPowerCost = 0.1f; //hur mycket power det drar varje gång man dashar
    protected bool dashUsed = false; //så att man måste bli grounded innan man kan använda den igen
    public GameObject dashEffectObject;
    public ParticleSystem dashReadyPS; //particlesystem som körs när dash är redo att användas

    protected float knockForceMovingPlatform = 420; //om man hamnar på fel sidan av moving platform så knuffas man bort lite

    //moving platform

    [HideInInspector] public Transform activePlatform;

    protected Vector3 activeGlobalPlatformPoint;
    protected Vector3 activeLocalPlatformPoint;

    protected float airbourneTime = 0.0f;
    //moving platform

    protected Vector3 horVector = new Vector3(0, 0, 0); //har dem här så jag kan hämta värdena via update
    protected Vector3 verVector = new Vector3(0, 0, 0);
    protected float hor, ver;
    [HideInInspector] public Vector3 dashVel = new Vector3(0, 0, 0); //vill kunna komma åt denna, så därför public
    protected Vector3 finalMoveDir = new Vector3(0,0,0);
    protected Vector3 externalVel = new Vector3(0, 0, 0);
    protected Vector3 currMomentum = Vector3.zero; //så man behåller fart även efter man släppt på styrning
    protected float startLimitSpeed = 60;
    protected float currLimitSpeed;

    public Text moveStackText;
    protected float movementStackGroundedTime = 5.0f;
    protected float movementStackGroundedTimer = 0.0f;

    protected float movementStackResetTimer = 0.0f;
    protected float movementStackResetTime = 4.0f;
    [HideInInspector]public int movementStacks = 0; //får mer stacks när man dashar och hoppar mycket


    public PullField pullField; //som drar till sig grejer till spelaren, infinite gravity!

    [Header("Ground Check")]
    public Transform groundCheckObject;
    protected float groundedCheckOffsetY = 0.6f;
    protected float groundedCheckDistance = 8.5f;
    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public bool isGroundedRaycast;
    protected Transform groundedRaycastObject; //objektet som man blev grounded med raycast på

    public LayerMask groundCheckLM;
    protected float groundedTimePoint = 0; //när man blev grounded
    protected float maxSlopeGrounded = 70; //vilken vinkel det som mest får skilja på ytan och vector3.down när man kollar grounded
    protected float groundedSlope = 0;
    protected Vector3 groundedNormal = Vector3.zero;
    protected GroundChecker groundChecker; //så man kan resetta stuff till camerashake tex

    [Header("Animation")]
    public Animation animationH;
    public float runAnimSpeedMult = 2.0f;
    public float animationSpeedMult = 2.0f; //en overall speed som sätts i början

    public AnimationClip runForward;
    public AnimationClip runForwardRight;
    public AnimationClip runForwardLeft;
    public AnimationClip idle;
    public AnimationClip idleAir;
    public AnimationClip jump;

    //[Header("Audio")]
    //public AudioSource jumpAudioSource;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        characterController = transform.GetComponent<CharacterController>();
        powerManager = transform.GetComponent<PowerManager>();
        cameraHolder = GameObject.FindGameObjectWithTag("MainCamera").transform;
        cameraObj = cameraHolder.GetComponentsInChildren<Transform>()[1].transform;
        cameraShaker = cameraObj.GetComponent<CameraShaker>();
        groundChecker = GetComponentsInChildren<GroundChecker>()[0];

        try
        {
            speedBreaker = GetComponentsInChildren<StagSpeedBreaker>()[0];
        }
        catch
        {
            Debug.Log("Hittade ingen Speedbreaker i mina children");
        }

        animationH[runForward.name].speed = runAnimSpeedMult;
        animationH[runForwardRight.name].speed = runAnimSpeedMult;
        animationH[runForwardLeft.name].speed = runAnimSpeedMult;
        animationH[idle.name].speed = animationSpeedMult;
        animationH[idleAir.name].speed = animationSpeedMult;
        animationH[jump.name].speed = animationSpeedMult;

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
        ySpeed = -gravity * 0.01f; //nollställer ej helt
        currExternalSpeedMult = 1.0f;
        currLimitSpeed = startLimitSpeed;

        maxDashTime = startMaxDashTime;
        dashTimePoint = 0;
        jumpTimePoint = -5; //behöver vara under 0 så att man kan hoppa dirr när spelet börjar
        //ToggleInfiniteGravity(false);
        dashUsed = true;
        jumpsAvaible = jumpAmount;
        movementStacks = 1;

        isGrounded = false;
        isGroundedRaycast = false;
    }

    void LateUpdate()
    {
        if (Time.timeScale == 0) return;
        if (isLocked) return;

        if (currMomentum.magnitude < 0.01f) return;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(currMomentum.x, 0, currMomentum.z));
        //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(cameraHolder.forward.x, 0, cameraHolder.forward.z));
        stagObject.rotation = Quaternion.Slerp(stagObject.rotation, lookRotation, deltaTime * 20);

        if((moveSpeedMultTimePoint + moveSpeedMultDuration) < Time.time)
        {
            currExternalSpeedMult = 1.0f;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        if (isLocked) return;

        isGrounded = characterController.isGrounded;
        isGroundedRaycast = GetGrounded(groundCheckObject);

        if (movementStackResetTimer < Time.time)
        {
            //movementStacks = 1;
            AddMovementStack(-2);
        }

        if(isGroundedRaycast && (groundedTimePoint + movementStackGroundedTimer) < Time.time)
        {
            AddMovementStack(-2);
        }

        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
        horVector = hor * cameraHolder.right;
        verVector = ver * cameraHolder.forward;

        distanceToGround = GetDistanceToGround(groundCheckObject);

        //FUNKAAAAAAR EJ?!?!?!? kallas bara när man rör på sig wtf, kan funka ändå
        if (isGrounded) //dessa if-satser skall vara separata
        {
            //dashUsed = false; //när man blir grounded så kan man använda dash igen
            if (jumpTimePoint < Time.time - 0.4f) //så den inte ska fucka och resetta dirr efter man hoppat
            {
                ySpeed = -gravity * 0.01f; //nollställer ej helt // grounded character has vSpeed = 0...
            }
        }

        if (isGroundedRaycast) //använd endast GetGrounded här, annars kommer man få samma problem när gravitationen slutar verka pga lång raycast
        {
            if (jumpTimePoint < Time.time - 0.4f) //så den inte ska fucka och resetta dirr efter man hoppat
            {
                //dessa resetsen görs här eftersom denna groundchecken är mycket mer pålitlig
                //dashUsed = true; //resettar bara med riktigt grounded så det ska vara mer "snällt"
                jumpsAvaible = jumpAmount;
            }

            if(groundedSlope > maxSlopeGrounded) //denna checken görs här när man är grounded och i charactercontrollerhit när man INTE är grounded
            {
                ApplyExternalForce(groundedNormal * 20); // så man glider för slopes
            }
        }
        else
        {
            groundChecker.Reset(groundedTimePoint);
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            Slam();
        }

        if(IsDashReady())
        {
            ToggleDashReadyPS(true); //visa att man kan dasha
        }
        else
        {
            ToggleDashReadyPS(false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.M))
        {
            //Dash(transform.forward);
            if (ver < 0.0f) //bakåt
            {
                //Dash(-transform.forward);
                Dash(-cameraHolder.forward);
            }
            else
            {
                Dash(cameraHolder.forward);
            }
        }

        // apply gravity acceleration to vertical speed:
        if (activePlatform == null)
        {
            ySpeed -= gravity * deltaTime;
            Vector3 yVector = new Vector3(0, ySpeed, 0);
            characterController.Move((yVector) * deltaTime);
        }

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

        if (GetGroundedTransform(groundCheckObject, 2) != activePlatform)
        {
            activePlatform = null; //kolla om platformen fortfarande finns under mig eller ej
        }

        externalVel = Vector3.Lerp(externalVel, Vector3.zero, deltaTime * 10); //ta sakta bort den externa forcen

        HandleMovement(); //moddar finalMoveDir
        characterController.Move((currMomentum + dashVel + externalVel) * deltaTime);


        currFrameMovespeed = (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(lastFramePos.x, 0, lastFramePos.z)) * deltaTime) * 100;
        //Debug.Log((currFrameMovespeed).ToString());

        //if (currFrameMovespeed > speedBreakerActiveSpeed)
        //{
        //    Debug.Log((currFrameMovespeed).ToString());
        //    speedBreakerTimer = Time.time + speedBreakerTime;

        //}

        if (speedBreakerTimer > Time.time)
        {
            speedBreaker.Activate();
        }
        else
        {
            speedBreaker.Disable();
        }

        lastFramePos = transform.position;
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    ToggleInfiniteGravity(!pullField.enabled);
        //}

        PlayAnimationStates();

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Time.timeScale == 0) return;
        //fan viktigt :o ful hacks men still
        float slope = GetSlope(hit.normal);

        if (!hit.collider.isTrigger)
        {
            if (slope > maxSlopeGrounded)
            {
                if (isGroundedRaycast)
                {
                    if (groundedSlope > maxSlopeGrounded)
                    {
                        ApplyExternalForce(hit.normal * 20);
                    }
                }
                else //om man inte är grounded så använder man ju en gammal slope? denna kan vara farlig att ha här
                {
                    ApplyExternalForce(hit.normal * 20); // så man glider för slopes
                }
            }
            else //ingen slope, dvs man står på marken, resetta stuff!
            {
                if (jumpTimePoint < Time.time - 0.4f) //så den inte ska fucka och resetta dirr efter man hoppat
                {
                    dashUsed = true;
                    jumpsAvaible = jumpAmount;
                    ySpeed = -gravity * 0.01f; //nollställer ej helt // grounded character has vSpeed = 0...
                }
            }
        

            //if (hit.normal.y < 0.5f) //slå i taket
            //{
            //    ySpeed = -gravity * 0.01f; //nollställer ej helt
            //    //dashUsed = false; //när man blir grounded så kan man använda dash igen
            //    //if (jumpTimePoint < Time.time - 0.4f) //så den inte ska fucka och resetta dirr efter man hoppat
            //    //{
            //    //    ySpeed = -gravity * 0.01f; //nollställer ej helt // grounded character has vSpeed = 0...
            //    //}
            //}
        }

        if (hit.gameObject.tag == "MovingPlatform")
        {
            //MovingPlatform movingPlatform = hit.gameObject.GetComponent<MovingPlatform>();
            //Vector3 platToPlayer = (transform.position - hit.point).normalized;
            //transform.position = new Vector3(transform.position.x, hit.point.y - 0.2f, transform.position.z);
            if (hit.moveDirection.y < -0.9f && hit.normal.y > 0.5f)
            {
                if (activePlatform != hit.transform)
                {
                    activeGlobalPlatformPoint = Vector3.zero;
                    activeLocalPlatformPoint = Vector3.zero;
                }
                activePlatform = hit.transform;
            }

            //if (Vector3.Angle(movingPlatform.moveDirection, platToPlayer) < 80) //rör sig platformen mot spelaren va?//knuffa spelaren lite för denne kom emot en kant, kolla ifall den rör sig mot en? då knockar den ju bort en, bättre än att den alltid gör det?
            //{
            //    Debug.Log(Time.time.ToString());
            //    ApplyExternalForce((transform.position - hit.transform.position).normalized * knockForceMovingPlatform); //knocked away
            //}
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "BreakerObject")
        {
            col.GetComponent<BreakerObject>().Break(); //tar sönder objekten
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "BreakerObject")
        {
            cameraShaker.ShakeCamera(0.2f, 0.6f, true);
        }
    }

    public virtual void HandleMovement()
    {
        float stagSpeedMultiplier = 1.0f;
        if (isGrounded)
        {
            stagSpeedMultiplier = Mathf.Max(Mathf.Abs(stagRootJointStartY - stagRootJoint.localPosition.y), stagSpeedMultMin); //min värde
            stagSpeedMultiplier = Mathf.Min(stagSpeedMultiplier, stagSpeedMultMax); //max värde
        }

        currMovementSpeed = startSpeed * currExternalSpeedMult;

        verVector = new Vector3(verVector.x, 0, verVector.z); //denna behöver vara under dash så att man kan dasha upp/ned oxå

        finalMoveDir = (horVector + verVector).normalized * stagSpeedMultiplier * currMovementSpeed * (Mathf.Max(0.8f, powerManager.currPower) * 1.2f);

        if (IsWalkable(1.0f, 1.8f, (horVector + verVector).normalized, maxSlopeGrounded)) //dessa värden kan behöva justeras
        {

        }
        else
        {
            finalMoveDir *= 0.1f;
            //Debug.Log(Time.time.ToString());
            Break(1000, ref currMomentum);
            //currMomentum *= 0.1f;
        }

        //poängen i början ska dock vara värda mer!!
        float flatMoveStacksSpeedBonues = Mathf.Max(1, Mathf.Log(movementStacks, 1.01f));
        flatMoveStacksSpeedBonues *= 0.003f;
        //Debug.Log(flatMoveStacksSpeedBonues.ToString());

        float bonusStageSpeed = 1.0f; //ökar för vart X stacks
        bonusStageSpeed = movementStacks / 3;
        bonusStageSpeed = Mathf.Floor(bonusStageSpeed);
        bonusStageSpeed = Mathf.Max(1, bonusStageSpeed);

        bonusStageSpeed *= 0.22f;
        bonusStageSpeed += 1;

        //Debug.Log(bonusStageSpeed.ToString());

        currLimitSpeed = startLimitSpeed * bonusStageSpeed * currExternalSpeedMult;

        if (movementStacks > 4)
        {
            currMomentum += finalMoveDir * deltaTime * bonusStageSpeed * (1 + flatMoveStacksSpeedBonues); //om inte man är uppe i hög speed så kan man alltid köra currMomentum = finalMoveDir som vanligt
            float momY = currMomentum.y;

            Vector3 currMomXZ = new Vector3(currMomentum.x, 0, currMomentum.z);

            if (currMomXZ.magnitude > currLimitSpeed)
            {
                Break((25 - movementStacks * 0.3f), ref currMomXZ);

            }
            else
            {
                if (isGroundedRaycast) //släppt kontrollerna, då kan man deaccelerera snabbare! : finalMoveDir.magnitude <= 0.0f
                {
                    Break((6 - movementStacks * 0.2f), ref currMomXZ);
                    //Break(2, ref currMomXZ);
                }
            }

            currMomentum = new Vector3(currMomXZ.x, momY, currMomXZ.z);
        }
        else //vanlig slö speed
        {
            currMomentum = finalMoveDir * 0.21f * (1 + (float)movementStacks * 0.019f);
        }

        //if(finalMoveDir.magnitude > 0.0f)
        //{
        //    Break(0.99f);
        //}
        //else
        //{
        //    Break(0.9f);
        //}
    }

    void Break(float breakamount, ref Vector3 vec) //brmosa
    {
        if(breakamount < 0)
        {
            breakamount = 0.1f;
        }
        vec = Vector3.Lerp(vec, Vector3.zero, Time.deltaTime * breakamount); //detta är inte braa!
    }

    public void Stagger(float staggTime) //låser spelaren kvickt
    {
        cameraShaker.ShakeCamera(staggTime, 1f, true, true);
        StartCoroutine(DoStagg(staggTime));
    }

    IEnumerator DoStagg(float staggTime)
    {
        isLocked = true;
        yield return new WaitForSeconds(staggTime);
        isLocked = false;
    }

    public virtual void Jump()
    {
        if (jumpsAvaible > 0)
        {
            if (Time.time > jumpTimePoint + jumpCooldown)
            {
                AddMovementStack(1);
                PlayJumpEffect();

                jumpsAvaible = Mathf.Max(0, (jumpsAvaible - 1));
                dashUsed = false; //när man blir grounded så kan man använda dash igen, men oxå när man hoppar, SKILLZ!!!
                activePlatform = null; //när man hoppar så är man ej längre attached till movingplatform
                jumpTimePoint = Time.time;

                if (ySpeed < 0) //ska motverka gravitationen, behövs ej atm?
                    ySpeed = 0; //nollställer ej helt

                if (groundedRaycastObject != null && groundedRaycastObject.tag == "BreakerObject") //breakar objekt om man hoppar på dem
                {
                    groundedRaycastObject.GetComponent<BreakerObject>().Break();
                }

                if (ySpeed <= 0)
                {
                    ySpeed += jumpSpeed;
                }
                else
                {
                    ySpeed += (jumpSpeed * 0.8f); //mindre force när man redan har force
                }
                //animationH.Play(jump.name);
                //animationH[jump.name].weight = 1.0f;
            }
        }
    }

    public void Slam()
    {
        if (!isGrounded && !isGroundedRaycast && movementStacks < 5) return;

        RaycastHit rHit;
        float slamMaxDistance = 200;
        if(Physics.Raycast(transform.position, Vector3.down, out rHit, slamMaxDistance, groundCheckLM))
        {
            float dist = Mathf.Abs(transform.position.y - rHit.point.y);
            StartCoroutine(MoveSlam(dist));
        }

        StartCoroutine(MoveSlam(slamMaxDistance));
    }
    IEnumerator MoveSlam(float maxDistance)
    {
        Vector3 startP = transform.position;
        isLocked = true; //kan vara farligt att göra detta här
        yield return new WaitForSeconds(0.3f);
        isLocked = false;
        ySpeed = -170;
        while(!isGrounded && ySpeed < -2.0f && Vector3.Distance(startP, transform.position) < (maxDistance + 2))
        {
            currMomentum = Vector3.zero;
            ySpeed = -170;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log(Time.time.ToString());
        cameraShaker.ShakeCamera(0.3f + (movementStacks * 0.1f), 2 + (movementStacks * 0.2f), true, true);
        //Slam!
        AddMovementStack(-movementStacks);
    }

    public virtual bool Dash(Vector3 dir)
    {
        if (!IsDashReady()) return false;
        powerManager.SufficentPower(-dashPowerCost, true); //camerashake, konstig syntax kanske du tycker, men palla göra det fancy!
        StartCoroutine(MoveDash(dir));
        return true;
    }

    public virtual bool IsDashReady()
    {
        if (!powerManager.SufficentPower(-dashPowerCost)) return false;
        if (dashTimePoint + dashCooldown > Time.time) return false;
        if (dashUsed) return false;

        return true;
    }

    public virtual IEnumerator MoveDash(Vector3 dir)
    {
        maxDashTime = startMaxDashTime; //den kan utökas sen
        AddMovementStack(1);
        cameraShaker.ChangeFOV(0.5f, 90);
        ySpeed = -gravity * 0.01f; //nollställer ej helt
        dashUsed = true;
        ToggleDashEffect(true);
        powerManager.AddPower(-dashPowerCost);
        dashTimePoint = Time.time;
        currDashTime = 0.0f;
        float startDashTime = Time.time;
        while (currDashTime < maxDashTime)
        {
            speedBreakerTimer = Time.time + speedBreakerTime; //speedbreakern aktiveras sedan i update
            dashVel = dir * dashSpeed;

            Vector3 hitNormal = Vector3.zero;
            if(!IsWalkable(1.0f, 3, dashVel, maxSlopeGrounded, ref hitNormal)) //så den slutar dasha när den går emot en vägg
            {
                ToggleDashEffect(false);
                dashVel = Vector3.zero;

                //Debug.Log("Fixa Dot mojset!!");

                Stagger(0.4f);
                Break(1000, ref currMomentum);
                yield break;
            }
            currDashTime = Time.time - startDashTime;
            yield return null;
        }
        ToggleDashEffect(false);
        dashVel = Vector3.zero;

    }

    void AddMovementStack(int i)
    {
        movementStacks += i;
        float timeReduceValue = Mathf.Max(0, Mathf.Pow(movementStacks, 1.6f));
        float groundedReduceValue = Mathf.Max(0, Mathf.Pow(movementStacks, 1.7f));
        timeReduceValue *= 0.035f;
        groundedReduceValue *= 0.04f;
        if (float.IsNaN(timeReduceValue))
        {
            timeReduceValue = 0;
        }
        if (float.IsNaN(groundedReduceValue))
        {
            groundedReduceValue = 0;
        }
        //Debug.Log(timeReduceValue.ToString());
        movementStackResetTimer = Time.time + movementStackResetTime - (timeReduceValue); //gör det svårare o svårare!
        movementStackGroundedTimer = movementStackGroundedTime - (groundedReduceValue); //inte plus tid för den ligger redan inräknad

        movementStackGroundedTimer = Mathf.Max(0.2f, movementStackGroundedTimer); //ska som minst vara x sekunder

        //Debug.Log((movementStackResetTimer - Time.time).ToString());
        //Debug.Log((movementStackGroundedTimer).ToString());

        if (movementStacks < 1)
        {
            movementStacks = 1;
        }

        moveStackText.text = movementStacks.ToString();
    }


    public virtual void PlayAnimationStates()
    {
        if (animationH == null) return;
        float fadeLengthA = 0.1f;

        if (isGrounded || GetGrounded(groundCheckObject))
        {
            if (ver > 0.1f || ver < -0.1f) //för sig frammåt/bakåt
            {
                if (hor > 0.1f) //rär sig sidledes
                {
                    animationH.CrossFade(runForwardRight.name, fadeLengthA);
                }
                else if (hor < -0.1f)
                {
                    animationH.CrossFade(runForwardLeft.name, fadeLengthA);
                }
                else
                {
                    animationH.CrossFade(runForward.name, fadeLengthA);
                }
            }
            else if (hor > 0.1f) //rär sig sidledes
            {
                animationH.CrossFade(runForwardRight.name, fadeLengthA);
            }
            else if (hor < -0.1f)
            {
                animationH.CrossFade(runForwardLeft.name, fadeLengthA);
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

    public virtual void ApplyYForce(float velY) //till characterscontrollern, inte rigidbody
    {
        jumpTimePoint = Time.time;
        ySpeed += velY;
    }
    public virtual void ApplyYForce(float velY, float maxVel) //till characterscontrollern, inte rigidbody, med ett max värde
    {
        if (ySpeed >= maxVel) return;
        jumpTimePoint = Time.time;
        ySpeed += velY;
    }

    public virtual void ApplyExternalForce(Vector3 moveDir)
    {
        externalVel = moveDir;
    }

    public virtual void PlayJumpEffect()
    {
        if (gameObject.activeSelf == false) return;

        AudioSource dAS = jumpEffectObject.GetComponent<AudioSource>();
        ParticleTimed psTimed = jumpEffectObject.GetComponentInChildren<ParticleTimed>();

        if (dAS != null)
        {
            dAS.Play();
        }

        if (psTimed != null)
        {
            psTimed.StartParticleSystem();
        }
        
    }

    public virtual void ToggleDashEffect(bool b)
    {
        if (gameObject.activeSelf == false) return;
        dashEffectObject.transform.rotation = cameraHolder.rotation;
        float trailOriginalTime = 1.0f;
        float startWidth = 1;
        float endWidth = 0.1f;
        TrailRenderer[] tR = dashEffectObject.GetComponentsInChildren<TrailRenderer>();
        ParticleSystem[] pS = dashEffectObject.GetComponentsInChildren<ParticleSystem>();
        AudioSource dAS = dashEffectObject.GetComponent<AudioSource>();

        if (b)
        {
            if (dAS != null)
            {
                dAS.Play();
            }
        }

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
    public virtual IEnumerator ShutDownTrail(TrailRenderer tR)
    {
        while(tR.time > 0.0f)
        {
            tR.time -= 3 * deltaTime;
            tR.startWidth -= deltaTime;
            tR.endWidth -= deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public virtual void ApplySpeedMultiplier(float multiplier, float duration) //slows o liknande, updateras i LateUpdate()
    {
        currExternalSpeedMult = multiplier;
        moveSpeedMultTimePoint = Time.time;
        moveSpeedMultDuration = duration;
    }

    //void ToggleInfiniteGravity(bool b)
    //{
    //    pullField.enabled = b;
    //    ParticleSystem pullps = pullField.gameObject.GetComponent<ParticleSystem>();

    //    //pullps.emission.enabled = b;

    //    if (b)
    //    {
    //        pullps.Play();
    //    }
    //    else
    //    {
    //        pullps.Stop();
    //    }
    //}

    public virtual void ToggleDashReadyPS(bool b)
    {
        if (dashReadyPS == null) return;
        if (b)
        {
            if (!dashReadyPS.isPlaying)
                dashReadyPS.Play();
        }
        else
        {
            if (dashReadyPS.isPlaying)
                dashReadyPS.Stop();
        }
    }

    public virtual bool IsWalkable(float yOffset, float distance, Vector3 direction, float maxSlope)
    {
        RaycastHit rHit;
        if(Physics.Raycast(transform.position + new Vector3(0, yOffset, 0), direction, out rHit, distance, groundCheckLM))
        {

            float angleValue = Vector3.Angle(rHit.normal, Vector3.up);
            //Debug.Log(angleValue.ToString());

            if (angleValue > maxSlope)
            {
                return false;
            }
        }
        return true;
    }

    public virtual bool IsWalkable(float yOffset, float distance, Vector3 direction, float maxSlope, ref Vector3 hitNormal) //man kan få tillbaks väggens normal
    {
        //if(isGroundedRaycast)
        //{
        //    if (groundedSlope > maxSlopeGrounded) //lutning
        //    {
        //        if (Physics.Raycast(transform.position + new Vector3(0, yOffset, 0), direction, distance, groundCheckLM))
        //        {
        //            return false;
        //        }
        //    }
        //    return true; //man står på marken utan lutning, då ska man kunna gå
        //}
        RaycastHit rHit;
        if (Physics.Raycast(transform.position + new Vector3(0, yOffset, 0), direction, out rHit, distance, groundCheckLM))
        {
            hitNormal = rHit.normal;
            float angleValue = Vector3.Angle(rHit.normal, Vector3.up);
            //Debug.Log(angleValue.ToString());

            if (angleValue > maxSlope) //skicka in negativt om den ska ha hit mot allt
            {
                return false;
            }
        }
        return true;
    }

    public virtual bool GetGrounded()
    {
        RaycastHit rHit;

        if (Physics.Raycast(this.transform.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, groundedCheckDistance, groundCheckLM))
        {
            if (rHit.transform == this.transform || rHit.normal.y < 0.5f) {  return false; } //MEH DEN SKA EJ COLLIDA MED SIG SJÄLV

            groundedSlope = GetSlope(rHit.normal);
            groundedNormal = rHit.normal;

            if (groundedSlope > maxSlopeGrounded) {  return false; }

            if (isGroundedRaycast == false) //om man inte var grounded innan
            {
                groundedTimePoint = Time.time;
            }

            groundedRaycastObject = rHit.transform;
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool GetGrounded(Transform tChecker) //från en annan utgångspunkt
    {
        RaycastHit rHit;
        if (Physics.Raycast(tChecker.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, groundedCheckDistance, groundCheckLM))
        {
            if (rHit.transform == this.transform || rHit.normal.y < 0.5f) { return false; } //MEH DEN SKA EJ COLLIDA MED SIG SJÄLV

            groundedSlope = GetSlope(rHit.normal);
            groundedNormal = rHit.normal;

            if (groundedSlope > maxSlopeGrounded) { return false; }

            if (isGroundedRaycast == false) //om man inte var grounded innan
            {
                groundedTimePoint = Time.time;
            }
            groundedRaycastObject = rHit.transform;
            return true;
        }
        else
        {
            return false;
        }
    }


    public virtual Transform GetGroundedTransform(Transform tChecker) //får den transformen man står på, från en annan utgångspunkt
    {
        RaycastHit rHit;
        if (Physics.Raycast(tChecker.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, groundedCheckDistance, groundCheckLM))
        {
            if (rHit.transform == this.transform || rHit.normal.y < 0.5f) { return transform; } //MEH DEN SKA EJ COLLIDA MED SIG SJÄLV

            groundedSlope = GetSlope(rHit.normal);
            groundedNormal = rHit.normal;

            if (groundedSlope > maxSlopeGrounded) { return transform; }

            groundedRaycastObject = rHit.transform;
            return rHit.transform;
        }
        else
        {
            return transform;
        }
    }

    public virtual Transform GetGroundedTransform(Transform tChecker, float distance) //får den transformen man står på, från en annan utgångspunkt
    {
        RaycastHit rHit;
        if (Physics.Raycast(tChecker.position + new Vector3(0, groundedCheckOffsetY, 0), Vector3.down, out rHit, distance, groundCheckLM))
        {
            if (rHit.transform == this.transform || rHit.normal.y < 0.5f) { return transform; } //MEH DEN SKA EJ COLLIDA MED SIG SJÄLV

            groundedSlope = GetSlope(rHit.normal);
            groundedNormal = rHit.normal;

            if (groundedSlope > maxSlopeGrounded) { return transform; }

            groundedRaycastObject = rHit.transform;
            return rHit.transform;
        }
        else
        {
            return transform;
        }
    }

    public float GetSlope(Vector3 normalSurface)
    {
        if(normalSurface.y > 0.5f) //normalen är uppåt
        {
            return(Vector3.Angle(Vector3.down, -normalSurface));
        }
        else //normalen är neråt
        {
            return (Vector3.Angle(Vector3.down, normalSurface));
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