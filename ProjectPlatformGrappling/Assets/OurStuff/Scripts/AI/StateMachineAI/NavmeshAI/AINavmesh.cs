using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class AINavmesh : AIMoveable {
    protected NavMeshAgent agent;
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        patrolState = new PatrolState(); //dessa behöver vara innan base.init, för där kallas reset
        chaseState = new ChaseState();
        guardPState = new GuardPState();

        base.Init();

        agent = GetComponent<NavMeshAgent>();
    }

    public override void Reset()
    {
        base.Reset();
        statePattern.ChangeState(guardPState);
    }

    public override void Move(Vector3 pos, float speed)
    {
        agent.Resume();
        agent.speed = speed;
        agent.SetDestination(pos);
    }

    public override void Stop()
    {
        base.Stop();
        agent.Stop();
    }

    public override Vector3 GetPatrolPoint() //returnerar vector3.zero ifall den ej klarar
    {
        if (newPatrolTimer > Time.time) return Vector3.zero;
        newPatrolTimer = Time.time + newPartrolCD;

        Vector3 tempPatrol;

        for (int i = 0; i < 30; i++)
        {

            if (patrolPoints.Length == 0)
            {
                //helt random
                float x = Random.Range(-randomPatrol_MaxDistance, randomPatrol_MaxDistance);
                float y = Random.Range(-randomPatrol_MaxDistance, randomPatrol_MaxDistance) * 0.3f; //den behöver inte vara så stor
                float z = Random.Range(-randomPatrol_MaxDistance, randomPatrol_MaxDistance);
                tempPatrol = new Vector3(startPosition.x + x, startPosition.y + y, startPosition.z + z);
            }
            else
            {
                currPatrolPointIndex = NextIndex(patrolPoints.Length, currPatrolPointIndex);
                tempPatrol = patrolPoints[currPatrolPointIndex].position;
            }
            
            NavMeshHit hitH;
            if (NavMesh.SamplePosition(tempPatrol, out hitH, 1.0f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(transform.position, hitH.position, NavMesh.AllAreas, path); //kolla ifall det är en legit path

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    return hitH.position;
                }
            }
        }

        return Vector3.zero;
    }

    public override bool IsWalkable()
    {

        return true;
    }

    public override bool IsWalkableFront()
    {
        return true;
    }

    public override void RotateTowards(Vector3 pos)
    {
        return;
    }
}
