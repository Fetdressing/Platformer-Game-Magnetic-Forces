using UnityEngine;
using System.Collections;

public class BreakerObject : BaseClass {
    private Transform thisTransform;
    private Renderer thisRenderer;
    private Collider[] thisColliders;


    private Material startMaterial;
    public Material phaseOutMaterial;

    public float fadeAmount = 10;

    public float fadeTime = 4;
    float currAlpha = 1;

    private bool fading = false;
    // Use this for initialization
    void Start()
    {
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
        fading = false;
        currAlpha = 1;
        //StartCoroutine(PhaseLifetime());
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            if (fading)
                return;
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        fading = true;
        
        while (currAlpha > 0.0f)
        {
            Color c = thisRenderer.material.color;
            
            currAlpha -= 1 / ((1 / Time.deltaTime) * fadeTime);
            thisRenderer.material.color = new Color(c.r, c.g, c.b, currAlpha);

            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < thisColliders.Length; i++)
        {
            thisColliders[i].enabled = false;
        }

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color wC;

        //yield return new WaitForSeconds(fadeTime);

        while (currAlpha < 1.0f)
        {
            Color c = thisRenderer.material.color;
            
            currAlpha += 1 / ((1 / Time.deltaTime) * fadeTime);
            thisRenderer.material.color = new Color(c.r, c.g, c.b, currAlpha);
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < thisColliders.Length; i++)
        {
            thisColliders[i].enabled = true;
        }
        fading = false;
    }
}
