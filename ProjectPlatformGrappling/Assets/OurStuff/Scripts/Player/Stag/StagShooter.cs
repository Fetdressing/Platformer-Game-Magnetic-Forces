using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StagShooter : BaseClass {
    private PowerManager powerManager;
    private Transform cameraObj;
    private Camera mainCamera;
    public Transform shooterObj;

    public LayerMask targetLM;
    private float shootForce = 200;
    private float cooldown_Time = 0.2f;
    private float cooldonwTimer = 0.0f;
    private float projectilePowerCost = 0.05f;

    public GameObject projectileType;
    private int poolSize = 100;
    private List<GameObject> projectilePool = new List<GameObject>();
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;
        mainCamera = cameraObj.GetComponentsInChildren<Transform>()[1].GetComponent<Camera>();
        powerManager = transform.GetComponent<PowerManager>();

        for(int i = 0; i < poolSize; i++)
        {
            GameObject temp = Instantiate(projectileType.gameObject);
            projectilePool.Add(temp);
            temp.SetActive(false);
        }
    }

    public override void Reset()
    {
        base.Reset();
        cooldonwTimer = 0.0f;
    }

    public override void Dealloc()
    {
        base.Dealloc();
        for (int i = 0; i < projectilePool.Count; i++)
        {
            projectilePool[i].GetComponent<ProjectileBase>().Dealloc();
            Destroy(projectilePool[i]);
        }
        projectilePool.Clear();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Fire();
        }
	}

    void Fire()
    {
        if (cooldonwTimer > Time.time) return;
        cooldonwTimer = Time.time + cooldown_Time;

        if (!powerManager.SufficentPower(projectilePowerCost)) return;
        powerManager.AddPower(-projectilePowerCost);

        GameObject currProj = null;
        for(int i = 0; i < projectilePool.Count; i++)
        {
            if(projectilePool[i].activeSelf == false)
            {
                currProj = projectilePool[i];
                break;
            }
        }
        if (currProj == null) return;
        Rigidbody currRig = currProj.GetComponent<Rigidbody>();
        ReturnProjectile currProjectile = currProj.GetComponent<ReturnProjectile>();


        RaycastHit raycastHit;
        if (Physics.Raycast(mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f)), out raycastHit, Mathf.Infinity, targetLM)) //kasta från mitten av skärmen!
        {
            if (Vector3.Dot(raycastHit.normal, mainCamera.transform.position - raycastHit.point) > 0) //normalen mot eller från sig?
            {
                if (Vector3.Distance(raycastHit.point, mainCamera.transform.position) > (Vector3.Distance(transform.position, mainCamera.transform.position))) //ligger raycasthit framför spelaren? man vill ju ej skjuta bakåt
                {
                    currProj.transform.position = shooterObj.position; //kommer från en random riktning kanske?
                    currProj.transform.LookAt(raycastHit.point);
                    currProjectile.Fire(2, currProjectile.transform.forward * shootForce, projectilePowerCost, transform);
                }
                else
                {
                    currProj.transform.position = shooterObj.position; //kommer från en random riktning kanske?
                    currProj.transform.forward = cameraObj.forward;
                    currProjectile.Fire(2, currProjectile.transform.forward * shootForce, projectilePowerCost, transform);
                }

            }
            else
            {
                currProj.transform.position = shooterObj.position; //kommer från en random riktning kanske?
                currProj.transform.forward = cameraObj.forward;
                currProjectile.Fire(2, currProjectile.transform.forward * shootForce, projectilePowerCost, transform);
            }
        }
        else
        {
            currProj.transform.position = shooterObj.position; //kommer från en random riktning kanske?
            currProj.transform.forward = cameraObj.forward;
            currProjectile.Fire(2, currProjectile.transform.forward * shootForce, projectilePowerCost, transform);
        }

    }
}
