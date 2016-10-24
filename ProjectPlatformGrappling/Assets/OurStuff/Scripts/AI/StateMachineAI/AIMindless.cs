using UnityEngine;
using System.Collections;

public class AIMindless : AICharacterController //denna rör sig bara mellan punkter
{
    protected float upVelocity = 0;

    protected float viewAngle = 25; //grader som denne ser target/spelaren på
    protected float viewDistance = 150;
    
    public override void Init()
    {
        patrolState = new PatrolState(); //dessa behöver vara innan base.init, för där kallas reset

        base.Init();

    }

    public override void Reset()
    {
        base.Reset();
        statePattern.ChangeState(patrolState);
    }

    //funktioner som kan användas
    public override void Update()
    {
        base.Update();

        if(IsTargetInFront())
        {
            currMovementSpeed = runMoveSpeed;
        }
        else
        {
            currMovementSpeed = normalMoveSpeed;
        }
    }

    public bool IsTargetInFront()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, viewDistance, targetSearchLM);

        for(int i = 0; i < col.Length; i++)
        {
            Vector3 vecToTar = col[i].transform.position - transform.position;
            if(Vector3.Angle(vecToTar, transform.forward) < viewAngle)
            {
                return true;
            }
        }

        return false;
    }

    public override void Move(Vector3 pos, float speed)
    {
        Vector3 modPos = new Vector3(pos.x, 0, pos.z);
        Vector3 modTPos = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 dir = (modPos - modTPos).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnRatio);

        upVelocity -= gravity;

        if (cController.isGrounded)
        {
            upVelocity = 0;
        }
        Vector3 upVector = new Vector3(0, upVelocity, 0);

        cController.Move((dir * speed + upVector) * Time.deltaTime);
    }

    public override void StateEnded(AIState endedState)
    {
        Debug.Log("Ska inte enda nått state");
    }
}
