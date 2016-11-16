using UnityEngine;
using System.Collections;

public class AIMoveable : AIEntity {
    [HideInInspector]
    public Transform target;
    protected AnimStandardPlayer animPlayer;

    [Header("Target Search")]
    public string[] targetTags = { "Player"};
    public LayerMask targetSearchLM;
    public float normalDetectRange = 280;
    protected float currDetectRange;

    public float maxChaseDistance = 300;

    //***speed and stats***
    [Header("MoveStats")]
    [HideInInspector]
    public float currMovementSpeed;
    public float normalMoveSpeed = 15;
    public float runMoveSpeed = 30;

    public float gravity = 10;

    public float turnRatio = 10;

    [Header("Move Checks")]
    public LayerMask groundCheckLM;
    float maxSlopeGround = 28;
    public float yHeightOffsetCheck = 0;
    //***speed and stats***

    //***patrol***
    [Header("Patrolling")]
    public Transform[] patrolPoints;
    protected int currPatrolPointIndex = 0;
    [HideInInspector] public Vector3 currPatrolPoint = Vector3.zero;

    public float randomPatrol_MaxDistance = 50;
    protected Vector3 startPosition;
    //***patrol***

    protected PatrolState patrolState;
    protected ChaseState chaseState;
    protected GuardPState guardPState;

    public override void Init()
    {
        startPosition = transform.position;
        animPlayer = GetComponent<AnimStandardPlayer>();
        base.Init();

        //statePattern.ChangeState(patrolState);
    }

    public override void Reset() //kallas i AIEntity
    {
        base.Reset();
        currMovementSpeed = normalMoveSpeed;
        currDetectRange = normalDetectRange;

    }

    //funktioner som kan användas

    public virtual void Move(Vector3 pos, float speed)
    {

    }

    public virtual Vector3 GetPatrolPoint()
    {
        if(patrolPoints.Length == 0)
        {
            //helt random
            float x = Random.Range(-randomPatrol_MaxDistance, randomPatrol_MaxDistance);
            float y = Random.Range(-randomPatrol_MaxDistance, randomPatrol_MaxDistance) * 0.3f; //den behöver inte vara så stor
            float z = Random.Range(-randomPatrol_MaxDistance, randomPatrol_MaxDistance);
            return new Vector3(startPosition.x + x, startPosition.y + y, startPosition.z + z);
        }
        else
        {
            currPatrolPointIndex = NextIndex(patrolPoints.Length, currPatrolPointIndex);
            return patrolPoints[currPatrolPointIndex].position;
        }
    }

    public virtual bool CheckForTarget(ref Transform tar)
    {
        Collider[] potTargets = Physics.OverlapSphere(transform.position, currDetectRange, targetSearchLM);

        for (int i = 0; i < potTargets.Length; i++)
        {
            for (int y = 0; y < targetTags.Length; y++)
            {
                if (targetTags[y] == potTargets[i].tag)
                {
                    tar = potTargets[i].transform;
                    return true;
                }
            }
        }

        return false;
    }

    public float DistanceToT(Transform t)
    {
        return Vector3.Distance(t.position, transform.position);
    }

    public float DistanceToT2D(Transform t) //utan Y
    {
        Vector3 modT = new Vector3(t.position.x, 0, t.position.z);
        Vector3 modTransform = new Vector3(transform.position.x, 0, transform.position.z);
        return Vector3.Distance(modT, modTransform);
    }


    protected int NextIndex(int arrayLength, int currIndex)
    {
        currIndex++;
        if (currIndex >= arrayLength)
            currIndex = 0;
        return currIndex;
    }

    public bool IsWalkable()
    {
        if (Physics.Raycast(transform.position + new Vector3(0, yHeightOffsetCheck, 0) + (transform.forward * 4), Vector3.down, 15, groundCheckLM))
        {
            return true;
        }
        return false;
    }

    public bool IsWalkableFront(float yOffsetCheck, float distance) //distance kan nog runt 8-10 vara lämpligt
    {
        RaycastHit rHit;

        if(yHeightOffsetCheck > -0.01f && yHeightOffsetCheck < 0.01f)
        {
            yOffsetCheck = yHeightOffsetCheck;
        }

        if (Physics.Raycast(transform.position + new Vector3(0, yOffsetCheck, 0), transform.forward, out rHit, distance, groundCheckLM))
        {
            float angleValue = Vector3.Angle(rHit.normal, Vector3.up);
            //Debug.Log(angleValue.ToString());

            if (angleValue > maxSlopeGround)
            {
                return false;
            }
        }
        return true;
    }

    public void RotateTowards(Vector3 pos)
    {
        Vector3 modPos = new Vector3(pos.x, 0, pos.z);
        Vector3 modTPos = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 dir = (modPos - modTPos); //vill inte den ska röra sig upp o ned genom dessa vektorer

        if (dir.magnitude < 4) return; //så att om man är ovanför så ska den inte försöka titta på en

        dir = dir.normalized;

        if (dir == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnRatio);
    }
}
