using UnityEngine;
using System.Collections;

public class PhaseOutObject : BaseClass {
    private Transform thisTransform;
    private Renderer thisRenderer;
    private Collider[] thisColliders;
    private Material startMaterial;
    public Material phaseOutMaterial;

    public float phaseTime = 1.0f;
    public float phaseCooldown = 1.5f;
    public float startTime = 1.0f; //när hela börjar köras, kan behövas offset för att få dem ur fas
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        thisTransform = this.transform;
        thisRenderer = thisTransform.GetComponent<Renderer>();
        startMaterial = thisRenderer.material;

        thisColliders = thisTransform.GetComponentsInChildren<Collider>();

        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        StopAllCoroutines();
        StartCoroutine(PhaseLifetime());
    }

    IEnumerator PhaseLifetime()
    {
        yield return new WaitForSeconds(startTime);
        while(this != null)
        {
            yield return new WaitForSeconds(phaseCooldown);
            TogglePhase();
            yield return new WaitForSeconds(phaseTime);
            TogglePhase();
        }
    }

    public void TogglePhase()
    {
        bool b = thisColliders[0].enabled;

        if (b)
        {
            thisRenderer.material = phaseOutMaterial;
        }
        else
        {
            thisRenderer.material = startMaterial;
        }

        for(int i = 0; i < thisColliders.Length; i++)
        {
            thisColliders[i].enabled = !b;
        }
    }
}
