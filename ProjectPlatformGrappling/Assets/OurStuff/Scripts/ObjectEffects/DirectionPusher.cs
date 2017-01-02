using UnityEngine;
using System.Collections;

public class DirectionPusher : MonoBehaviour {
    public Transform pushTarget; //det hållet som ska pushas mot

    //för squish effekten just
    public Transform pushEffectObj;
    Vector3 startScaleObj; //så att man kan flytta tillbaks den till ursprungs skalan
    IEnumerator bounceEffect;

    static protected StagMovement stagMov;

    public float pushForce = 400;
	// Use this for initialization
	void Start () {
	    if(stagMov == null)
        {
            stagMov = GameObject.FindGameObjectWithTag("Player").GetComponent<StagMovement>();
        }

        if(pushEffectObj != null)
        {
            startScaleObj = pushEffectObj.localScale;
        }
	}
	

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            if(pushEffectObj != null)
            {
                if(bounceEffect != null)
                {
                    StopCoroutine(bounceEffect);
                }
                bounceEffect = BounceEffect();
                StartCoroutine(bounceEffect);
            }

            Vector3 dir = (pushTarget.position - col.transform.position).normalized;
            stagMov.ApplyExternalForce(dir * pushForce, true);
        }
    }

    IEnumerator BounceEffect()
    {
        pushEffectObj.localScale = startScaleObj;
        Vector3 wantedScale = pushEffectObj.localScale * 0.5f;

        while(Mathf.Abs(pushEffectObj.localScale.magnitude - wantedScale.magnitude) > 0.001f)
        {
            pushEffectObj.localScale = Vector3.Lerp(pushEffectObj.localScale, wantedScale, Time.deltaTime * 5);
            yield return new WaitForEndOfFrame();
        }

        while (Mathf.Abs(pushEffectObj.localScale.magnitude - startScaleObj.magnitude) > 0.001f)
        {
            pushEffectObj.localScale = Vector3.Lerp(wantedScale, pushEffectObj.localScale, Time.deltaTime * 5);
            yield return new WaitForEndOfFrame();
        }

        bounceEffect = null;
    }
}
