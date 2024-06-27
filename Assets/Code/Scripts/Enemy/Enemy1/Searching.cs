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

        if (IsTargetIn360View())
        {
            nextState = new Aiming(enemy);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
