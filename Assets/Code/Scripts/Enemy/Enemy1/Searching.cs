using UnityEngine;

internal class Searching : State
{
    public Searching(Enemy1Controller gameObject) : base(gameObject)
    {
        name = STATE.SEARCHING;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (enemy.Target) enemy.NavMeshAgent.destination = enemy.Target.position;

        if (IsTargetIn360View() && enemy.NavMeshAgent.remainingDistance <= enemy.ViewDistance)
        {
            nextState = new Attacking(enemy);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
