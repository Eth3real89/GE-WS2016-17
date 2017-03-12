using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelOppositeMovementCombo : AngelOnlyMovementCombo
{
    public Transform m_Scarlet;
    public float m_MinDistanceForSuccess;

    public override bool AlreadyInGoodPosition()
    {
        return ManeuverSuccesful();
    }

    protected override bool ManeuverSuccesful()
    {
        return FarAwayFromScarlet();
    }

    protected override bool FarAwayFromScarlet()
    {
        return Vector3.Distance(m_Boss.transform.position, m_Scarlet.position) >= m_MinDistanceForSuccess;
    }

}
