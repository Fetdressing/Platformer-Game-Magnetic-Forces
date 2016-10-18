using UnityEngine;
using System.Collections;

public abstract class AIState {
    public abstract void Enter(AIEntity entity);
    public abstract void Execute(AIEntity entity);
    public abstract void Exit(AIEntity entity);
    public virtual void ForceExit(AIEntity entity)
    {

    }
}
