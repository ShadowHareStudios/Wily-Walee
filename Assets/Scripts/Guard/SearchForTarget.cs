using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchForTarget : IState
{
    //Search Variables
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float walkDistance;
    public LayerMask whatIsGround;
    public float alertedTimer = 10f;
    public float startAlertedTimer;
    


    private readonly StateGuard _guard;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;

    public SearchForTarget(StateGuard guard, NavMeshAgent navMeshAgent, Animator animator)
    {
        _guard = guard;
        _animator = animator;
        _navMeshAgent = navMeshAgent;
    }

    public void Tick()
    {

        
    }

    private Vector3 CreatePointAt(Vector3 walkPoint)
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        /* Debug.Log("This is randomZ: " + randomZ + " This is randomX: " + randomX);*/

        walkPoint = new Vector3(_guard.transform.position.x + randomX, _guard.transform.position.y, _guard.transform.position.z + randomZ);


        if (Physics.Raycast(walkPoint, -_guard.transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }

        Vector3 dirToWalkPoint = ((_guard.transform.position + Vector3.down * 0.5f) - walkPoint).normalized;

        RaycastHit hit;
        if (Physics.Raycast((_guard.transform.position + Vector3.down * 0.5f), dirToWalkPoint, out hit))
        {
            walkPoint = hit.point;
        }
        
        Debug.DrawLine((_guard.transform.position + Vector3.down * 0.5f), walkPoint, Color.red, 5f);
        
        return walkPoint;
    }

    public void OnEnter()
    {
        
        Debug.Log("SearchState");
        _navMeshAgent.enabled = true;
        CreatePointAt(walkPoint);

    }
    public void OnExit()
    {
        _guard.searchPoint = walkPoint;
        Debug.Log("Transition to Move");
        _navMeshAgent.enabled = false;
    }
}
