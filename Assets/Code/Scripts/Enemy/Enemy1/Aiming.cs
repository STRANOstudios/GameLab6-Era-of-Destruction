using UnityEngine;

internal class Attacking : State
{
    public Attacking(Enemy1Controller gameObject) : base(gameObject)
    {
        name = STATE.ATTACKING;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.NavMeshAgent.isStopped = true;
    }

    public override void Update()
    {
        if (enemy.NavMeshAgent.remainingDistance > enemy.ViewDistance)
        {
            nextState = new Searching(enemy);
            stage = EVENT.EXIT;
            return;
        }

        if (enemy.Target) enemy.NavMeshAgent.destination = enemy.Target.position;

        if (enemy.Target)
        {
            Vector3 predictedPosition = (enemy.NavMeshAgent.destination + Vector3.up) + (enemy.NavMeshAgent.velocity * /*predictionTime*/ 1f);

            Vector3 directionToTarget = predictedPosition - enemy.Turret.position;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));

            enemy.Turret.rotation = Quaternion.Slerp(enemy.Turret.rotation, targetRotation, Time.deltaTime * 5f);

            float elevationAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.magnitude) * Mathf.Rad2Deg;

            enemy.Barrel.localRotation = Quaternion.Euler(Mathf.Clamp(-elevationAngle, -20f, 0f), 0f, 0f);

            if (Time.time - enemy.TimeSinceLastFire > enemy.FireRatio)
            {
                if (!IsTargetInSight()) return;

                enemy.TimeSinceLastFire = Time.time;
                enemy.FireParticles.Emit(1);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.NavMeshAgent.isStopped = false;
    }
}
