using UnityEngine;
using System.Collections;

public class StagSpeedBreaker : BaseClass {
    StagMovement stagMovement; //kunna skicka att man gjort hit osv

    Renderer[] renderers;
    Collider[] colliders;
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        stagMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<StagMovement>();
        colliders = GetComponentsInChildren<Collider>();
        renderers = GetComponentsInChildren<Renderer>();

        Disable();
    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        HealthSpirit h = col.GetComponent<HealthSpirit>();
        if(h != null)
        {
            stagMovement.maxDashTime += 0.5f;
            stagMovement.Stagger(0.2f);
            h.Die();
        }
    }

    public void Activate()
    {
        ToggleColliders(true);
        ToggleRenderers(true);
    }

    public void Disable()
    {
        ToggleColliders(false);
        ToggleRenderers(false);
    }

    void ToggleColliders(bool b)
    {
        for(int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = b;
        }
    }

    void ToggleRenderers(bool b)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = b;
        }
    }
}
