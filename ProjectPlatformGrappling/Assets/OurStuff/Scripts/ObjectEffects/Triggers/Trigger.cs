using UnityEngine;
using System.Collections;

public class Trigger : BaseClass {
    [HideInInspector]
    public bool isTriggered = false;

    public ParticleSystem psActivated;
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        ToggleTrigger(false);
        //psActivated = this.transform.GetComponent<ParticleSystem>();
    }

    void OnTriggerEnter(Collider col)
    {
        ToggleTrigger(true);
    }

    void OnTriggerExit(Collider col)
    {
        ToggleTrigger(false);
    }

    void OnCollisionEnter(Collision col)
    {
        ToggleTrigger(true);
    }

    void OnCollisionExit(Collision col)
    {
        ToggleTrigger(false);
    }

    public void ToggleTrigger(bool b)
    {
        if(b)
        {
            psActivated.Simulate(0.0f, true, true);
            ParticleSystem.EmissionModule psemit = psActivated.emission;
            psemit.enabled = true;
            psActivated.Play();

            isTriggered = true;
        }
        else
        {
            psActivated.Stop();

            isTriggered = false;
        }
    }
}
