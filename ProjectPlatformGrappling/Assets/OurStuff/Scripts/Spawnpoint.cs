using UnityEngine;
using System.Collections;

public class Spawnpoint : BaseClass {
    [HideInInspector]
    public bool isPassed = false;
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        isPassed = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            isPassed = true;
        }
    }
}
