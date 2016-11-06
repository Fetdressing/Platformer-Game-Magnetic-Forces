using UnityEngine;
using System.Collections;

public class Mud : BaseClass { //slöar ner och gör så att spelaren inte kan dasha (hoppa ?)
    StagMovement stagMovement;

    private float moveSpeedChange = 0.80f;
    private static float timePointEntered;
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        stagMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<StagMovement>();
        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        timePointEntered = -5;
    }

    void Update()
    {
        if((timePointEntered + 0.2f) > Time.time)
        {
            stagMovement.ySpeed = -10;
            stagMovement.dashTimePoint = Time.time;
            stagMovement.ApplySpeedMultiplier(moveSpeedChange, 0.2f);
        }
    }

    void OnTriggerStay(Collider col)
    {
        if(col.tag == "Player")
        {
            timePointEntered = Time.time;            
        }
    }
}
