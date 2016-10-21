using UnityEngine;
using System.Collections;

public class AIMindless : AICharacterController //denna rör sig bara mellan punkter
{

    public override void Init()
    {
        patrolState = new PatrolState(); //dessa behöver vara innan base.init, för där kallas reset

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

        Vector3 dir = (modPos - modTPos).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnRatio);

        cController.Move((dir * speed) * Time.deltaTime);
    }

    public override void StateEnded(AIState endedState)
    {
        Debug.Log("Ska inte enda nått state");
    }
}
