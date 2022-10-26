using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChaseTarget : IState
{
    private readonly StateGuard _guard;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    private readonly EnemyDetector _enemyDetector;
    public ChaseTarget(StateGuard guard, NavMeshAgent navMeshAgent, Animator animator)
    {
        _guard = guard;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
         _enemyDetector  = _guard.gameObject.GetComponent<EnemyDetector>();

    }

    public void Tick()
    {
        
        
        
    }
    public void OnEnter()
    {
        
        Debug.Log("ChaseState");
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(_guard.Target.transform.position);
    }

    public void OnExit()
    {
        Debug.Log("FinishChaseState");
        if (_guard.Target = null)
        {
            _navMeshAgent.SetDestination(_enemyDetector.playerLastSeenPosition);
        }
        _navMeshAgent.enabled = false;
    }
}
