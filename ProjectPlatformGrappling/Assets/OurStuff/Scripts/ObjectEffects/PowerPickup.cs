using UnityEngine;
using System.Collections;

public class PowerPickup : BaseClass {
    public GameObject pickUpObj;

    private Rigidbody thisRigidbody;
    private Transform player;
    private Vector3 startPos;
    public float playerChaseSpeed = 0;
    public float playerChaseDistance = 140;

    public float powerWorth = 0.1f;

    public float respawnTime = 10;

    public string[] acceptedTags;
    public ParticleTimed particlePicked;
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        thisRigidbody = this.transform.GetComponent<Rigidbody>();
        startPos = this.transform.position;
    }

    public override void Reset()
    {
        base.Reset();
        Spawn();
    }

    //void FixedUpdate()
    //{
    //    if (playerChaseSpeed < 0.1f || thisRigidbody.isKinematic) return;

    //    if (Vector3.Distance(player.position, this.transform.position) < playerChaseDistance)
    //    {
    //        Vector3 dir = (player.position - this.transform.position).normalized;
    //        thisRigidbody.AddForce(dir * playerChaseSpeed * Time.deltaTime, ForceMode.Force);
    //        //pickUpObj.transform.position = Vector3.Slerp(pickUpObj.transform.position, player.position, Time.deltaTime * playerChaseSpeed);
    //    }
    //    else if (Vector3.Distance(startPos, this.transform.position) > (playerChaseDistance))
    //    {
    //        Vector3 dir = (startPos - this.transform.position).normalized;
    //        thisRigidbody.AddForce(dir * playerChaseSpeed * Time.deltaTime, ForceMode.Force);
    //    }
    //    else
    //    {
    //        thisRigidbody.velocity = thisRigidbody.velocity * 0.99f;
    //    }
    //}

    void Update()
    {
        if (player.gameObject.activeSelf == false) return;
        if (playerChaseSpeed < 0.1f) return;

        float distanceToPlayer = Vector3.Distance(player.position, this.transform.position);
        if (distanceToPlayer < playerChaseDistance)
        {
            this.transform.position = Vector3.Slerp(this.transform.position, player.position, Time.deltaTime * playerChaseSpeed / distanceToPlayer);
        }
        else if (Vector3.Distance(startPos, this.transform.position) > (playerChaseDistance))
        {
            this.transform.position = Vector3.Slerp(this.transform.position, startPos, Time.deltaTime * playerChaseSpeed);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (pickUpObj.activeSelf == false) return;
        bool acceptedTag = false;
        for(int i = 0; i < acceptedTags.Length; i++)
        {
            if(col.transform.tag == acceptedTags[i])
            {
                acceptedTag = true;
                break;
            }
        }

        if (acceptedTag == false) return;

        if(col.transform.GetComponent<PowerManager>() != null)
        {
            col.transform.GetComponent<PowerManager>().AddPower(powerWorth);
            particlePicked.StartParticleSystem();
            Die();
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        Spawn();
    }

    void Spawn()
    {
        this.transform.position = startPos;
        thisRigidbody.velocity = new Vector3(0, 0, 0);
        pickUpObj.gameObject.SetActive(true);
    }

    void Die()
    {
        if(respawnTime > 0.0f)
        {
            pickUpObj.gameObject.SetActive(false);
            StartCoroutine(Respawn());
        }
        else
        {
            Destroy(pickUpObj.gameObject);
        }
    }
}
