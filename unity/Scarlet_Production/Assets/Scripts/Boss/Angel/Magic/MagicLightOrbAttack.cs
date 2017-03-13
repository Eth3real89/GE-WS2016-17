using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicLightOrbAttack : BulletAttack {

    public Transform m_GoalObj;

    public TurnTowardsScarlet m_TurnTowardsCenter;

    public override void StartAttack()
    {
        m_GoalObj.transform.position = m_Boss.transform.position + m_Boss.transform.forward * 5;

        m_TurnTowardsCenter.DoTurn();

        base.StartAttack();
        m_Animator.SetTrigger("MagicSummonSphereTrigger");
    }

    public override void CancelAttack()
    {
    }
}
