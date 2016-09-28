using UnityEngine;
using System.Collections;

public class ObjectSpawner : BaseClass {
    public Transform spawnObject;
    public Transform spawnPoint;

    public GameObject particleEffect;
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        if(spawnPoint == null)
        {
            spawnPoint = transform;
        }
    }


    void OnTriggerEnter(Collider col)
    {
        GameObject parTemp = GameObject.Instantiate(particleEffect.gameObject);
        parTemp.transform.position = spawnObject.position;
        Destroy(parTemp, 3);

        spawnObject.position = spawnPoint.position;

        GameObject parTemp2 = GameObject.Instantiate(particleEffect.gameObject);
        parTemp.transform.position = spawnObject.position;
        Destroy(parTemp, 3);
    }
}
