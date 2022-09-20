using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardHasCaughtPlayer;
    
    public enum GuardStates
    {
        none,patrol,chase, search
    }

    GuardStates currentState;

    public float patrolSpeed = 4;
    public float chaseSpeed = 6;
    public float waitTime = 0.3f;
    public float turnSpeed = 90;
    public float timeToSpotPlayer = 1.5f;
    public float grabDistance = 0.5f;

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;

    float viewAngle;
    float playerVisibleTimer;

    public Transform pathHolder;
    Transform player;
    Color originalSpotlightColour;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;

        if(currentState == GuardStates.none)
        {
            currentState = GuardStates.patrol;
        }    

        StartCoroutine (FollowPath(CreatePath()));
    }

    private void Update()
    {
        switch (currentState)
        {
            case GuardStates.patrol:
                if (CanSeePlayer())
                {
                    playerVisibleTimer += Time.deltaTime;

                }
                else
                {
                    playerVisibleTimer -= Time.deltaTime;

                }
                playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
                spotlight.color = Color.Lerp(originalSpotlightColour, Color.red, playerVisibleTimer / timeToSpotPlayer);

                if (playerVisibleTimer >= timeToSpotPlayer)
                {
                    currentState = GuardStates.chase;
                    StopAllCoroutines();
                //swap to Nav Mesh
                
                }

                break;

            case GuardStates.chase:
                
                ChasePlayer();
                StartCoroutine(TurnToFace(player.position));

                if (Vector3.Distance(GetComponent<Collider>().ClosestPointOnBounds(player.position), player.position) < grabDistance)
                {
                    if(OnGuardHasCaughtPlayer != null)
                    {
                        OnGuardHasCaughtPlayer();
                        currentState = GuardStates.none;
                    }
                }
           

                    if (!CanSeePlayer())
                {
                    currentState= GuardStates.search;
                    StopAllCoroutines();
                    
                }

                break;
                default:break;
        }
   
        
    }

   Vector3[] CreatePath()
    {

        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        return waypoints;

    }


    bool CanSeePlayer()
    {
        if(Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if(!Physics.Linecast (transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }    
        return false;
    }

    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while(true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, patrolSpeed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine (TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine (previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
