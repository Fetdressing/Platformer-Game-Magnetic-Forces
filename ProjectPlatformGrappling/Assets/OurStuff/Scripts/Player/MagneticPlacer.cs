using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagneticPlacer : BaseClass {

    public Transform[] projectilesPull;
    private List<Vector3> projectilesStartPositionsPull = new List<Vector3>();

    public Transform[] projectilesPush;
    private List<Vector3> projectilesStartPositionsPush = new List<Vector3>();
    // Use this for initialization
    void Start () {
        Init();
    }

    public override void Init()
    {
        base.Init();
        for (int i = 0; i < projectilesPull.Length; i++)
        {
            projectilesStartPositionsPull.Add(projectilesPull[i].position);
        }

        for (int i = 0; i < projectilesPush.Length; i++)
        {
            projectilesStartPositionsPush.Add(projectilesPush[i].position);
        }
    }

    void FireProjectile()
    {

    }

    // Update is called once per frame
    void Update () {
	
	}
}
