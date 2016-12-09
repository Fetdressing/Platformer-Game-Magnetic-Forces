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


    void OnTriggerEnter(Collider col)
    {
        HealthSpirit h = col.GetComponent<HealthSpirit>();
        if(h != null)
        {
            stagMovement.IgnoreCollider(0.8f, col.transform); //så man inte collidar med den när man åker igenom
            stagMovement.Dash(true); //använd kamera riktningen
            stagMovement.Stagger(0.25f);
            h.AddHealth(-2);
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
