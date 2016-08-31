using UnityEngine;
using System.Collections;

public class WalkerAI : AIBase {

    [Header("Walker Stats")]
    public float walkSpeedForce = 1000;
    public float runSpeedForce = 2000;
    public float walkArea = 100;

    [Header("Animation")]
    public AnimationClip idle;
    public AnimationClip run;
    public float loopRunAnimSpeed = 1.4f;
    public float loopIdleAnimSpeed = 0.8f;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        animationH[run.name].speed = loopRunAnimSpeed;
        animationH[idle.name].speed = loopIdleAnimSpeed;

        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        StartCoroutine(WalkRandom(15));
    }

    void FixedUpdate()
    {
        if (initTimes == 0) return;
        isGrounded = GetGrounded(thisTransform);

        if (!IsTransformCloseEnoughToAgent() && isGrounded)
        {
            MoveTowardsDestination(agentTransform.position, currMoveForce);
        }

        //Debug.Log("men va" + GetGroundedDuration().ToString());
        if (currRigidbody.velocity.magnitude > thisHealth.speedDamageThreshhold || !isGrounded)
        {
            ToggleRagdoll(true);
        }
        else if(IsStanding())
        {
            //Debug.Log("Stått tillräkligt länge" + Time.time.ToString());
            ToggleRagdoll(false);
        }

        PlayStateAnimations();
    }

    IEnumerator WalkRandom(float time)
    {
        currMoveForce = walkSpeedForce;
        float endTime = Time.time + time;
        float changeDirTimeInterval = 0f;

        while (endTime > Time.time)
        {
            if (changeDirTimeInterval < Time.time)
            {
                changeDirTimeInterval = Time.time + 5;
                Vector3 fleePos;
                GetRandomNavmeshDestination(thisTransform.position, walkArea, out fleePos);
                SetDestination(fleePos);
            }
            yield return new WaitForSeconds(0.1f);

        }

        StopAllCoroutines();
        StartCoroutine(DoIdle(Random.Range(2,7)));
    }

    IEnumerator DoIdle(float time)
    {
        ResetPath();
        yield return new WaitForSeconds(time);

        StopAllCoroutines();
        StartCoroutine(WalkRandom(15));
    }

    IEnumerator Flee(float time)
    {
        currMoveForce = runSpeedForce;
        float endTime = Time.time + time;
        float changeDirTimeInterval = 0f;

        while (endTime > Time.time)
        {
            if (changeDirTimeInterval < Time.time)
            {
                changeDirTimeInterval = Time.time + 1.5f;
                Vector3 fleePos;
                GetRandomNavmeshDestination(thisTransform.position, walkArea * 0.3f, out fleePos);
                SetDestination(fleePos);
            }
            yield return new WaitForSeconds(0.5f);

        }

        StopAllCoroutines();
        StartCoroutine(DoIdle(Random.Range(2, 7)));
    }

    public override void ReportAttacked(Transform t)
    {
        //base.ReportAttacked(t);
        StopAllCoroutines();
        StartCoroutine(Flee(10));
    }

    public override void PlayStateAnimations()
    {
        if (thisRigidbody.velocity.magnitude > 0.0f && isGrounded)
        {
            animationH[run.name].weight = 0.2f;
            animationH[run.name].layer = 10;
            animationH[run.name].speed = loopRunAnimSpeed;// * ((float)movementSpeed / (float)startMovementSpeed);
            animationH.CrossFade(run.name);
        }
        else
        {
            animationH[idle.name].weight = 0.2f;
            animationH[idle.name].layer = 10;
            animationH.CrossFade(idle.name);
        }
    }
}
