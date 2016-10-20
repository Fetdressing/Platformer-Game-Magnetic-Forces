using UnityEngine;
using System.Collections;

public abstract class AIState {
    public abstract void Enter(AIEntity entity);
    public abstract void Execute(AIEntity entity);
    public abstract void Exit(AIEntity entity); //säger till AIn att byta state
    public virtual void ForceExit(AIEntity entity)
    {

    }

    public virtual bool HasReached(Vector3 reacher, Vector3 reachPos, float distanceMargin)
    {
        if (Vector3.Distance(reacher, reachPos) < distanceMargin)
        {
            return true;
        }
        return false;
    }

    public virtual bool HasReached(Vector3 reacher, Vector3 reachPos, float distanceMargin, bool useY)
    {
        Vector3 modReacher, modReachPos;
        if(!useY)
        {
            modReacher = new Vector3(reacher.x, 0, reacher.z);
            modReachPos = new Vector3(reachPos.x, 0, reachPos.z);
        }
        else
        {
            modReacher = reacher;
            modReachPos = reachPos;
        }

        if (Vector3.Distance(modReacher, modReachPos) < distanceMargin)
        {
            return true;
        }
        return false;
    }
}
