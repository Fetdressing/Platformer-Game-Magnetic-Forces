using UnityEngine;
using System.Collections;

public class AIBase : BaseClass {
    [HideInInspector]
    public Transform thisTransform;
    [HideInInspector]
    public Rigidbody thisRigidbody;
    [HideInInspector]
    public Animation animationH;

    public Transform agentTransform;
    [HideInInspector]
    public NavMeshAgent agent;

    public float moveForce = 500;
    //look angle threshhold, hur mycket den måste titta på fienden för att kunna attackera
    private float lookAngleThreshhold = 15;
    public float turnRatio = 2;
    // Use this for initialization

    public override void Init()
    {
        base.Init();
        thisTransform = this.transform;
        agent = agentTransform.GetComponent<NavMeshAgent>();
        animationH = thisTransform.GetComponent<Animation>();
    }

    public override void Reset()
    {
        base.Reset();
        ReturnAgent();
    }

    //***agent förflyttning***
    public virtual void SetDestination(Vector3 pos)
    {
        if (!IsReadyToMove()) return;
        agent.SetDestination(pos);
    }

    public void ResetPath() //så att den inte fuckar när den är av, mer safe
    {
        if (IsReadyToMove())
        {
            agent.ResetPath();
            ReturnAgent();
        }
    }

    public void ReturnAgent() //förflyttar den till sin startposition
    {
        NavMeshHit nhit;
        if (NavMesh.SamplePosition(thisTransform.position, out nhit, 1.0f, NavMesh.AllAreas))
        {
            agentTransform.position = nhit.position;
        }
    }
    //***agent förflyttning***

    //***thistransform förflyttning***
    public virtual void MoveTowardsDestination(Vector3 pos)
    {
        if (!IsReadyToMove()) return;
        RotateTowards(pos);
        thisRigidbody.AddForce(thisTransform.forward * moveForce * Time.deltaTime);
    }

    public void RotateTowards(Vector3 t)
    {
        Vector3 tPosWithoutY = new Vector3(t.x, thisTransform.position.y, t.z); //så den bara kollar på x o z leden
        Vector3 direction = (tPosWithoutY - thisTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        thisTransform.rotation = Quaternion.Slerp(thisTransform.rotation, lookRotation, Time.deltaTime * turnRatio);
    }
    //***thistransform förflyttning***

   
    public virtual bool IsReadyToMove()
    {
        if (agent == null) return false;
        if (thisTransform.gameObject.activeSelf == false) return false;
        if (agent.isOnNavMesh == false) return false;
        if (agent.enabled == false) return false;

        return true;
    }

    public bool IsFacingTransform(Transform t)
    {
        Vector3 tPosWithoutY = new Vector3(t.position.x, thisTransform.position.y, t.position.z); //så den bara kollar på x o z leden
        Vector3 vecToTransform = tPosWithoutY - thisTransform.position;
        float angle = Vector3.Angle(thisTransform.forward, vecToTransform);

        if (angle > lookAngleThreshhold)
        {
            //Debug.Log(angle.ToString());
            return false;
        }
        return true;
    }


    public virtual void ToggleRagdoll(bool b)
    {
        if (animationH != null)
        {
            animationH.enabled = !b;
        }
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {            
            if (rb != thisRigidbody)
            {
                rb.isKinematic = b;
            }
        }
    }

    //public bool HasReachedPosition(Vector3 pos) //krävs att man har en path
    //{
    //    agent.SetDestination(pos);
    //    if (GetDistanceToPosition(pos) < 1.5f)
    //    {
    //        return true;
    //    }
    //    if (agent.pathPending) //så agent.remainingDistance funkar (den är inte klar med o beräkna så vänta lite)
    //    {
    //        return false;
    //    }
    //    if (agent.remainingDistance < 1.5f)
    //    {
    //        return true;
    //    }
    //    return false;
    //}
}
