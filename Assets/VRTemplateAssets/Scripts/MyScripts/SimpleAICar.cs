using UnityEngine;

public class SimpleAICar : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform waypointsParent;
    public float waypointReachDistance = 5f;

    [Header("Driving")]
    public float maxSpeed = 30f;
    public float acceleration = 1500f;
    public float steeringStrength = 5f;
    public float corneringSpeedFactor = 0.5f;

    [Header("Collision Avoidance")]
    public float lookAheadDistance = 8f;
    public LayerMask carLayer;

    [Header("Randomness")]
    public float laneOffset = 0f;

    [Header("Recovery")]
    public float flipCheckDelay = 2f; // seconds upside down before righting
    public float flipRecoveryHeight = 1.5f;  // how high above ground to drop

    private float flippedTimer = 0f;
    public float stuckCheckDelay = 4f;
    private float stuckTimer = 0f;

    private Transform[] waypoints;
    private int currentWaypoint = 0;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        waypoints = new Transform[waypointsParent.childCount];
        for (int i = 0; i < waypointsParent.childCount; i++)
            waypoints[i] = waypointsParent.GetChild(i);
    }

    void FixedUpdate()
    {
        CheckFlipped();
        CheckStuck();

        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        Vector3 offsetTarget = target.position + target.right * laneOffset;
        Vector3 toTarget = offsetTarget - transform.position;
        toTarget.y = 0; // to ignore height

        // rotate toward the waypoint
        float angleToTarget = Vector3.SignedAngle(transform.forward, toTarget, Vector3.up);
        float steerInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
        transform.Rotate(0, steerInput * steeringStrength, 0);

        // slow down for sharp turns
        float turnSharpness = Mathf.Abs(angleToTarget) / 90f;  // 0 = straight, 1 = 90 degrees
        float targetSpeed = Mathf.Lerp(maxSpeed, maxSpeed * corneringSpeedFactor, turnSharpness);

        Vector3 rayStart = transform.position + transform.forward * 1f;  // start 1 unit ahead of car center
        Ray forwardRay = new Ray(rayStart, transform.forward);
        if (Physics.Raycast(forwardRay, out RaycastHit hit, lookAheadDistance, carLayer))
        {
            float distanceFactor = hit.distance / lookAheadDistance;  // 0 = touching, 1 = at max range
            targetSpeed *= distanceFactor;  // closer = slower
        }

        if (rb.linearVelocity.magnitude < targetSpeed)
            rb.AddForce(transform.forward * acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);

        // advance to next waypoint when close enough or when passed
        Vector3 toTargetFlat = target.position - transform.position;
        toTargetFlat.y = 0;
        float distance = toTargetFlat.magnitude;

        bool closeEnough = distance < waypointReachDistance;
        bool passedIt = Vector3.Dot(transform.forward, toTargetFlat.normalized) < -0.3f
                        && distance < waypointReachDistance * 2.5f;

        if (closeEnough || passedIt)
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    // visual debugging (shows the current target in the editor)
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // current target in red
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, waypoints[currentWaypoint].position);
        Gizmos.DrawWireSphere(waypoints[currentWaypoint].position, waypointReachDistance);

        // all waypoints in yellow
        Gizmos.color = Color.yellow;
        foreach (var wp in waypoints)
            if (wp != null) Gizmos.DrawWireSphere(wp.position, 0.5f);
    }

    void CheckFlipped()
    {
        // 1 = car is upright, -1 = car is upside down
        bool isFlipped = Vector3.Dot(transform.up, Vector3.up) < 0.3f;

        if (isFlipped)
        {
            flippedTimer += Time.fixedDeltaTime;
            if (flippedTimer >= flipCheckDelay)
            {
                // keep the car's position and heading, but zero "roll"
                Vector3 forwardFlat = transform.forward;
                forwardFlat.y = 0;
                transform.rotation = Quaternion.LookRotation(forwardFlat.normalized, Vector3.up);
                transform.position += Vector3.up * flipRecoveryHeight;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                flippedTimer = 0f;
            }
        }
        else
        {
            flippedTimer = 0f;
        }
    }

    void CheckStuck()
    {
        // if car is barely moving (or not at all), it's stuck
        if (rb.linearVelocity.magnitude < 1f)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stuckCheckDelay)
            {
                // teleport to current waypoint facing the next one
                Transform target = waypoints[currentWaypoint];
                int nextIdx = (currentWaypoint + 1) % waypoints.Length;
                Vector3 lookDir = waypoints[nextIdx].position - target.position;
                lookDir.y = 0;

                transform.position = target.position + Vector3.up * flipRecoveryHeight;
                transform.rotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }
}