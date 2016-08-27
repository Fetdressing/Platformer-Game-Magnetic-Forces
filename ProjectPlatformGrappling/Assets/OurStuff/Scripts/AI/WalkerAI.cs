using UnityEngine;
using System.Collections;

public class WalkerAI : AIBase {

    [Header("Walker Stats")]
    public float walkSpeedForce = 1000;
    public float runSpeedForce = 2000;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
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
        if (currRigidbody.velocity.magnitude > 55)
        {
            ToggleRagdoll(true);
        }
        else if(isGrounded && GetGroundedDuration() > 2.0f)
        {
            ToggleRagdoll(false);
        }
    }

    void Update()
    {
        UpdateLoop();
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
                GetRandomNavmeshDestination(thisTransform.position, 35, out fleePos);
                SetDestination(fleePos);
            }
            yield return new WaitForSeconds(0.5f);

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
                changeDirTimeInterval = Time.time + 3;
                Vector3 fleePos;
                GetRandomNavmeshDestination(thisTransform.position, 15, out fleePos);
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
}
