using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRotateTrackingMovement : BulletMovement
{

    public TurnTowardsScarlet m_TurnCommand;

    public override void HandleMovement(BulletBehaviour b)
    {
        m_TurnCommand.DoTurn();
    }
}
