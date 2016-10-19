using UnityEngine;
using System.Collections;

public class AIMoveable : AIEntity {
    [HideInInspector]
    public Transform target;

    //***speed and stats***
    [Header("MoveStats")]
    [HideInInspector]
    public float currMovementSpeed;
    public float normalMoveSpeed = 10;

    public float gravity = 10;
    //***speed and stats***

    //***patrol***
    [Header("Patrolling")]
    public Transform[] patrolPoints;
    protected int currPatrolPointIndex = 0;

    public float randomPatrol_MaxDistance = 50;
    //***patrol***

    protected PatrolState patrolState;
    protected ChaseState chaseState;

    public override void Init()
    {
        base.Init();
        patrolState = new PatrolState();
        chaseState = new ChaseState();

        statePattern.ChangeState(patrolState);
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
            return new Vector3(x, y, z);
        }
        else
        {
            currPatrolPointIndex = NextIndex(patrolPoints.Length, currPatrolPointIndex);
            return patrolPoints[currPatrolPointIndex].position;
        }
    }

    public virtual bool CheckForPlayer()
    {
        return true;
    }


    protected int NextIndex(int arrayLength, int currIndex)
    {
        currIndex++;
        if (currIndex >= arrayLength)
            currIndex = 0;
        return currIndex;
    }
}
