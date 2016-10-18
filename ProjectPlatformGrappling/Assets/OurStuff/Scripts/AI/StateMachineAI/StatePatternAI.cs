using UnityEngine;
using System.Collections;

public class StatePatternAI : BaseClass {
    protected AIEntity entity;
    protected AIState currState;
	// Use this for initialization

    public StatePatternAI(AIEntity entity)
    {
        this.entity = entity;
    }

    public override void UpdateLoop()
    {
        base.UpdateLoop();
        if (currState != null)
        {
            currState.Execute(entity);
        }
    }

    public void ChangeState(AIState newState)
    {
        if (currState != null)
        {
            currState.ForceExit(entity); //ForceExit för annars blir det en endless loop om de inte endar sig själva
        }
        currState = newState;
        currState.Enter(entity);
        currState.Execute(entity);
    }
}
