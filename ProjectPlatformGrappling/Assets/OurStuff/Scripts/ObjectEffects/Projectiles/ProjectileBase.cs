using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileBase : BaseClass {
    public LayerMask targetsLM;
    [HideInInspector]
    public Rigidbody o_Rigidbody;
    public string[] healTags = { "Player" };
    public string[] damageTags = { "Enemy" };

    [HideInInspector]
    public bool activated; //gör inte skada dirr, nån sekunds då den inte gör något i luften
    [HideInInspector]
    public float activationTime = 0.3f;

    public GameObject particleExplosionObj;
    [HideInInspector]
    public List<GameObject> particleObjectPool = new List<GameObject>();
    public int particleObjectPoolSize = 3;

    void Awake()
    {
        Init();
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        if (initTimes > 0) return;
        initTimes++;

        base.Init();
        o_Rigidbody = transform.GetComponent<Rigidbody>();

        for(int i = 0; i < particleObjectPoolSize; i++)
        {
            GameObject tempPar = Instantiate(particleExplosionObj.gameObject);
            particleObjectPool.Add(tempPar);
            tempPar.SetActive(false);
        }
    }

    public override void Dealloc()
    {
        base.Dealloc();
        
        for(int i = 0; i < particleObjectPool.Count; i++)
        {
            Destroy(particleObjectPool[i]);
        }
        particleObjectPool.Clear();
    }

    public virtual void Fire( float lifeTime, Vector3 forceDir)
    {
        if (initTimes == 0) return;
        o_Rigidbody.velocity = new Vector3(0, 0, 0);
        activated = false;
        Reset();
        StartCoroutine(Activate());
        StartCoroutine(LifeTime(lifeTime));
        o_Rigidbody.AddForce(forceDir, ForceMode.Impulse);
    }

    public virtual IEnumerator LifeTime(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Die();
    }

    public virtual IEnumerator Activate()
    {
        yield return new WaitForSeconds(activationTime);
        activated = true;
    }

    public override void Reset()
    {
        base.Reset();
        o_Rigidbody.velocity = new Vector3(0, 0, 0);
        StopAllCoroutines();
        transform.gameObject.SetActive(true);
    }

    public virtual void Die()
    {
        PlayParticleSystem();
        transform.gameObject.SetActive(false);
    }

    public virtual void PlayParticleSystem()
    {
        for(int i = 0; i < particleObjectPool.Count; i++)
        {
            if(particleObjectPool[i].activeSelf == false)
            {
                particleObjectPool[i].transform.position = transform.position;
                ParticleTimed pT = particleObjectPool[i].GetComponent<ParticleTimed>();
                pT.StartParticleSystem();
                break;
            }
        }
    }
}
