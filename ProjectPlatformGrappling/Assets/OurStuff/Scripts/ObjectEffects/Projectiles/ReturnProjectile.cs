using UnityEngine;
using System.Collections;

public class ReturnProjectile : ProjectileBase {
    [HideInInspector]
    public bool returning = false;
    public float powerFeedback = 0.5f;
    [HideInInspector]
    public Transform returnTarget;
    private float returnHomingForce = 500000;
    private float startDrag;
    private float returnHomingDrag = 3;

    public override void Init()
    {
        base.Init();
        if (initTimes > 0) return;
        startDrag = o_Rigidbody.drag;
    }

    public override void Fire(float lifeTime, Vector3 forceDir)
    {
        if (initTimes == 0) return;
        o_Rigidbody.velocity = new Vector3(0, 0, 0);
        returning = false;
        activated = false;
        Reset();
        StartCoroutine(Activate());
        StartCoroutine(FlyAway(lifeTime, forceDir));
    }

    public void Fire(float lifeTime, Vector3 forceDir, float powFeedback)
    {
        if (initTimes == 0) return;
        powerFeedback = powFeedback;
        o_Rigidbody.velocity = new Vector3(0, 0, 0);
        returning = false;
        activated = false;
        Reset();
        StartCoroutine(Activate());
        StartCoroutine(FlyAway(lifeTime, forceDir));
    }

    public void Fire(float lifeTime, Vector3 forceDir, float powFeedback, Transform returnTar)
    {
        if (initTimes == 0) return;
        powerFeedback = powFeedback;
        returnTarget = returnTar;
        o_Rigidbody.velocity = new Vector3(0, 0, 0);
        returning = false;
        activated = false;
        Reset();
        StartCoroutine(Activate());
        StartCoroutine(FlyAway(lifeTime, forceDir));
    }

    public IEnumerator FlyAway(float lifeTime, Vector3 forceDir)
    {
        o_Rigidbody.AddForce(forceDir, ForceMode.Impulse);
        yield return new WaitForSeconds(lifeTime);
        StartCoroutine(FlyBack(lifeTime, -forceDir));
       
    }

    public IEnumerator FlyBack(float lifeTime, Vector3 forceDir)
    {
        float startTime = Time.time;
        returning = true;
        o_Rigidbody.velocity = new Vector3(0, 0, 0);
        o_Rigidbody.drag = returnHomingDrag;
        if (returnTarget == null || returnHomingForce < 0.1f)
        {
            o_Rigidbody.AddForce(forceDir * 0.5f, ForceMode.Impulse);
        }
        while(((startTime + lifeTime * 2)) > Time.time)
        {
            if(returnTarget != null && returnHomingForce > 0.1f)
            {
                Vector3 toTarget = (returnTarget.position - transform.position).normalized;
                o_Rigidbody.AddForce(toTarget * returnHomingForce * Time.deltaTime, ForceMode.Force);
                yield return new WaitForSeconds(0.1f);
            }
        }
        Die();
    }

    public override void Reset()
    {
        base.Reset();
        StopAllCoroutines();
        o_Rigidbody.drag = startDrag;
        returning = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (activated == false) return;
        //PlayParticleSystem();

        if (returning)
        {
            for (int i = 0; i < healTags.Length; i++)
            {
                if (col.tag == healTags[i])
                {
                    col.GetComponent<PowerManager>().AddPower(powerFeedback);
                    Die();
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < damageTags.Length; i++)
            {
                if (col.tag == damageTags[i])
                {
                    PlayParticleSystem();
                    break;
                }
            }
        }
    }
}
