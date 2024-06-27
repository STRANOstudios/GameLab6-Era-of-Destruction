using UnityEngine;

public class State
{
    public enum STATE
    {
        IDLE,
        SEARCHING,
        AIMING,
        ATTACKING,
        DEAD,
        FLEE
    }

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public STATE name;
    protected EVENT stage;

    protected Enemy1Controller enemy;

    protected State nextState;

    /// <summary>
    /// Initializes the state.
    /// </summary>
    /// <param name="gameObject"></param>
    public State(Enemy1Controller gameObject)
    {
        enemy = gameObject;

        stage = EVENT.ENTER;
    }

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public virtual void Enter() { stage = EVENT.UPDATE; }
    /// <summary>
    /// Called when the state is updated.
    /// </summary>
    public virtual void Update() { stage = EVENT.UPDATE; }
    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public virtual void Exit() { stage = EVENT.EXIT; }

    /// <summary>
    /// Process the state.
    /// </summary>
    /// <returns></returns>
    public State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    protected bool IsTargetIn360View()
    {
        Vector3 targetPosition = enemy.Target.position;

        Vector3 directionToTarget = targetPosition - enemy.Barrel.position;
        directionToTarget.y = 0;

        if (directionToTarget.magnitude <= enemy.ViewDistance)
        {
            if (Physics.Raycast(enemy.Barrel.position, directionToTarget.normalized, out RaycastHit hit, enemy.ViewDistance))
            {
                Debug.DrawRay(enemy.Barrel.position, directionToTarget.normalized * enemy.ViewDistance, Color.red);

                if (hit.transform == enemy.Target)
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected bool EscapeFromTargetIfTooClose()
    {
        Vector3 escapePosition;
        Vector3 targetPosition = enemy.Target.position;
        Vector3 directionToTarget = targetPosition - enemy.Barrel.position;

        directionToTarget.y = 0;

        if (directionToTarget.magnitude < enemy.SafeDistance)
        {
            Vector3 directionAwayFromTarget = -directionToTarget.normalized;

            Vector2 randomPoint = Random.insideUnitCircle * 5f;
            escapePosition = enemy.Barrel.position + directionAwayFromTarget * enemy.SafeDistance + new Vector3(randomPoint.x, 0, randomPoint.y);

            enemy.NavMeshAgent.SetDestination(escapePosition);

            Debug.DrawLine(enemy.Barrel.position, escapePosition, Color.blue, 1f);

            return true;
        }
        return false;
    }

    protected bool IsTargetInSight()
    {
        Vector3 direction = enemy.FireParticles.transform.forward;

        if (Physics.Raycast(enemy.FireParticles.transform.position, direction, out RaycastHit hit, enemy.ViewDistance))
        {
            Debug.DrawRay(enemy.FireParticles.transform.position, direction * enemy.ViewDistance, Color.green, 1f);

            if (hit.transform == enemy.Target)
            {
                return true;
            }
        }
        return false;
    }
}
