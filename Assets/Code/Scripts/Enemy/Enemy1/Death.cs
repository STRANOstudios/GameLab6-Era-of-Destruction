using UnityEngine;

internal class Death : State
{
    public Death(Enemy1Controller gameObject) : base(gameObject)
    {
        name = STATE.DEAD;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.NavMeshAgent.isStopped = true;
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        base.Exit();
        enemy.NavMeshAgent.isStopped = false;
    }
}