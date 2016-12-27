using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCommand : PlayerCommand, Damage.DamageCallback {

    public float m_RegularHitDamage = 20f;
    public float m_FinalHitDamage = 30f;
    public float m_RiposteDamage = 50f;

    public float m_MaxTimeForCombo = 0.5f;
    public float m_DelayAfterAttack = 0.3f;
    public float m_DelayAfterLastAttack = 0.5f;
    public float m_DelayAfterRiposte = 0.5f;

    private Rigidbody m_ScarletBody;

    /// <summary>
    /// @todo this has to change!!
    /// </summary>
    public GameObject m_RiposteTarget;
    
    private IEnumerator m_AttackEnumerator;
    private IEnumerator m_ComboEnumerator;

    public GameObject m_DamageTrigger;
    private PlayerDamage m_PlayerDamage;

    private int m_CurrentCombo = 0;

    public bool m_RiposteActive = false;

    public AttackCallback m_AttackCallback;
    
    void Start () {
        m_CommandName = "Attack";
        m_CurrentCombo = 0;
	}

    public override void InitTrigger()
    {
        m_ScarletBody = m_Scarlet.GetComponent<Rigidbody>();
        m_PlayerDamage = m_DamageTrigger.GetComponent<PlayerDamage>();

        m_CommandName = "Attack";
        m_Trigger = new PressAxisTrigger(this, m_CommandName);
        m_PlayerDamage.m_Callback = this;
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

        if (m_RiposteActive)
        {
            m_PlayerDamage.m_Damage = m_RiposteDamage;
            m_AttackEnumerator = DelayEnd(m_DelayAfterRiposte);
            Hittable hittable = ((m_RiposteTarget == null) ? null : m_RiposteTarget.GetComponent<Hittable>());
            if (hittable != null)
                hittable.Hit(m_PlayerDamage);
            LookAtTarget();
        }
        else if (m_CurrentCombo < 3)
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

    private void LookAtTarget()
    {
        m_ScarletBody.transform.rotation = Quaternion.Euler(0, 
            Mathf.Atan2(m_ScarletBody.transform.position.x - m_RiposteTarget.transform.position.x,
                        m_RiposteTarget.transform.position.z - m_ScarletBody.transform.position.z) * Mathf.Rad2Deg * -1, 0);
    }

    private void PlayAttackAnimation(int currentCombo)
    {
        if (m_RiposteActive)
        {
            m_Animator.SetTrigger("RiposteTrigger");
        }
        else if (currentCombo == 0)
        {
            m_Animator.SetInteger("WhichAttack", UnityEngine.Random.Range(1, 3));
        }
        else if (currentCombo == 1)
        {
            m_Animator.SetInteger("WhichAttack", UnityEngine.Random.Range(3, 5));
        }
        else if (currentCombo == 2)
        {
            m_Animator.SetInteger("WhichAttack", UnityEngine.Random.Range(5, 7));
        }
        else if (currentCombo == 3)
        {
            m_Animator.SetInteger("WhichAttack", 7);
        }

        m_Animator.SetTrigger("AttackTrigger");
    }

    public override void CancelDelay()
    {
        m_PlayerDamage.m_Active = false;

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

        m_PlayerDamage.m_Active = false;
        m_Callback.OnCommandEnd(m_CommandName, this);
    }

    public void OnParryDamage()
    {
        // play some sound
        if (m_AttackCallback != null)
            m_AttackCallback.OnPlayerAttackParried();
    }

    public void OnBlockDamage()
    {
        // play some sound
        if (m_AttackCallback != null)
            m_AttackCallback.OnPlayerAttackBlocked();
    }

    public void OnSuccessfulHit()
    {
        // play some sound
    }

    public interface AttackCallback
    {
        void OnPlayerAttackParried();
        void OnPlayerAttackBlocked();

    }
}
