using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCommand : PlayerCommand {

    public float m_RegularHitDamage = 20f;
    public float m_FinalHitDamage = 30f;

    public float m_MaxTimeForCombo = 0.5f;
    public float m_DelayAfterAttack = 0.3f;
    public float m_DelayAfterLastAttack = 0.5f;

    private Rigidbody m_ScarletBody;
    
    private IEnumerator m_AttackEnumerator;
    private IEnumerator m_ComboEnumerator;

    public GameObject m_DamageTrigger;
    private PlayerDamage m_PlayerDamage;

    private int m_CurrentCombo = 0;
    
    void Start () {
        m_CommandName = "Attack";
        m_CurrentCombo = 0;

        m_PlayerDamage = m_DamageTrigger.GetComponent<PlayerDamage>();
	}

    public override void InitTrigger()
    {
        m_Trigger = new DefaultAxisTrigger(this, m_CommandName);
        m_ScarletBody = m_Scarlet.GetComponent<Rigidbody>();
    }

    public override void TriggerCommand()
    {
        DoAttack();
    }

    private void DoAttack()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        m_ScarletBody.velocity = new Vector3(0, 0, 0);

        if (m_ComboEnumerator != null)
            StopCoroutine(m_ComboEnumerator);

        ActivateDamage();

        m_ComboEnumerator = CancelCombo();
        StartCoroutine(m_ComboEnumerator);
    }

    private void ActivateDamage()
    {
        m_PlayerDamage.m_Active = true;

        PlayAttackAnimation(m_CurrentCombo);

        if (m_CurrentCombo < 4)
        {
            m_PlayerDamage.m_Damage = m_RegularHitDamage;
            m_AttackEnumerator = DelayEnd(m_DelayAfterAttack);
            m_CurrentCombo++;
        }
        else
        {
            m_PlayerDamage.m_Damage = m_FinalHitDamage;
            m_AttackEnumerator = DelayEnd(m_DelayAfterLastAttack);
            m_CurrentCombo = 0;
        }

        StartCoroutine(m_AttackEnumerator);
    }

    private void PlayAttackAnimation(int currentCombo)
    {
        print("play attack anim. " + currentCombo);
    }

    public override void CancelDelay()
    {
        if (m_AttackEnumerator != null)
            StopCoroutine(m_AttackEnumerator);
    }

    private IEnumerator CancelCombo()
    {
        yield return new WaitForSeconds(m_MaxTimeForCombo);
        m_CurrentCombo = 0;
    }

    private IEnumerator DelayEnd(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_Callback.OnCommandEnd(m_CommandName, this);
    }
}
