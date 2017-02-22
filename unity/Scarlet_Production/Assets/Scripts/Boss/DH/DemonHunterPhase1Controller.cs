using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterPhase1Controller : DemonHunterController {

    protected override IEnumerator PrepareAttack(int attackIndex)
    {
        if (m_Types[attackIndex] == AttackType.Pistols)
        {
            m_DHAnimator.SetTrigger("EquipPistolsTrigger");
            yield return new WaitForSeconds(1f);
        }
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        return base.StartNextComboAfter(time);
    }

}
