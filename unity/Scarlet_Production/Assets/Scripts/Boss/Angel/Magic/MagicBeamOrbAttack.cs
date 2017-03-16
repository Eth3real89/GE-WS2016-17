using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBeamOrbAttack : BulletAttack {

    public override void StartAttack()
    {
        base.StartAttack();
        m_Animator.SetTrigger("MagicSummonDualSpheresTrigger");
    }

    public override void CancelAttack()
    {
    }
}
