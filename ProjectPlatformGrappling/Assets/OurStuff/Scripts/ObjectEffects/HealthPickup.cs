using UnityEngine;
using System.Collections;

public class HealthPickup : BaseClass {
    public int healthWorth = 100;

    public float respawnTime = 10;

    public string[] acceptedTags;
    public ParticleTimed particlePicked;
	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter(Collider col)
    {
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

        if(col.transform.GetComponent<Health>() != null)
        {
            col.transform.GetComponent<Health>().AddHealth(healthWorth);
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
        this.gameObject.SetActive(true);
    }

    void Die()
    {
        if(respawnTime > 0.0f)
        {
            this.gameObject.SetActive(false);
            StartCoroutine(Respawn());
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
