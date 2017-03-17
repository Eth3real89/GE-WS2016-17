using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBulletBeamOrbAttack : BulletAttack {

    public override void StartAttack()
    {
        base.StartAttack();
        m_Animator.SetTrigger("MagicSummonDualSpheresTrigger");
        AngelSoundPlayer.PlayMiscStanceSound();
    }

    protected override IEnumerator MoveOnTimer()
    {
        yield return new WaitForSeconds(m_MoveOnAfter);
        AngelSoundPlayer.PlayMiscWindupSound();
        m_Callback.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        if (m_Timer != null)
            StopCoroutine(m_Timer);
    }
}
