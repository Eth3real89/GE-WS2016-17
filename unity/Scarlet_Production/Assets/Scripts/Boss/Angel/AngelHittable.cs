using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelHittable : BossHittable {

    public bool m_TakeLessDamage = false;
    public float m_LessDamageFactor = 0.3f;

    public override void Hit(Damage damage)
    {
        if (m_Interject == null || !m_Interject.OnHit(damage))
        {
            if (damage.DamageAmount() == 0)
                return;

            m_Health.m_CurrentHealth = Mathf.Max(0, (m_Health.m_CurrentHealth - damage.DamageAmount() * (m_TakeLessDamage ? m_LessDamageFactor : 1f)));
            damage.OnSuccessfulHit();

            if (m_OnHitSignal != null)
                m_OnHitSignal.OnHit();

            if (m_OnHitAudio != null)
            {
                m_OnHitAudio.Play();
            }
        }
    }
}
