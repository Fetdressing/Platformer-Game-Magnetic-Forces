using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Health : MonoBehaviour {
    private Transform thisTransform;
    private Rigidbody thisRigidbody;
    [HideInInspector]
    public Renderer[] thisRenderer;
    private List<Material> thisMaterial = new List<Material>();
    public Material damagedMaterial;
    private int initializedTimes = 0;

    private Camera mainCamera;
    public GameObject uiHealth;
    public Image healthBar;

    public Transform uiCanvas;

    [HideInInspector]
    public Vector3 middlePoint; //var dennas mittpunkt ligger
    public float middlePointOffsetY = 0.5f;

    public int startHealth = 100;
    [HideInInspector]
    public int maxHealth; //public för att den skall kunna moddas från tex AgentStats
    private int currHealth;

    public int startHealthRegAmount = 1;
    [HideInInspector]
    public int healthRegAmount;
    private float healthRegIntervall = 0.8f;
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

    private int speedDamageThreshhold = 80;

    void Start()
    {
        thisTransform = this.transform;
        thisRigidbody = thisTransform.GetComponent<Rigidbody>();
        thisRenderer = GetComponentsInChildren<Renderer>();
        int i = 0;
        foreach (Renderer re in thisRenderer)
        {
            //Debug.Log(re.material.name);
            thisMaterial.Add(re.material);
            i++;
        }

        mainCamera = Camera.main;

        maxHealth = startHealth; //maxHealth kan påverkas av andra faktorer also
        AddHealth(maxHealth);

        healthRegAmount = startHealthRegAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive == false)
        {
            if (uiCanvas != null)
            {
                uiCanvas.gameObject.SetActive(false);
            }
            return;
        }

        if (initializedTimes == 0)
        {
            return;
        }

        middlePoint = new Vector3(thisTransform.position.x, thisTransform.position.y + middlePointOffsetY, thisTransform.position.z);

        if (uiCanvas != null)
        {
            uiCanvas.LookAt(uiCanvas.position + mainCamera.transform.rotation * Vector3.forward,
    mainCamera.transform.rotation * Vector3.up); //vad gör jag med saker som bara har health då?
        }

        if (uiHealth != null)
        {
            if (currHealth >= maxHealth)
            {
                uiHealth.SetActive(false);
            }
            else
            {
                uiHealth.SetActive(true);
            }
        }

        if (healthRegTimer < Time.time)
        {
            healthRegTimer = Time.time + healthRegIntervall;
            AddHealth(healthRegAmount);
        }

    }

    public bool AddHealth(int h)
    {
        if (isAlive == false) return false;

        currHealth += h;
        ApplyMaterial(damagedMaterial, 0.5f);

        if (currHealth > maxHealth)
        {
            currHealth = maxHealth;
        }
        else if (currHealth <= 0)
        {
            healthBar.fillAmount = (float)currHealth / (float)maxHealth;
            Die();
            return false; //target dog
            //die
        }
        healthBar.fillAmount = (float)currHealth / (float)maxHealth;
        return true; //target vid liv
    }

    public void Die()
    {
        Debug.Log("död");
        return;
        isAlive = false;

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
            animationObj.GetComponent<Animation>().Play(deathAnimation.name);
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

    IEnumerator DieDelayed()
    {
        yield return new WaitForSeconds(delayedDeathTime);
        thisTransform.gameObject.SetActive(false);
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
        StartCoroutine(MarkMaterial(m, time));
    }

    IEnumerator MarkMaterial(Material m, float time)
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
        float speed = collision.relativeVelocity.magnitude;

        if (speed > speedDamageThreshhold)
        {
            AddHealth(Mathf.Min(0, -(int)(speed - speedDamageThreshhold)));
        }
    }

}
