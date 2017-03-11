using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicLightOrbAttack : BulletAttack {

    public Transform m_GoalObj;

    public override void StartAttack()
    {
        m_GoalObj.transform.position = m_Boss.transform.position + m_Boss.transform.forward * 5;

        base.StartAttack();
        m_Animator.SetTrigger("MagicSummonSphereTrigger");
    }

    public override void CancelAttack()
    {
    }
}
