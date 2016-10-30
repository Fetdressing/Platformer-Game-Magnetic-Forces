using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : BaseClass {
    public int maxLives = 3;
    [HideInInspector]
    public int currLives;

    public Transform player;
    private StagMovement stagMovement;

    private List<Transform> spawnPoints = new List<Transform>();
    public Transform startSpawn;

    private bool isRespawning;

    PowerPickup[] powerPickups;
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        powerPickups = FindObjectsOfType(typeof(PowerPickup)) as PowerPickup[];

        GameObject[] spawnpointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");

        for(int i = 0; i < spawnpointObjects.Length; i++)
        {
            spawnPoints.Add(spawnpointObjects[i].transform);
        }

        stagMovement = player.GetComponent<StagMovement>();

        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        currLives = maxLives;
        isRespawning = false;

        StartSpawn();
    }

    public void StartSpawn()
    {
        if (startSpawn == null) return;
        if (isRespawning == true) return;
        
        isRespawning = true;
        stagMovement.isLocked = true;

        player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        StartCoroutine(SpawnPlayerAtLocation(startSpawn.position));
    }

    public void Respawn(Vector3 playerDeathPos)
    {
        if (isRespawning == true) return;
        isRespawning = true;
        stagMovement.isLocked = true;

        Vector3 closestSpawnPos = new Vector3(1000000000, 1000000000, 10000000000);

        for(int i = 0; i < spawnPoints.Count; i++)
        {
            if(Vector3.Distance(playerDeathPos, spawnPoints[i].position) < Vector3.Distance(playerDeathPos, closestSpawnPos))
            {
                closestSpawnPos = spawnPoints[i].position;
            }
        }

        player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        StartCoroutine(SpawnPlayerAtLocation(closestSpawnPos));
        
        for(int i = 0; i < powerPickups.Length; i++)
        {
            powerPickups[i].Reset();
        }
        //player.position = closestSpawnPos;
    }

    IEnumerator SpawnPlayerAtLocation( Vector3 pos)
    {
        while(Vector3.Distance(player.position, pos) > 2.0f)
        {
            player.position = Vector3.Lerp(player.position, pos, Time.deltaTime * 4);
            yield return new WaitForSeconds(0.01f);
        }
        player.GetComponent<PowerManager>().Reset();
        player.GetComponent<StagMovement>().Reset();
        player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        try
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MagneticPlacer>().Reset();
        }
        catch
        {
            Debug.Log("Ingen magneticplacer");
        }
        stagMovement.isLocked = false;
        isRespawning = false;
    }
}
