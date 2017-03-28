using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHittable : MonoBehaviour, Hittable {

    public CharacterHealth m_Health;

    public AudioSource m_OnHitAudio;

    public OnDamageTakenListener m_HittableListener;

    protected HitInterject m_Interject;

    public float m_InvulerableSecondsAfterHit = 0.5f;
    protected IEnumerator m_InvulernabilityEnumerator;
    protected bool m_Invulnerable;

    // the one method that is called here should be in some interface, but then unity would make it more difficult...
    public PlayerAttackCommand m_CounterDamageHandler;

    protected virtual void Start()
    {
        m_Invulnerable = false;
    }

    public void Hit(Damage damage)
    {
        if (m_Interject == null || !m_Interject.OnHit(damage))
        {
            if (m_Invulnerable || damage.DamageAmount() == 0)
                return;

            float healthBefore = m_Health.m_CurrentHealth;

            if (m_CounterDamageHandler != null && m_CounterDamageHandler.IsDamageActive())
            {
                m_Health.m_CurrentHealth = Mathf.Max(0, m_Health.m_CurrentHealth - damage.DamageAmount() * 1.2f);
                MLog.Log(LogType.BattleLog, "Player just got counter damage!");
            }
            else
            {
                m_Health.m_CurrentHealth = Mathf.Max(0, m_Health.m_CurrentHealth - damage.DamageAmount());
            }

            damage.OnSuccessfulHit();

            if (m_OnHitAudio != null)
                m_OnHitAudio.Play();

            if (m_HittableListener != null)
                m_HittableListener.OnDamageTaken(damage);

            if (m_Health.m_CurrentHealth <= healthBefore - 32 && m_Health.m_CurrentHealth > 0)
            {
                ScarletVOPlayer.Instance.PlayHeavyHitSound();
            } 
            else if (m_Health.m_CurrentHealth > 0)
            {
                ScarletVOPlayer.Instance.PlayLightHitSound();
            }

            if (m_Health.m_CurrentHealth <= 0.3f * m_Health.m_MaxHealth && m_Health.m_CurrentHealth > 0)
            {
                ScarletVOPlayer.Instance.PlayBadlyWoundedSound();
            }

            CameraController.Instance.Shake();

            m_InvulernabilityEnumerator = GrantInvulnerability();
            StartCoroutine(m_InvulernabilityEnumerator);
        }
    }

    public void RegisterInterject(HitInterject interject)
    {
        m_Interject = interject;
    }

    public HitInterject GetInterject()
    {
        return m_Interject;
    }

    private IEnumerator GrantInvulnerability()
    {
        m_Invulnerable = true;
        yield return new WaitForSeconds(m_InvulerableSecondsAfterHit);
        m_Invulnerable = false;
    }

    public void MakeVulnerable(bool vulnerable)
    {
        m_Invulnerable = !vulnerable;
    }

    public interface OnDamageTakenListener
    {
        void OnDamageTaken(Damage dmg);
    }

}
