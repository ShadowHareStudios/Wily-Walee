using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

internal class MoveToTargetLocation : IState
{
    private readonly StateGuard _guard;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;

    private Vector3 _lastPosition = Vector3.zero;

    public float TimeStuck;

    public MoveToTargetLocation(StateGuard guard, NavMeshAgent navMeshAgent, Animator animator)
    {
        _guard = guard;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
        
    }

    public void Tick()
    {
        if(Vector3.Distance(a:_guard.transform.position,b: _lastPosition) <= 0f)
        {
            TimeStuck += Time.deltaTime;
        }
        _lastPosition = _guard.transform.position;
    }

    public void OnEnter()
    {
        _guard.StopAllCoroutines();
        _guard.StartCoroutine(_guard.TurnToFace(_guard.walkPoint));
        Debug.Log("MoveToPointState");
        TimeStuck = 0f;
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(_guard.walkPoint);
    }

    public void OnExit()
    {
        _guard.StopAllCoroutines();
        Debug.Log("Create new Search Point");
        _navMeshAgent.enabled = false;
    }
}
