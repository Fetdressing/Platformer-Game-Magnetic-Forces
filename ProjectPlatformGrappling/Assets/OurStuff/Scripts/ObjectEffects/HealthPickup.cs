using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour {
    public int healthWorth = 100;

    public float respawnTime = 10;
	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.transform.GetComponent<Health>() != null)
        {
            col.transform.GetComponent<Health>().AddHealth(healthWorth);
            Die();
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
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
