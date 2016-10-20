using UnityEngine;
using System.Collections;

public class AITraverser : AICharacterController
{
    public Transform traverseRCCheckOrigin;

    protected float upVelocity = 0;
    public LayerMask forwardRayLM;
    protected float forwardDistanceCheck = 10;

    public override void Init()
    {
        patrolState = new PatrolState(); //dessa behöver vara innan base.init, för där kallas reset
        chaseState = new ChaseState();
        guardPState = new GuardPState();

        if (traverseRCCheckOrigin == null)
            traverseRCCheckOrigin = transform;
        base.Init();

    }

    public override void Reset()
    {
        base.Reset();
        statePattern.ChangeState(guardPState);
    }

    //funktioner som kan användas

    public override void Move(Vector3 pos, float speed)
    {
        Vector3 modPos = new Vector3(pos.x, 0, pos.z);
        Vector3 modTPos = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 dir = (modPos - modTPos).normalized; //vill inte den ska röra sig upp o ned genom dessa vektorer
        upVelocity -= gravity;

        if(Physics.Raycast(traverseRCCheckOrigin.position, dir, forwardDistanceCheck, forwardRayLM))
        {
            upVelocity += gravity * 2;
        }

        Vector3 upVector = new Vector3(0, upVelocity, 0);

        cController.Move((dir * speed + upVector) * Time.deltaTime);
    }

    public override void StateEnded(AIState endedState)
    {
        if(endedState is GuardPState)
        {
            statePattern.ChangeState(chaseState);
        }
        else if (endedState is ChaseState)
        {
            statePattern.ChangeState(guardPState);
        }
    }
}
