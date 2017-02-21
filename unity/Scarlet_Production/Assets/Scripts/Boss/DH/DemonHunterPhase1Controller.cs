using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterPhase1Controller : DemonHunterController {

    protected override IEnumerator PrepareAttack(int attackIndex)
    {
        yield return null;
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        return base.StartNextComboAfter(time);
    }

}
