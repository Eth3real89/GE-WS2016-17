using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheReapOverheadAttack : AngelMeleeAttack {

    public PlayerControls m_PlayerControls;

    protected bool m_StaggerCalled = false;

    public override void StartAttack()
    {
        base.StartAttack();
        m_StaggerCalled = false;
    }

    public override void OnSuccessfulHit()
    {
        base.OnSuccessfulHit();

        PlayerStaggerCommand.StaggerScarlet(false, m_Boss.transform.position);
        m_StaggerCalled = true;
    }

    protected override void EndAttack()
    {
        base.EndAttack();

        if (!m_StaggerCalled)
        {
            m_PlayerControls.EnableAllCommands();
        }
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
