using UnityEngine;
using System.Collections;
using UnityEditor;

public class ThirdPersonCamera : BaseClass {
    public Transform target;
    private StagMovement stagMovement;
    public Camera cameraC;


    //för att kolla ifall spelaren står still eller ej
    private Vector3 targetLastFramePos;
    private float targetSpeed;

    public float distance = 10;
    public float minDistance = 8;
    public float height = 4;
    public float cameraSpeed = 4;

    private Vector3 wantedPosition;
    private Vector3 velocityCamSmooth = Vector3.zero;
    private float camSmoothTime = 0.1f;
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        stagMovement = target.GetComponent<StagMovement>();
        targetLastFramePos = Vector3.zero;
    }

    // Update is called once per frame
    void LateUpdate () {
        Vector3 toTarget = (target.position - transform.position).normalized;
        Vector3 toCamera = (transform.position - target.position).normalized;
        Vector3 toCameraNoY = new Vector3(toCamera.x, 0, toCamera.z).normalized;
        Vector3 toTargetNoY = new Vector3(toTarget.x, 0, toTarget.z).normalized;
        //wantedPosition = target.position + (Vector3.up * height) + (-target.forward * distance);
        wantedPosition = target.position + (Vector3.up * height) + (-toTargetNoY * distance);

        targetSpeed = Mathf.Abs(Vector3.Distance(target.position, targetLastFramePos)) * Time.deltaTime;
        float currDistance = Vector3.Distance(target.position, transform.position);
        float currYDistance = Mathf.Abs(target.position.y - transform.position.y);

        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * cameraSpeed);

        //if(currDistance < minDistance)
        //{
        //    wantedPosition = target.position + (Vector3.up * height) + toCameraNoY * minDistance;

        //    transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * cameraSpeed);

        //}
        //else if (currDistance > distance * 1.5f || currYDistance > distance * 0.75f)
        //{
        //    transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * cameraSpeed);
        //}

        //if (currDistance < (minDistance * 0.8f)) //jättenära
        //{
        //    transform.Translate(toCameraNoY * Time.deltaTime * 12); //blir snett?? nån offset så börjar den väl
        //}

        //look at
        //Quaternion lookRotation = Quaternion.LookRotation(toTarget);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, deltaTime * 20);
        transform.LookAt(target.position);

        targetLastFramePos = target.position;
	}

    private void SmoothPosition(Vector3 startPos, Vector3 wantedPos)
    {
        transform.position = Vector3.SmoothDamp(startPos, wantedPos, ref velocityCamSmooth, camSmoothTime);
    }

    //private void CompensateForWalls(Vector3 fromObject, ref Vector3 toCamera)
    //{
    //    // Compensate for walls between camera
    //    RaycastHit wallHit = new RaycastHit();
    //    if (Physics.Linecast(fromObject, toCamera, out wallHit))
    //    {
    //        Debug.DrawRay(wallHit.point, wallHit.normal, Color.red);
    //        toCamera = wallHit.point;
    //    }

    //    // Compensate for geometry intersecting with near clip plane
    //    Vector3 camPosCache = GetComponent<Camera>().transform.position;
    //    GetComponent<Camera>().transform.position = toCamera;
    //    viewFrustum = DebugDraw.CalculateViewFrustum(cameraC, ref nearClipDimensions);

    //    for (int i = 0; i < (viewFrustum.Length / 2); i++)
    //    {
    //        RaycastHit cWHit = new RaycastHit();
    //        RaycastHit cCWHit = new RaycastHit();

    //        // Cast lines in both directions around near clipping plane bounds
    //        while (Physics.Linecast(viewFrustum[i], viewFrustum[(i + 1) % (viewFrustum.Length / 2)], out cWHit) ||
    //               Physics.Linecast(viewFrustum[(i + 1) % (viewFrustum.Length / 2)], viewFrustum[i], out cCWHit))
    //        {
    //            Vector3 normal = wallHit.normal;
    //            if (wallHit.normal == Vector3.zero)
    //            {
    //                // If there's no available wallHit, use normal of geometry intersected by LineCasts instead
    //                if (cWHit.normal == Vector3.zero)
    //                {
    //                    if (cCWHit.normal == Vector3.zero)
    //                    {
    //                        Debug.LogError("No available geometry normal from near clip plane LineCasts. Something must be amuck.", this);
    //                    }
    //                    else
    //                    {
    //                        normal = cCWHit.normal;
    //                    }
    //                }
    //                else
    //                {
    //                    normal = cWHit.normal;
    //                }
    //            }

    //            toCamera += (compensationOffset * normal);
    //            GetComponent<Camera>().transform.position += toCamera;

    //            // Recalculate positions of near clip plane
    //            viewFrustum = DebugDraw.CalculateViewFrustum(GetComponent<Camera>(), ref nearClipDimensions);
    //        }
    //    }

    //    GetComponent<Camera>().transform.position = camPosCache;
    //    viewFrustum = DebugDraw.CalculateViewFrustum(GetComponent<Camera>(), ref nearClipDimensions);
    //}
}
