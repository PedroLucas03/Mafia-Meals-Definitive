using UnityEngine;

public class AttackState : States
{
    public ChaseState chaseState;
    public float attackRange = 5f;
    public Transform player;
    public float aimingThreshold = 10f; // Ângulo máximo para considerar boa mira

    public override States RunCurrentState()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && IsProperlyAimed())
        {
            return this;
        }
        else
        {
            return chaseState;
        }
    }

    bool IsProperlyAimed()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle <= aimingThreshold;
    }
}