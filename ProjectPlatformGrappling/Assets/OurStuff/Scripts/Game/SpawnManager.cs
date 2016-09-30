using UnityEngine;
using System.Collections;

public class SpawnManager : BaseClass {
    public int maxLives = 3;
    [HideInInspector]
    public int currLives;

    public Transform player;

    public Transform[] spawnPoints;
	// Use this for initialization
	void Start () {
	
	}

    public override void Init()
    {
        base.Init();
        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        currLives = maxLives;
    }

    public void Respawn(Vector3 playerDeathPos)
    {
        Vector3 closestSpawnPos = new Vector3(1000000000, 1000000000, 10000000000);

        for(int i = 0; i < spawnPoints.Length; i++)
        {
            if(Vector3.Distance(playerDeathPos, spawnPoints[i].position) < Vector3.Distance(playerDeathPos, closestSpawnPos))
            {
                closestSpawnPos = spawnPoints[i].position;
            }
        }

        player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        StartCoroutine(SpawnPlayerAtLocation(closestSpawnPos));
        //player.position = closestSpawnPos;
    }

    IEnumerator SpawnPlayerAtLocation( Vector3 pos)
    {
        while(Vector3.Distance(player.position, pos) > 2.0f)
        {
            player.position = Vector3.Lerp(player.position, pos, Time.deltaTime * 10);
            yield return new WaitForSeconds(0.01f);
        }
        player.GetComponent<Health>().Reset();
        player.GetComponent<Movement>().Reset();
        player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MagneticPlacer>().Reset();
    }
}
