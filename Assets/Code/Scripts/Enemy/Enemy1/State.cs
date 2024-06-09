using UnityEngine;

public class State
{
    public enum STATE
    {
        IDLE,
        SEARCHING,
        AIMING,
        ATTACKING
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
        // Calcola la direzione dalla canna da sparo al target
        Vector3 directionToTarget = enemy.NavMeshAgent.destination - enemy.Barrel.position;
        directionToTarget.y = 0; // Ignora la differenza di altezza

        // Controlla se il target è entro la distanza di vista
        if (directionToTarget.magnitude <= enemy.ViewDistance)
        {
            // Esegui un raycast per verificare la linea di vista
            if (Physics.Raycast(enemy.Barrel.position, directionToTarget.normalized, out RaycastHit hit, enemy.ViewDistance, enemy.TargetMask))
            {
                return true;
            }
        }
        return false;
    }

    protected bool IsTargetInSight()
    {
        Debug.DrawRay(enemy.FireParticles.transform.position, (enemy.FireParticles.transform.position + new Vector3(0, 0, enemy.ViewDistance)) * enemy.ViewDistance, Color.red, 0.1f);

        if (Physics.Raycast(enemy.FireParticles.transform.position, enemy.FireParticles.transform.position + new Vector3(0, 0, enemy.ViewDistance), out RaycastHit hit, enemy.ViewDistance, enemy.TargetMask))
        {
            if (hit.transform == enemy.Target)
            {
                return true;
            }
        }
        return false;
    }
}
