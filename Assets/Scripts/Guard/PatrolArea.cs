using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolArea : IState
{
    
    //Patrol Variables
    public float patrolSpeed = 4;
    public float waitTime = 0.3f;
    public float turnSpeed = 90;
    public bool hasPath;
    public bool onPatrol;


    private readonly StateGuard _guard;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    private readonly Vector3[] _myPath;
    public PatrolArea(StateGuard guard, NavMeshAgent navMeshAgent, Vector3[] myPath, Animator animator)
    {
        _guard = guard;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
        _myPath = myPath;

    }
    public void OnEnter()
    {
        Debug.Log("PatrolState");
        _navMeshAgent.enabled = true;
        _navMeshAgent.speed = patrolSpeed;
        _guard.FollowPath(_myPath);
        
    }
        
    public void Tick()
    {

        

    }

    public void OnExit()
    {
        Debug.Log("PatrolStateExit");
        _navMeshAgent.enabled = false;
        _guard.StopCoroutine(_guard.FollowPath(_guard.CreatePath()));
    }

    
    

    private void OnDrawGizmos()
    {
        Vector3 startPosition = _guard.pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in _guard.pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        
    }
}
