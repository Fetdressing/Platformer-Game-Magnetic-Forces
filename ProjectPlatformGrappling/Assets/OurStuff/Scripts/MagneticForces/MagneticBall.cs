using UnityEngine;
using System.Collections;

public class MagneticBall : MagneticForce
{
    [HideInInspector]
    public Transform homeTransform;
    [HideInInspector]
    public Vector3 homePos;

    [HideInInspector]
    public MagneticBallState magneticBallState = MagneticBallState.HeadingHome;

    private Transform player;

    public float playerForce = 15000;

    private float cooldownTime = 3.0f;
    private float cooldownTimer = 0.0f;
    public Material cooldownMat;

    private float cooldownTimeCallback = 0.4f;
    private float cooldownTimerCallback = 0.0f;

    public LineRenderer lineRendererBindPlayer;
	// Use this for initialization
	void Start () {
        Init();
    }

    public override void Init()
    {
        base.Init();
        thisRigidbody = thisTransform.GetComponent<Rigidbody>();
        thisRigidbody.isKinematic = false;
        lineRendererBindPlayer = thisTransform.GetComponent<LineRenderer>();
        SetState(MagneticBallState.HeadingHome);
        cooldownTimer = 0.0f;
    }

    public void SetStartTransform(Transform t) //absolete
    {
        homeTransform = t;
        initTimes++;
    }

    public void SetStartPosition(Vector3 pos)
    {
        homePos = pos;
        initTimes++;
    }

    public void SetPlayer(Transform p)
    {
        player = p;
    }

    // Update is called once per frame
    void Update () {
        UpdateLoop();
    }

    public override void UpdateLoop()
    {
        base.UpdateLoop();
        if (initTimes == 0) return;
        switch (magneticBallState)
        {
            case MagneticBallState.HeadingHome:
                HeadHome();
                break;
            case MagneticBallState.HeadingToTarget:
                ApplyForce();
                break;
            case MagneticBallState.ApplyingGravity:
                ApplyForce();
                break;
        }

        if (cooldownTimer > Time.time && magneticBallState == MagneticBallState.HeadingHome) //cooldown, ska bara visas när bollen är hemma o idlar
        {
            SetCurrColor(Color.yellow);
            SetCurrMaterial(cooldownMat);
        }
        else
        {
            SetCurrColor(normalColor);
            SetCurrMaterial(normalMat);
        }

        Vector3[] positionArray = new[] { thisTransform.position, player.position };
        lineRendererBindPlayer.SetPositions(positionArray);
    }

    public override void FixedUpdateLoop()
    {
        //ingenting, för tydligen körs Updates via arv
        lineRendererBindPlayer.enabled = false;
        if(Input.GetKey(KeyCode.F))
        {
            if (magneticBallState != MagneticBallState.HeadingHome)
            {
                lineRendererBindPlayer.enabled = true;
                ApplyForceTarget(player, playerForce);
            }
        }
    }

    public void SetState(MagneticBallState bS)
    {
        magneticBallState = bS;
        switch (magneticBallState)
        {
            case MagneticBallState.HeadingHome:
                holoRangeTransform.gameObject.SetActive(false);
                ps.enableEmission = false;
                pLight.enabled = false;
                thisRigidbody.isKinematic = true;
                break;
            case MagneticBallState.HeadingToTarget:
                holoRangeTransform.gameObject.SetActive(false);
                ps.enableEmission = false;
                pLight.enabled = true;
                thisRigidbody.isKinematic = false; //forces ska applyas
                break;
            case MagneticBallState.ApplyingGravity:
                holoRangeTransform.gameObject.SetActive(true);
                ps.enableEmission = true;
                pLight.enabled = true;
                thisRigidbody.isKinematic = true;
                break;
        }
    }

    void HeadHome()
    {
        thisTransform.position = Vector3.Slerp(thisTransform.position, homePos, Time.deltaTime * 10f);
    }

    void OnTriggerEnter(Collider col)
    {
        if (magneticBallState == MagneticBallState.HeadingToTarget)
        {
            SetState(MagneticBallState.ApplyingGravity);
        }
        return;
        if(magneticBallState == MagneticBallState.ApplyingGravity)
        {
            if(col.gameObject.tag == "MagneticBall") //en annan kula
            {
                MagneticBall mForce = col.gameObject.GetComponent<MagneticBall>();
                if(mForce.magneticBallState == MagneticBallState.HeadingToTarget)
                {
                    //slå ihop dem
                }
            }
        }
    }

    public void OrderHeadHome()
    {
        if (cooldownTimerCallback > Time.time) return;
        StopAllCoroutines();
        cooldownTimer = cooldownTime + Time.time;
        SetState(MagneticBallState.HeadingHome);
    }
    public void OrderFire(float force, float stayTime)
    {
        if (cooldownTimer > Time.time) return;
        cooldownTimerCallback = cooldownTimeCallback + Time.time;
        StopAllCoroutines();
        SetState(MagneticBallState.HeadingToTarget);
        thisRigidbody.AddForce(thisRigidbody.transform.forward * force, ForceMode.Impulse);
        StartCoroutine(FlyTimeChecker(stayTime));
    }

    IEnumerator FlyTimeChecker(float time)
    {
        yield return new WaitForSeconds(time);
        if(magneticBallState == MagneticBallState.HeadingToTarget)
        {
            OrderHeadHome();
        }
    }
    
}
public enum MagneticBallState { HeadingHome, HeadingToTarget, ApplyingGravity };
