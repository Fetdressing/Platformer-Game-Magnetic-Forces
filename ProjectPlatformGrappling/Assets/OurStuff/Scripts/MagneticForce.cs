using UnityEngine;
using System.Collections;

public class MagneticForce : BaseClass
{
    public enum ForceType {Push, Pull};
    public ForceType forceType;

    public Color pushColor;
    public Color pullColor;
    public Material pushHoloMat;
    public Material pullHoloMat;

    private static Color pushColorS;
    private static Color pullColorS;

    private Transform thisTransform;
    private Transform holoRangeTransform;
    private ParticleSystem ps;
    private Light pLight;

    public float force = 20;
    public float range = 40;

	// Use this for initialization
	void Start () {
        Init();
    }

    public override void Init()
    {
        base.Init();
        thisTransform = this.transform;
        holoRangeTransform = thisTransform.GetComponentsInChildren<Transform>()[1];
        holoRangeTransform.localScale = new Vector3(range, range, range);

        ps = thisTransform.GetComponent<ParticleSystem>();
        pLight = thisTransform.GetComponent<Light>();

        pushColorS = pushColor;
        pullColorS = pullColor;

        switch (forceType)
        {
            case ForceType.Push:
                ps.startColor = pushColorS;
                pLight.color = pushColorS;
                holoRangeTransform.GetComponent<Renderer>().material = pushHoloMat;
                break;
            case ForceType.Pull:
                ps.startColor = pullColorS;
                pLight.color = pullColorS;
                holoRangeTransform.GetComponent<Renderer>().material = pullHoloMat;
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        ApplyForce();
	}


    void ApplyForce()
    {
        Collider[] colliders;
        colliders = Physics.OverlapSphere(thisTransform.position, range);
        foreach (Collider col in colliders)
        {
            Transform tr = col.transform;

            if(tr.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rigidbodyTemp = tr.GetComponent<Rigidbody>();
                Vector3 dir;
                float distanceMultiplier = Vector3.Distance(thisTransform.position, tr.position);
                switch (forceType)
                {
                    case ForceType.Push:
                        dir = (tr.transform.position - thisTransform.position).normalized;
                        rigidbodyTemp.AddForce(force * distanceMultiplier * dir * Time.deltaTime, ForceMode.Force);
                        break;
                    case ForceType.Pull:
                        dir = (thisTransform.position - tr.transform.position).normalized;
                        rigidbodyTemp.AddForce(force * distanceMultiplier * dir * Time.deltaTime, ForceMode.Force);
                        break;
                }
            }            
        }
    }
}
