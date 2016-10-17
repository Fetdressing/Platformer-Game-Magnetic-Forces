﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthSpirit : BaseClass
{
    private Transform thisTransform;
    private Rigidbody thisRigidbody;
    private AIBase aiBase;
    public Renderer[] thisRenderer; //om den inte sätts så kör den på default stuff
    private List<Material> thisMaterial = new List<Material>();
    public Material damagedMaterial;

    [HideInInspector]
    public Vector3 middlePoint; //var dennas mittpunkt ligger
    public float middlePointOffsetY = 0.5f;

    public int startHealth = 3;
    [HideInInspector]
    public int maxHealth; //public för att den skall kunna moddas från tex AgentStats
    private int currHealth;
    private Vector3 deathLocation; //spara ned vart denna dog, används för respawn
    private float transparentValue = 1; //hur mkt denna fadeas ut

    public int startHealthRegAmount = 1;
    [HideInInspector]
    public int healthRegAmount;
    private float healthRegIntervall = 5.0f;
    private float healthRegTimer = 0.0f;

    public bool destroyOnDeath = false;

    public GameObject animationObj;
    public AnimationClip deathAnimation;
    public GameObject deathParticleSystemObj;
    [HideInInspector]
    public GameObject deathParticleSystemSpawned;
    public float delayedDeathTime = 0;
    [HideInInspector]
    public bool isAlive = true;
    public bool isIndestructable = false;
    

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        thisTransform = this.transform;
        thisRigidbody = thisTransform.GetComponent<Rigidbody>();
        if (thisRenderer == null)
        {
            thisRenderer = GetComponentsInChildren<Renderer>();
        }
        aiBase = thisTransform.GetComponent<AIBase>();
        int i = 0;
        foreach (Renderer re in thisRenderer)
        {
            //Debug.Log(re.material.name);
            thisMaterial.Add(re.material);
            i++;
        }

        Reset();
        initTimes++;
    }

    public override void Reset()
    {
        base.Reset();
        maxHealth = startHealth; //maxHealth kan påverkas av andra faktorer also
        isAlive = true;
        SetHealth(maxHealth);

        healthRegAmount = startHealthRegAmount;
        isAlive = true;
        thisTransform.gameObject.SetActive(true);
        transparentValue = 1;
        ApplyTransparency(transparentValue);

        ApplyMaterial(damagedMaterial, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLoop();

    }

    public override void UpdateLoop()
    {
        if (initTimes == 0)
        {
            return;
        }
        base.UpdateLoop();

        if (isAlive == false)
        {
            transparentValue -= (Time.deltaTime * 0.0001f);
            ApplyTransparency(transparentValue);
            return;
        }

        middlePoint = new Vector3(thisTransform.position.x, thisTransform.position.y + middlePointOffsetY, thisTransform.position.z);

        if (healthRegTimer < Time.time)
        {
            healthRegTimer = Time.time + healthRegIntervall;
            AddHealth(healthRegAmount);
        }

        ApplyTransparency(Mathf.Clamp(transparentValue, 0.2f, 1));
    }

    public virtual bool AddHealth(int h)
    {
        if (isAlive == false) return false;
        if (isIndestructable == true) return false;

        currHealth += h;
        if (h < 0.0f)
        {
            ApplyMaterial(damagedMaterial, 0.5f);
        }

        if (currHealth > maxHealth)
        {
            currHealth = maxHealth;
        }
        else if (currHealth <= 0)
        {
            transparentValue = (float)currHealth / (float)maxHealth;
            Die();
            return false; //target dog
            //die
        }

        transparentValue = (float)currHealth / (float)maxHealth;
        //Debug.Log(currHealth.ToString());
        return true; //target vid liv
    }

    public void SetHealth(int h)
    {
        currHealth = h;
        if (currHealth > maxHealth)
        {
            currHealth = maxHealth;
        }

        transparentValue = (float)currHealth / (float)maxHealth;

        if (currHealth <= 0)
        {
            transparentValue = (float)currHealth / (float)maxHealth;
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("död");
        for (int i = 0; i < thisRenderer.Length; i++)
        {
            thisRenderer[i].material = thisMaterial[i];
        }
        isAlive = false;

        BaseClass[] baseclassObj = GetComponentsInChildren<BaseClass>();
        for(int i = 0; i < baseclassObj.Length; i++)
        {
            baseclassObj[i].Deactivate();
        }

        deathLocation = transform.position;
        //if (aiBase.GetComponent<AgentBase>() != null)
        //{
        //    aiBase.GetComponent<AgentBase>().agent.enabled = false;
        //}
        if (deathParticleSystemSpawned != null)
        {
            deathParticleSystemSpawned.transform.position = middlePoint;
            deathParticleSystemSpawned.GetComponent<ParticleTimed>().StartParticleSystem();
        }
        if (deathAnimation != null)
        {
            animationObj.GetComponent<Animation>().CrossFade(deathAnimation.name);
        }

        if (destroyOnDeath == true)
        {
            //if (thisTransform.GetComponent<AIBase>() != null)
            //{
            //    thisTransform.GetComponent<AIBase>().Dealloc();
            //}
            //Destroy(thisTransform.gameObject, delayedDeathTime);
        }
        else
        {
            StartCoroutine(DieDelayed());
        }
    }

    public IEnumerator DieDelayed()
    {
        yield return new WaitForSeconds(delayedDeathTime);
        thisTransform.gameObject.SetActive(false);
        if (transform.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("Manager").GetComponent<SpawnManager>().Respawn(deathLocation);
        }
    }

    public bool IsAlive()
    {
        if (currHealth > 0 && isAlive == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetCurrHealth()
    {
        return currHealth;
    }

    public void ApplyMaterial(Material m, float time)
    {
        if (thisTransform.gameObject.activeSelf == false) return;
        StartCoroutine(MarkMaterial(m, time));
    }

    void ApplyTransparency(float tVal)
    {
        for (int i = 0; i < thisMaterial.Count; i++)
        {
            Color c = thisMaterial[i].color;
            thisMaterial[i].color = new Color(c.r, c.g, c.b, tVal);
        }

        //for (int i = 0; i < thisRenderer.Length; i++)
        //{
        //    Color c = thisRenderer[i].material.color;
        //    thisRenderer[i].material.color = new Color(c.r, c.g, c.b, tVal);
        //}
    }

    public IEnumerator MarkMaterial(Material m, float time)
    {
        //thisRenderer.material = m;
        for (int i = 0; i < thisRenderer.Length; i++)
        {
            thisRenderer[i].material = m;
        }
        yield return new WaitForSeconds(time);
        for (int i = 0; i < thisRenderer.Length; i++)
        {
            thisRenderer[i].material = thisMaterial[i];
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        ////kolla så att man inte collidar med sig själv
        ////if(collision.)
        //float speed = collision.relativeVelocity.magnitude;
        //float ySpeed = Mathf.Abs(collision.relativeVelocity.y);

        //if (speed > speedDamageThreshhold)
        //{
        //    AddHealth(Mathf.Min(0, -(int)(speed - speedDamageThreshhold) + -(int)(ySpeed * 0.8f)));

        //    if (aiBase != null)
        //    {
        //        aiBase.ReportAttacked(collision.transform);
        //    }
        //}
    }

    void OnTriggerEnter(Collider col)
    {
        //return;
        //float speed = Vector3.Magnitude(col.transform.GetComponent<Rigidbody>().velocity - thisRigidbody.velocity);

        //if (speed > speedDamageThreshhold)
        //{
        //    AddHealth(Mathf.Min(0, -(int)(speed - speedDamageThreshhold)));

        //    if (aiBase != null)
        //    {
        //        aiBase.ReportAttacked(col.transform);
        //    }
        //}
    }

}

