using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardSpearAttack : AngelMeleeAttack {

    public float m_TimeBeforeHurlTakesPlace;
    
    public PlayerControls m_PlayerControls;

    public Transform m_SpearTip;
    protected IEnumerator m_MoveScarletAlongEnumerator;

    protected override void SetDamageActive()
    {
        if (m_Cancelled)
            return;

        m_Damage.m_Active = true;

        m_StateTimer = WaitBeforeAttackEnds();
        StartCoroutine(m_StateTimer);
    }

    protected virtual IEnumerator MoveScarletAlong()
    {
        Transform scarlet = m_PlayerControls.transform;
        Vector3 prevPosition = m_SpearTip.transform.position;

        while(true)
        {
            scarlet.transform.position += m_SpearTip.transform.position - prevPosition;
            prevPosition = m_SpearTip.transform.position;
            yield return null;
        }
    }


    public override void OnSuccessfulHit()
    {
        base.OnSuccessfulHit();

        m_PlayerControls.DisableAllCommands();
        m_PlayerControls.StopMoving();

        m_MoveScarletAlongEnumerator = MoveScarletAlong();
        StartCoroutine(m_MoveScarletAlongEnumerator);
    }

    protected override void EndAttack()
    {
        if (m_MoveScarletAlongEnumerator != null)
            StopCoroutine(m_MoveScarletAlongEnumerator);
            
        if (m_SuccessLevel < 1)
        {
            base.EndAttack();
        }
        else
        {
            m_Animator.SetTrigger("SpearThrowScarletRightTrigger");
            m_StateTimer = ThrowScarlet();
            StartCoroutine(m_StateTimer);
        }
    }

    protected IEnumerator ThrowScarlet()
    {
        yield return new WaitForSeconds(AdjustTime(m_TimeBeforeHurlTakesPlace));
        PlayerStaggerCommand.StaggerScarlet(true, (-m_PlayerControls.transform.right) + new Vector3(0, 1f, 0), 3);

        m_Callback.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        base.CancelAttack();

        if (m_MoveScarletAlongEnumerator != null)
            StopCoroutine(m_MoveScarletAlongEnumerator);
    }

    public override void HandleCollision(Collider other, bool initialCollision)
    {
    }

    public override void HandleScarletLeave(Collider other)
    {
    }

    public override void OnBlockDamage()
    {
    }

    public override void OnParryDamage()
    {
    }
}
