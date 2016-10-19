using UnityEngine;
using System.Collections;

public class ChaseState : AIState {
    AIMoveable _AIMoveable;
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
        while (_AIMoveable.target != null)
        {
            _AIMoveable.Move(_AIMoveable.target.position, _AIMoveable.currMovementSpeed);
        }
        Exit(entity);
    }
    public override void Exit(AIEntity entity)
    {

        entity.StateEnded(ChaseState.instance);
    }

    // singleton
    private static ChaseState instance;

    public static ChaseState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ChaseState();
            }

            return instance;
        }
    }
}
