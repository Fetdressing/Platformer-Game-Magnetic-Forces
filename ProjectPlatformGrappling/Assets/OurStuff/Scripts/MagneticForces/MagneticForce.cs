using UnityEngine;
using System.Collections;

public class MagneticForce : BaseClass
{
    public enum ForceType { Push, Pull };
    public ForceType forceType;

    public Color pushColor;
    public Color pullColor;
    public Material pushHoloMat;
    public Material pullHoloMat;

    [HideInInspector]
    public static Color pushColorS;
    [HideInInspector]
    public static Color pullColorS;

    [HideInInspector]
    public static Material pushHoloFMat;
    [HideInInspector]
    public static Material pullHoloFMat;

    [HideInInspector]
    public Transform thisTransform;
    [HideInInspector]
    public Transform holoRangeTransform;
    [HideInInspector]
    public ParticleSystem ps;
    [HideInInspector]
    public Light pLight;
    [HideInInspector]
    public TrailRenderer tRenderer;
    [HideInInspector]
    public Renderer renderer;

    public float force = 20;
    public float range = 40;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        thisTransform = this.transform;
        holoRangeTransform = thisTransform.GetComponentsInChildren<Transform>()[1];
        holoRangeTransform.localScale = new Vector3(range * thisTransform.localScale.x, range * thisTransform.localScale.x, range * thisTransform.localScale.x);

        pushHoloFMat = pushHoloMat;
        pullHoloFMat = pullHoloMat;

        ps = thisTransform.GetComponent<ParticleSystem>();
        pLight = thisTransform.GetComponent<Light>();
        tRenderer = thisTransform.GetComponent<TrailRenderer>();
        renderer = thisTransform.GetComponent<MeshRenderer>();

        pushColorS = pushColor;
        pullColorS = pullColor;

        switch (forceType)
        {
            case ForceType.Push:
                ps.startColor = pushColorS;
                pLight.color = pushColorS;
                holoRangeTransform.GetComponent<Renderer>().material = pushHoloFMat;
                //renderer.material = pushHoloFMat; //vänta lite med dessa
                tRenderer.material = pushHoloFMat;
                break;
            case ForceType.Pull:
                ps.startColor = pullColorS;
                pLight.color = pullColorS;
                holoRangeTransform.GetComponent<Renderer>().material = pullHoloFMat;
                //renderer.material = pullHoloFMat;
                tRenderer.material = pullHoloFMat;
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FixedUpdateLoop();
    }

    public virtual void FixedUpdateLoop()
    {
        ApplyForce();
    }

    public virtual void UpdateLoop()
    {

    }


    public virtual void ApplyForce()
    {
        Collider[] colliders;
        colliders = Physics.OverlapSphere(thisTransform.position, range);
        foreach (Collider col in colliders)
        {
            Transform tr = col.transform;

            if (tr.GetComponent<Rigidbody>() != null)
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
