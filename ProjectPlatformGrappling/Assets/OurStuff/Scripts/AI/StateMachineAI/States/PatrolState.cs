using UnityEngine;
using System.Collections;
using System;

public class PatrolState : AIState {
    protected AIMoveable _AIMoveable;

    protected Vector3 currPatrolPoint = Vector3.zero;

    public override void Enter(AIEntity entity)
    {
        if (entity is AIMoveable)
            _AIMoveable = entity.GetComponent<AIMoveable>();
        else
        {
            Debug.Log("Entity inte kapabel att röra på sig");
        }
    }
    public override void Execute(AIEntity entity)
    {
        if(currPatrolPoint == Vector3.zero || HasReached(_AIMoveable.transform.position, currPatrolPoint, 5, false))
        {
            currPatrolPoint = _AIMoveable.GetPatrolPoint();
        }
        _AIMoveable.Move(currPatrolPoint, _AIMoveable.currMovementSpeed);
    }
    public override void Exit(AIEntity entity)
    {

        entity.StateEnded(PatrolState.Instance);
    }

    // singleton
    private static PatrolState instance;

    private static PatrolState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PatrolState();
            }

            return instance;
        }
    }
}
