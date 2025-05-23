using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : States
{
    public ChaseState chaseState;
    public bool canSeePlayer;

    public override States RunCurrentState()
    {
        if (canSeePlayer)
        {
            return chaseState;
        }
        else
        {
            return this;
        }
    }
}
