using UnityEngine;
using System.Collections;

public class PowerManager : BaseClass {
    private Camera activeCamera;
    public Renderer hornRenderer;
    private float[] uvStartOffsetHorns = { 0, 0};
    public Light[] lifeLights; //fadear med powern
    public float lightsMaxIntensity = 2;

    public Material emissiveStagMaterial; //det som fadeas ut när denne tappar powernn

    private float maxPower = 1;
    private float currPower;
    private float powerDecay = -0.02f;

    [HideInInspector]
    public bool isAlive;
    [HideInInspector]
    public Vector3 deathLocation;
    public GameObject deathParticleSystemObj;
    [HideInInspector]
    public GameObject deathParticleSystemSpawned;
    public float delayedDeathTime = 0;
    public GameObject animationObj;
    public AnimationClip deathAnimation;
    // Use this for initialization
    void Start () {
	
	}

    public override void Init()
    {
        base.Init();
        activeCamera = GameObject.FindGameObjectWithTag("Manager").GetComponent<CameraManager>().cameraPlayerFollow;
        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        transform.gameObject.SetActive(true);
        isAlive = true;
        AddPower(maxPower);
    }
    // Update is called once per frame
    void Update () {
        AddPower(powerDecay * Time.deltaTime);
	}

    public void AddPower(float p)
    {
        currPower += p;

        if(currPower > maxPower)
        {
            currPower = maxPower;
        }
        else if( currPower <= 0)
        {
            Die();
        }

        float offsetV = (currPower / maxPower);

        hornRenderer.material.SetTextureOffset("_MainTex", new Vector2(uvStartOffsetHorns[0], uvStartOffsetHorns[1] - offsetV));
        emissiveStagMaterial.SetColor("_EmissionColor", new Color(1,1,1) * offsetV);
        for (int i = 0; i < lifeLights.Length; i++)
        {
            lifeLights[i].intensity = (lightsMaxIntensity * currPower) - 0.3f;
        }
    }

    public bool SufficentPower(float p) //kolla ifall det finns tillräkligt med power för att dra
    {
        if ((currPower + p) <= 0) return false;
        return true;
    }

    public void Die()
    {
        Debug.Log("död");
        isAlive = false;

        deathLocation = transform.position;
        //if (aiBase.GetComponent<AgentBase>() != null)
        //{
        //    aiBase.GetComponent<AgentBase>().agent.enabled = false;
        //}
        if (deathParticleSystemSpawned != null)
        {
            deathParticleSystemSpawned.GetComponent<ParticleTimed>().StartParticleSystem();
        }
        if (deathAnimation != null)
        {
            animationObj.GetComponent<Animation>().Play(deathAnimation.name);
        }

        StartCoroutine(DieDelayed());
    }

    public IEnumerator DieDelayed()
    {
        yield return new WaitForSeconds(delayedDeathTime);
        transform.gameObject.SetActive(false);
        if (transform.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("Manager").GetComponent<SpawnManager>().Respawn(deathLocation);
        }
    }
}
