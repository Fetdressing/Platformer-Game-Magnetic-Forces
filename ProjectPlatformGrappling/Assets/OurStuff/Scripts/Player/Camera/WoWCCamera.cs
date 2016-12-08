using UnityEngine;
using System.Collections;

public class WoWCCamera : MonoBehaviour
{
    protected ControlManager controlManager;
    public LayerMask collisionLayerMask; //mot vilka lager ska kameran kolla kollision?

    public Transform target;
    private float currSpeed = 0.0f;
    private Vector3 currFrameTargetPos = new Vector3();
    private Vector3 lastFrameTargetPos = new Vector3();

    public float targetHeight = 12.0f;
    public float distance = 5.0f;
    public float extraDistance = 0.0f;

    public float maxDistance = 20;
    public float minDistance = 2.5f;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;
    [HideInInspector]
    public float speedMultiplier = 1;

    public float yMinLimit = -20;
    public float yMaxLimit = 80;

    public float zoomRate = 20;

    public float rotationDampening = 3.0f;

    public float theta2 = 0.5f;

    private float x = 0.0f;
    private float y = 0.0f;

    private Vector3 fwd = new Vector3();
    private Vector3 rightVector = new Vector3();
    private Vector3 upVector = new Vector3();
    private Vector3 movingVector = new Vector3();
    private Vector3 collisionVector = new Vector3();
    private bool isColliding = false;

    private Vector3 a1 = new Vector3();
    private Vector3 b1 = new Vector3();
    private Vector3 c1 = new Vector3();
    private Vector3 d1 = new Vector3();
    private Vector3 e1 = new Vector3();
    private Vector3 f1 = new Vector3();
    private Vector3 h1 = new Vector3();
    private Vector3 i1 = new Vector3();

    //@script AddComponentMenu("Camera-Control/WoW Camera")

    void Start()
    {
        controlManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ControlManager>();
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

    }

    void LateUpdate()
    {
        if (Time.timeScale == 0) return;

        if (!target)
            return;

        currFrameTargetPos = target.position;
        if (lastFrameTargetPos != Vector3.zero)
        {
            var valueSpeed = Mathf.Abs(Mathf.Abs(lastFrameTargetPos.magnitude) - Mathf.Abs(currFrameTargetPos.magnitude));
            extraDistance = Mathf.Lerp(extraDistance, valueSpeed, Time.deltaTime * 10f); ;
        }
        else
        {
            extraDistance = Mathf.Lerp(extraDistance, 0, Time.deltaTime * 10f);
        }


        x += controlManager.horAxisView * xSpeed * 0.02f * speedMultiplier;
        y -= controlManager.verAxisView * ySpeed * 0.02f * speedMultiplier;


        distance -= (Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * zoomRate * Mathf.Abs(distance);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * (distance + extraDistance) + new Vector3(0, -targetHeight, 0));

        //// Check to see if we have a collision
        //collisionVector = AdjustLineOfSight(transform.position, position);

        //// Check Line Of Sight
        //if (collisionVector != Vector3.zero)
        //{
        //    Debug.Log("Check Line Of Sight");
        //    a1 = transform.position;
        //    b1 = position;
        //    c1 = AdjustLineOfSight(transform.position, position);
        //    d1 = c1 - a1;
        //    e1 = d1.normalized * -1;
        //    f1 = d1 + e1 * 1;
        //    g1 = f1 + a1;
        //    position = g1;

        //    // check distance player to camera
        //    h1 = position - a1;
        //    if (h1.magnitude < 10)
        //    {
        //        position = a1 - fwd * 4;
        //        //position.y = targetPlayer.y;
        //        theta2 = theta2 + .25;
        //    }

        //    // set new camera distance
        //    h1 = position - a1;
        //    distance = h1.magnitude;
        //}

        //// check collision
        //if (Physics.CheckSphere (position, .5) )
        //{
        //    a1 = transform.position;

        //    newPosition = a1 - fwd * 4;
        //    //newPosition.y = targetPlayer.y;
        //    theta2 = theta2 + .25;

        //    // set new camera distance
        //    h1 = position - a1;
        //    distance = h1.magnitude;
        //}  

        //position = Vector3.Slerp(transform.position, position, Time.deltaTime * 100);

        transform.rotation = rotation;
        transform.position = position;

        //Vector3 fow = transform.forward;
        //CompensateForWalls(transform.position, ref fow);
        //transform.forward = fow;

        lastFrameTargetPos = target.position;
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    private void CompensateForWalls(Vector3 fromObject, ref Vector3 toTar)
    {
        // Compensate for walls between camera
        RaycastHit wallHit = new RaycastHit();
        if (Physics.Linecast(fromObject, toTar, out wallHit, collisionLayerMask))
        {
            toTar = wallHit.point;
        }

    }


    //Vector3 AdjustLineOfSight(Vector3 vecA, Vector3 vecB)
    //{
    //    RaycastHit hit;

    //    if (Physics.Linecast(vecA, vecB, hit))
    //    {
    //        Debug.Log("I hit something");
    //        return hit.point;
    //    }

    //    return Vector3.zero;
    //}
}