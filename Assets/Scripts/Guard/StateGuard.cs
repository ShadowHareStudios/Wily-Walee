using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class StateGuard : MonoBehaviour
{
    public event Action<int> OnDetectedChanged;

    [SerializeField] public Transform pathHolder;
   
    public float grabDistance;

    private StateMachine _stateMachine;
    
    public Transform Target { get;set; }

    public Lootables Lootables { get; set; }

    private NavMeshAgent _navMeshAgent;

    //Patrol Variables
    public float patrolSpeed = 4;
    public float waitTime = 0.3f;
    public float turnSpeed = 90;
    public bool hasPath;
    public Vector3[] myPath;
    public bool onPatrol;

    //Search Variables
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange = 3f;
    public float walkDistance = 3f;
    public LayerMask whatIsGround;
    //Chase Variables



    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        var animator = GetComponent<Animator>();
        var enemyDetector = gameObject.GetComponent<EnemyDetector>();
        var myPath = CreatePath();
        

        _stateMachine = new StateMachine();

        var patrol = new PatrolArea(guard: this, _navMeshAgent, myPath, animator);
        var chase = new ChaseTarget(guard: this, _navMeshAgent, animator);
        var search = new SearchForTarget(guard: this, _navMeshAgent, animator);
        var moveToLocation = new MoveToTargetLocation(guard: this, _navMeshAgent, animator);
       
        

        
        At(moveToLocation, from: search, condition: HasNoTarget() );
        At(patrol, from: search, condition:() => enemyDetector.goPatrol);
        At(search, from: chase, condition: HasNoTarget() );
        At(search, from:moveToLocation, condition:  ReachedSearchPoint() );


        _stateMachine.AddAnyTransition(chase, predicate: () => enemyDetector.goChase);
        _stateMachine.AddAnyTransition(search, predicate: () => enemyDetector.goSearch);
        



        /*At(from: chase, to: search, condition: () => enemyDetector.canSeePlayer = false);*/

        _stateMachine.SetState(patrol);
        /*StartCoroutine(FollowPath(CreatePath()));*/

        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(from: from, to: to, condition);


        Func<bool>HasNoTarget() => () => Target == null;
        Func<bool> ReachedSearchPoint() => () =>  Vector3.Distance(transform.position, walkPoint) < 1f;
        /*Func<bool>ReachedTarget() => () => Target != null && Vector3.Distance(GetComponent<Collider>().ClosestPointOnBounds(transform.position), Target.transform.position) < grabDistance;*/
        /*Func<bool>StuckForOverASecond() => () => moveToLocation.TimeStuck > 1f;*/
        

        
        
    }
    void Update() 
    {
        _stateMachine.Tick();
        Debug.Log(Target);
    }

    public Vector3[] CreatePath()
    {

        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        myPath = waypoints;
        return waypoints;
        
    }
    public IEnumerator FollowPath(Vector3[] waypoints)
    {
        
        Debug.Log("follow");
        yield return new WaitForSeconds(2f);
        Debug.Log("path");

        foreach (Vector3 waypoint in waypoints)
        {
            /* Debug.Log("waypoint list: " + waypoint);*/
        }
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            _navMeshAgent.SetDestination(targetWaypoint);
            if (_navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                /* Debug.Log("Switching to end node: " + guardNavAgent.pathEndPosition);
                 Debug.Log("Switching to next position: " + guardNavAgent.nextPosition);*/
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return TurnToFace(targetWaypoint);
            }
            yield return null;
        }
    }
    public void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        /* Debug.Log("This is randomZ: " + randomZ + " This is randomX: " + randomX);*/

        walkPoint = new Vector3(this.transform.position.x + randomX, this.transform.position.y, this.transform.position.z + randomZ);
        

        if (Physics.Raycast(walkPoint, -this.transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }

        Vector3 dirToWalkPoint = ((this.transform.position + Vector3.down * 0.5f) - walkPoint).normalized;

        RaycastHit hit;
        if (Physics.Raycast((this.transform.position + Vector3.down * 0.5f), dirToWalkPoint, out hit))
        {
            walkPoint = hit.point;
            
        }
        
        Debug.DrawLine((this.transform.position + Vector3.down * 0.5f), walkPoint, Color.red, 5f);

        
    }
    public IEnumerator TurnToFace(Vector3 lookTarget)
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
        Gizmos.DrawLine(previousPosition, startPosition);


    }

}
