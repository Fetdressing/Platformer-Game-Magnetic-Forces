using UnityEngine;
using System.Collections;

public class ProjectileMagnetSpawner : BaseClass
{
    private bool isActive = false;
    public GameObject spawnObject;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col)
    {
        if (isActive == false) return;

    }
}
