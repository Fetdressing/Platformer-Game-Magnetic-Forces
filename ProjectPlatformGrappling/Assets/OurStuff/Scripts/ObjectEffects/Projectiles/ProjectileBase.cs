using UnityEngine;
using System.Collections;

public class ProjectileBase : BaseClass {
    public LayerMask targetsLM;
    [HideInInspector]
    public Rigidbody o_Rigidbody;

    [HideInInspector]
    public bool activated; //gör inte skada dirr, nån sekunds då den inte gör något i luften
    [HideInInspector]
    public float activationTime = 0.5f;

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
        base.Init();
        o_Rigidbody = transform.GetComponent<Rigidbody>();
        initTimes++;
    }

    public void Fire( float lifeTime, Vector3 forceDir)
    {
        if (initTimes == 0) return;
        activated = false;
        Reset();
        StartCoroutine(Activate());
        StartCoroutine(LifeTime(lifeTime));
        o_Rigidbody.AddForce(forceDir, ForceMode.Impulse);
    }

    IEnumerator LifeTime(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        transform.gameObject.SetActive(false);
    }

    IEnumerator Activate()
    {
        yield return new WaitForSeconds(activationTime);
        activated = true;
    }

    public override void Reset()
    {
        base.Reset();
        StopAllCoroutines();
        transform.gameObject.SetActive(true);
    }
}
