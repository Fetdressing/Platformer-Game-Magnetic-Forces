﻿using UnityEngine;
using System.Collections;

public class BreakerObject : BaseClass {
    private Transform thisTransform;
    public Renderer thisRenderer;
    private Collider[] thisColliders;


    private Material startMaterial;
    public Material phaseOutMaterial;

    public float fadeAmount = 10;

    public float fadeTime = 4;
    float currAlpha = 1;

    private bool fading = false;

    public Animation animationH;
    public AnimationClip breakAnim;
    public AnimationClip renewAnim;

    public AnimationClip idleAnim;
    public AnimationClip breakingAnim;
    public float animSpeed = 1.0f;
    public float idleASpeed = 1.0f;
    // Use this for initialization
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        thisTransform = this.transform;

        if (thisRenderer == null)
        {
            thisRenderer = thisTransform.GetComponent<Renderer>();
        }

        startMaterial = thisRenderer.material;

        thisColliders = thisTransform.GetComponentsInChildren<Collider>();

        if (animationH == null)
        {
            animationH = transform.GetComponent<Animation>();
            animationH[breakingAnim.name].speed = idleASpeed;
        }

        transform.tag = "BreakerObject";

        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        StopAllCoroutines();
        fading = false;
        currAlpha = 1;

        Color c = thisRenderer.material.color;
        thisRenderer.material.color = new Color(c.r, c.g, c.b, currAlpha);
        //StartCoroutine(PhaseLifetime());
    }

    //void OnTriggerEnter(Collider col)
    //{
    //    if(col.tag == "Player")
    //    {
    //        if (fading)
    //            return;
    //        StartCoroutine(FadeOut());
    //    }
    //}

    void Update()
    {
        if(!fading)
        {
            if (animationH != null && animationH.isPlaying == false)
            {
                animationH.CrossFade(idleAnim.name);
            }
        }
    }

    public void Break() //låt någon kalla på det
    {
        if (fading)
            return;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        fading = true;
        
        while (currAlpha > 0.0f)
        {
            Color c = thisRenderer.material.color;
            
            currAlpha -= 1 / ((1 / Time.deltaTime) * fadeTime);
            thisRenderer.material.color = new Color(c.r, c.g, c.b, currAlpha);

            if(animationH != null)
            {
                animationH[breakingAnim.name].speed = animSpeed * (1 - currAlpha);
                animationH.CrossFade(breakingAnim.name);
            }

            yield return new WaitForEndOfFrame();
        }

        animationH.Play(breakAnim.name);

        for (int i = 0; i < thisColliders.Length; i++)
        {
            thisColliders[i].enabled = false;
        }

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color wC;
        Color c;

        c = thisRenderer.material.color;

        while (currAlpha < 1.0f)
        {
            c = thisRenderer.material.color;
            
            currAlpha += 1 / ((1 / Time.deltaTime) * (fadeTime * 2));
            thisRenderer.material.color = new Color(c.r, c.g, c.b, currAlpha * 0.2f);

            yield return new WaitForEndOfFrame();
        }

        animationH.Play(renewAnim.name);

        for (int i = 0; i < thisColliders.Length; i++)
        {
            thisColliders[i].enabled = true;
        }
        thisRenderer.material.color = new Color(c.r, c.g, c.b, currAlpha);
        fading = false;
    }
}
