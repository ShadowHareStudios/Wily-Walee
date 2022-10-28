
using UnityEngine;
using UnityEngine.AI;

public class SearchForTarget : IState
{
    
    private readonly StateGuard _guard;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    private readonly EnemyDetector _enemyDetector;

    public SearchForTarget(StateGuard guard, NavMeshAgent navMeshAgent, Animator animator)
    {
        _guard = guard;
        _animator = animator;
        _navMeshAgent = navMeshAgent;
        _enemyDetector = _guard.gameObject.GetComponent<EnemyDetector>();
    }

    public void Tick()
    {
     

    }

    public void OnEnter()
    {
        
        _guard.StopAllCoroutines();
        Debug.Log("SearchState");
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(_enemyDetector.playerLastSeenPosition);
        
        _guard.SearchWalkPoint();

    }
    public void OnExit()
    {
        _guard.StopAllCoroutines();
        
        Debug.Log("Transition to Move" + _guard.walkPoint);
       
        _navMeshAgent.enabled = false;
    }
}
