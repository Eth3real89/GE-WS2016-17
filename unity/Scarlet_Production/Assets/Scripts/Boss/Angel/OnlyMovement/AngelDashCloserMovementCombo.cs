using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelDashCloserMovementCombo : AngelOnlyMovementCombo {

    public Transform m_Scarlet;

    public override bool AlreadyInGoodPosition()
    {
        return ManeuverSuccesful();
    }

    protected override bool ManeuverSuccesful()
    {
        return CloseToScarlet();
    }

    protected override bool CloseToScarlet()
    {
        return Vector3.Distance(m_Boss.transform.position, m_Scarlet.position) <= 1.5;
    }
}
