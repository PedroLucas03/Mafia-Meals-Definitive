using UnityEngine;

public class ChaseState : States
{
    public AttackState attackState;
    public float attackRange = 5f;
    public Transform player;

    public override States RunCurrentState()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            return attackState; // Vai para AttackState
        }
        else
        {
            return this; // Continua perseguindo
        }
    }
}