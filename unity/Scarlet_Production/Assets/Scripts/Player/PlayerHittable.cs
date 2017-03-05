using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHittable : MonoBehaviour, Hittable {

    public CharacterHealth m_Health;

    public AudioSource m_OnHitAudio;

    protected HitInterject m_Interject;

    public float m_InvulerableSecondsAfterHit = 0.5f;
    protected IEnumerator m_InvulernabilityEnumerator;
    protected bool m_Invulnerable;

    protected virtual void Start()
    {
        m_Invulnerable = false;
    }

    public void Hit(Damage damage)
    {
        if (m_Interject == null || !m_Interject.OnHit(damage))
        {
            if (m_Invulnerable)
                return;

            m_Health.m_CurrentHealth = Mathf.Max(0, m_Health.m_CurrentHealth - damage.DamageAmount());
            damage.OnSuccessfulHit();

            if (m_OnHitAudio != null)
                m_OnHitAudio.Play();

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
}
