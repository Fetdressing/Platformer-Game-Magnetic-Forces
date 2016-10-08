using UnityEngine;
using System.Collections;

public class PowerCharger : BaseClass {
    private Transform thisTransform;
    private Collider[] thisColliders;

    public float phaseTime = 1.0f;
    public float phaseCooldown = 1.5f;
    public float startTime = 1.0f; //när hela börjar köras, kan behövas offset för att få dem ur fas

    public float chargePower = 0.1f;

    private ParticleSystem ps;
    private Light lightActive;

    //ett particlesystem på den som de påverkar oxå, eller linerenderer
    private bool beamIsOnPlayer = false;
    private LineRenderer playerBeam;
    private Transform player;
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
        lightActive = thisTransform.GetComponent<Light>();

        try
        {
            player = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        }
        catch
        {
            Debug.Log("No player-Object");
        }
        playerBeam = thisTransform.GetComponent<LineRenderer>();

        Reset();
        initTimes++;
    }

    public override void Reset()
    {
        base.Reset();
        StopAllCoroutines();
        if (phaseTime != 0)
        {
            StartCoroutine(KillZoneLifetime());
        }
        else
        {
            ToggleKillZone(true);
        }
    }

    void Update()
    {

        if (playerBeam == null) return;
        if(beamIsOnPlayer)
        {
            playerBeam.enabled = true;
            Vector3[] positions = { transform.position, player.position };
            playerBeam.SetPositions(positions);
        }
        else
        {
            playerBeam.enabled = false;
        }
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
        if (initTimes == 0) return;
        bool b = thisColliders[0].enabled;

        if (b == false)
        {
            ps.Simulate(0.0f, true, true);
            ParticleSystem.EmissionModule psemit = ps.emission;
            psemit.enabled = true;
            ps.Play();

            if(lightActive != null)
            {
                lightActive.enabled = true;
            }
        }
        else
        {
            ps.Stop();
            if (lightActive != null)
            {
                lightActive.enabled = false;
            }
        }

        for (int i = 0; i < thisColliders.Length; i++)
        {
            thisColliders[i].enabled = !b;
        }
    }

    public void ToggleKillZone(bool b)
    {
        if (initTimes == 0) return;
        if (b == false)
        {
            ps.Simulate(0.0f, true, true);
            ParticleSystem.EmissionModule psemit = ps.emission;
            psemit.enabled = true;
            ps.Play();
            if (lightActive != null)
            {
                lightActive.enabled = true;
            }
        }
        else
        {
            ps.Stop();
            if (lightActive != null)
            {
                lightActive.enabled = false;
            }
        }

        for (int i = 0; i < thisColliders.Length; i++)
        {
            thisColliders[i].enabled = !b;
        }
    }

    void OnTriggerStay(Collider col)
    {
        //Health h = col.GetComponent<Health>();
        PowerManager pM = col.GetComponent<PowerManager>();
        if (pM != null)
        {
            pM.AddPower((int)((float)chargePower*Time.deltaTime));
        }

        if (!beamIsOnPlayer)
        {
            if (playerBeam == null) return;

            if (col.tag != "Player") return;
            StopAllCoroutines();
            StartCoroutine(PlacePlayerBeam());
        }
    }

    IEnumerator PlacePlayerBeam()
    {
        beamIsOnPlayer = true;
        yield return new WaitForSeconds(0.1f);
        beamIsOnPlayer = false;

    }
}
