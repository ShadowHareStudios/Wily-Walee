using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    private NavMeshAgent guardNavAgent;
    public static event System.Action OnGuardHasCaughtPlayer;
    public LevelManager levelManager;
    public enum GuardStates
    {
        none,patrol,chase,search
    }

    GuardStates currentState;

    //Search Variables
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float walkDistance;
    public LayerMask whatIsGround;
    public float alertedTimer = 10f;
    public float startAlertedTimer;
   public Vector3 playerLastSeenPosition;

    //Patrol Variables
    public float patrolSpeed = 4;
    public float waitTime = 0.3f;
    public float turnSpeed = 90;

    //Chase Variables
    public float timeToSpotPlayer = 0.5f;
    public float grabDistance = 0.5f;
    public float chaseSpeed = 6;

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;

    float viewAngle;
    float playerVisibleTimer;

    public Transform pathHolder;
    Transform player;
    Color originalSpotlightColour;

    private NavMeshPath path;

    private void Awake()
    {
       
    }

    private void Start()
    {
        guardNavAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;

        if(currentState == GuardStates.none)
        {
            currentState = GuardStates.patrol;
        }

        StartCoroutine(FollowPath(CreatePath()));
    }

    private void Update()
    {
        Debug.Log(currentState);
        switch (currentState)
        {
            case GuardStates.patrol:

                guardNavAgent.speed = 4;

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
                    guardNavAgent.speed = 6f;
                    currentState = GuardStates.chase;

                   StopAllCoroutines();
                    //swap to Nav Mesh

                }
               

                break;

            case GuardStates.chase:

                guardNavAgent.speed = 6;
                if (CanSeePlayer())
                {
                    ChasePlayer();
                    StartCoroutine(TurnToFace(player.position));
                    playerLastSeenPosition = player.position;
                    startAlertedTimer = 0;

                    if (Vector3.Distance(GetComponent<Collider>().ClosestPointOnBounds(player.position), player.position) < grabDistance)
                    {
                        if (OnGuardHasCaughtPlayer != null)
                        {
                            OnGuardHasCaughtPlayer();
                            currentState = GuardStates.none;
                            levelManager.LevelHasBeenLost();
                        }
                    }
                }

                if (!CanSeePlayer())
                {


                   
                    StopAllCoroutines();


                    Debug.DrawLine(transform.position, playerLastSeenPosition, Color.red, 5f); 
                    
                    if (!guardNavAgent.hasPath)
                    {

                        currentState = GuardStates.search;
                        guardNavAgent.ResetPath();

                        Debug.Log("time to search");
                    }
                    else
                    {
                        guardNavAgent.destination = playerLastSeenPosition;
                    }

                }
                
                break;

            case GuardStates.search:

                spotlight.color = Color.Lerp(spotlight.color, Color.yellow, 3f);
                guardNavAgent.speed = 3;

                if (guardNavAgent.destination != playerLastSeenPosition)
                {
                    if (startAlertedTimer < alertedTimer)
                    {
                        startAlertedTimer += Time.deltaTime;

                    }
                }
                
                    if (!guardNavAgent.hasPath)
                    {

                        if (!walkPointSet)
                        {
                            SearchWalkPoint();
                        
                        }

                    }
                if (walkPointSet)
                {
                    if (startAlertedTimer < alertedTimer)
                    {
                        Debug.Log("still searching");

                    }

                    else
                    {
                        guardNavAgent.ResetPath();
                        /*alertedTimer = 10f;*/
                        guardNavAgent.speed = 4f;
                        currentState = GuardStates.patrol;
                        walkPointSet = false;
                        StopAllCoroutines();
                        StartCoroutine(FollowPath(CreatePath()));

                    }

                    guardNavAgent.SetDestination(walkPoint);
                   

                    Vector3 distanceToWalkPoint = transform.position - walkPoint;

                    if (distanceToWalkPoint.magnitude < 1f)
                    {
                        walkPointSet = false;
                    }
                }
                alertedTimer = Mathf.Clamp(alertedTimer, 0, 10);
                break;

            default: break;
                
        }
    }
   
        
    

    private void SearchWalkPoint()
    {

        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

       /* Debug.Log("This is randomZ: " + randomZ + " This is randomX: " + randomX);*/

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint,-transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }

        Vector3 dirToWalkPoint = ((transform.position + Vector3.down * 0.5f) - walkPoint).normalized;

        RaycastHit hit;
        if (Physics.Raycast((transform.position + Vector3.down * 0.5f), dirToWalkPoint, out hit))
        {
         walkPoint = hit.point;
        }
        Debug.DrawLine((transform.position + Vector3.down * 0.5f), walkPoint, Color.red, 5f);
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
        /*transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);*/
        guardNavAgent.destination = player.position;
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        foreach (Vector3 waypoint in waypoints)
        {
           /* Debug.Log("waypoint list: " + waypoint);*/
        }
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while(true)
        {
            guardNavAgent.SetDestination(targetWaypoint);
            if (guardNavAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
               /* Debug.Log("Switching to end node: " + guardNavAgent.pathEndPosition);
                Debug.Log("Switching to next position: " + guardNavAgent.nextPosition);*/
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
