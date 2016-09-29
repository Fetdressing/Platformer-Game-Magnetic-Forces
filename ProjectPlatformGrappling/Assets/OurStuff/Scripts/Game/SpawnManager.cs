using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {
    public Transform player;

    public Transform[] spawnPoints;
	// Use this for initialization
	void Start () {
	
	}

    public void Respawn(Vector3 playerDeathPos)
    {
        Vector3 closestSpawnPos = transform.position;
        float closestSpawnDistance = 1000000000000;

        for(int i = 0; i < spawnPoints.Length; i++)
        {
            if(Vector3.Distance(playerDeathPos, spawnPoints[i].position) < closestSpawnDistance)
            {
                closestSpawnPos = spawnPoints[i].position;
            }
        }

        player.position = closestSpawnPos;
        player.GetComponent<Health>().Reset();
        player.GetComponent<Movement>().Reset();
        player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MagneticPlacer>().Reset();
    }
}
