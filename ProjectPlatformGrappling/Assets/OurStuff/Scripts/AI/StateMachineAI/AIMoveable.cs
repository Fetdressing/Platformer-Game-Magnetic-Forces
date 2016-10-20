using UnityEngine;
using System.Collections;

public class AIMoveable : AIEntity {
    [HideInInspector]
    public Transform target;

    [Header("Target Search")]
    public string[] targetTags = { "Player"};
    public LayerMask targetSearchLM;
    public float normalDetectRange = 280;
    protected float currDetectRange;

    //***speed and stats***
    [Header("MoveStats")]
    [HideInInspector]
    public float currMovementSpeed;
    public float normalMoveSpeed = 15;

    public float gravity = 10;
    //***speed and stats***

    //***patrol***
    [Header("Patrolling")]
    public Transform[] patrolPoints;
    protected int currPatrolPointIndex = 0;

    public float randomPatrol_MaxDistance = 50;
    protected Vector3 startPosition;
    //***patrol***

    protected PatrolState patrolState;
    protected ChaseState chaseState;
    protected GuardPState guardPState;

    public override void Init()
    {
        startPosition = transform.position;
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


    protected int NextIndex(int arrayLength, int currIndex)
    {
        currIndex++;
        if (currIndex >= arrayLength)
            currIndex = 0;
        return currIndex;
    }
}
