using UnityEngine;
using System.Collections;

public class KillZone : BaseClass {
    private Transform thisTransform;
    private Collider[] thisColliders;

    public float phaseTime = 1.0f;
    public float phaseCooldown = 1.5f;
    public float startTime = 1.0f; //när hela börjar köras, kan behövas offset för att få dem ur fas

    public float damage = 1;

    private ParticleSystem ps;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        thisTransform = this.transform;

        thisColliders = thisTransform.GetComponentsInChildren<Collider>();
        ps = thisTransform.GetComponent<ParticleSystem>();

        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        StopAllCoroutines();
        StartCoroutine(KillZoneLifetime());
    }

    IEnumerator KillZoneLifetime()
    {
        yield return new WaitForSeconds(startTime);
        while (this != null)
        {
            yield return new WaitForSeconds(phaseCooldown);
            ToggleKillZone();
            yield return new WaitForSeconds(phaseTime);
            ToggleKillZone();
        }
    }

    public void ToggleKillZone()
    {
        bool b = thisColliders[0].enabled;

        if (b == false)
        {
            ps.Simulate(0.0f, true, true);
            ParticleSystem.EmissionModule psemit = ps.emission;
            psemit.enabled = true;
            ps.Play();
        }
        else
        {
            ps.Stop();
        }

        for (int i = 0; i < thisColliders.Length; i++)
        {
            thisColliders[i].enabled = !b;
        }
    }

    void OnTriggerStay(Collider col)
    {
        Health h = col.GetComponent<Health>();
        if (h != null)
        {
            h.AddHealth((int)((float)-damage*Time.deltaTime));
        }
    }
}
