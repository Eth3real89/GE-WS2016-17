using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelMoveCommand : BossMoveCommand {

    public static float s_SpeedMultiplier = 1f;

    protected override Vector3 CalculateMovemend(float horizontal, float vertical)
    {
        return base.CalculateMovemend(horizontal, vertical) * (s_SpeedMultiplier);
    }

}
