using UnityEngine;
using System.Collections;

public class StagSpeedBreaker : BaseClass {
    StagMovement stagMovement; //kunna skicka att man gjort hit osv

    Renderer[] renderers;
    Collider[] colliders;
    IEnumerator fadeOut;
    float startAlpha = 0.6f;

    Transform internalLastUnitHit; //används för o mecka collision
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
        initTimes++;
    }

    void OnTriggerEnter(Collider col)
    {
        HealthSpirit h = col.GetComponent<HealthSpirit>();
        if(h != null && h.IsAlive())
        {
            stagMovement.IgnoreCollider(false, internalLastUnitHit);

            internalLastUnitHit = col.transform;
            stagMovement.lastUnitHit = col.transform;
            stagMovement.IgnoreCollider(0.6f, col.transform); //så man inte collidar med den när man åker igenom
            //stagMovement.IgnoreCollider(true, col.transform);
            stagMovement.StartCoroutine(stagMovement.StagDash(true, 0.4f, 0.15f));
            //stagMovement.Dash(true, true); //använd kamera riktningen
            //Debug.Log("Felet med riktningen är att man kallar dash före stagger, gör så att de körs i rad");
            //stagMovement.Stagger(0.25f);
            h.AddHealth(-2);
        }
    }

    //IEnumerator StagDash()
    //{

    //}

    public void Activate()
    {
        ToggleColliders(true);
        ToggleRenderers(true);
    }

    public void Disable()
    {
        if (initTimes == 0) return;
        if (fadeOut != null || renderers[0].enabled == false) return;

        fadeOut = FadeOut(0.5f);
        StartCoroutine(fadeOut);
    }

    void ToggleColliders(bool b)
    {
        for(int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = b;
        }
    }

    IEnumerator FadeOut(float time)
    {
        
        float currAlpha = startAlpha;
        while (currAlpha > 0)
        {
            currAlpha -= startAlpha / ((startAlpha / Time.deltaTime) * time);
            if (currAlpha < 0) currAlpha = 0;

            for (int i = 0; i < renderers.Length; i++)
            {
                Color c = renderers[i].material.color;
                renderers[i].material.color = new Color(c.r, c.g, c.b, currAlpha);
            }
            yield return new WaitForEndOfFrame();
        }
        ToggleColliders(false);
        ToggleRenderers(false);
        fadeOut = null;
    }

    void ToggleRenderers(bool b)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = b;
            Color c = renderers[i].material.color;
            renderers[i].material.color = new Color(c.r, c.g, c.b, startAlpha);
        }
    }
}
