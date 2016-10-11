using UnityEngine;
using System.Collections;

public class ParticleTimed : BaseClass {
    private GameObject thisObject;
    private ParticleSystem ps;
    public float lifeTime = 3;

    [HideInInspector]
    public bool isReady = true;
	// Use this for initialization
	void Start () {
        Init();
    }
    void Awake()
    {
        Init();
    }

    public override void Init()
    {
        if (initTimes > 0) return;
        base.Init();
        thisObject = this.gameObject;
        ps = thisObject.GetComponent<ParticleSystem>();
        isReady = true;
    }

    public void StartParticleSystem()
    {
        StopAllCoroutines();
        thisObject.SetActive(true);
        ps.Simulate(0.0f, true, true);
        ParticleSystem.EmissionModule psemit = ps.emission;
        psemit.enabled = true;
        ps.Play();
        StartCoroutine(RunParticleSystem());
    }
    IEnumerator RunParticleSystem()
    {
        isReady = false;
        yield return new WaitForSeconds(lifeTime);
        isReady = true;
        thisObject.SetActive(false);
    }
}
