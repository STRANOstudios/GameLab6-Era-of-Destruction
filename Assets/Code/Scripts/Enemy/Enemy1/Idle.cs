public class Idle : State
{
    public Idle(Enemy1Controller gameObject) : base(gameObject)
    {
        name = STATE.IDLE;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        nextState = new Searching(enemy);
        stage = EVENT.EXIT;
    }

    public override void Exit()
    {
        base.Exit();
    }
}