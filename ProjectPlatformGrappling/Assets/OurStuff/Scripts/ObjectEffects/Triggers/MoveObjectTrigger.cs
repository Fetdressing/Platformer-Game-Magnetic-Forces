using UnityEngine;
using System.Collections;

public class MoveObjectTrigger : Trigger {
    public Transform moveObject;
    private Rigidbody moveRigidbody;
    public float moveSpeed = 9.0f;

    private Vector3 currMoveposition;
    public Transform idlePosition;
    public Transform triggerPosition;

    public override void Init()
    {
        base.Init();
        moveRigidbody = moveObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ToggleTrigger(GetTriggered());
        if(Vector3.Distance(moveObject.position, currMoveposition) > 3.0f)
        {
            Vector3 toTarget = (currMoveposition - moveObject.position).normalized;
            moveRigidbody.MovePosition(moveObject.position + toTarget * Time.deltaTime * moveSpeed);
        }
    }

    public override void StartTrigger()
    {
        base.StartTrigger();
        currMoveposition = triggerPosition.position;
    }

    public override void ExitTrigger()
    {
        base.ExitTrigger();
        currMoveposition = idlePosition.position;
    }
}
