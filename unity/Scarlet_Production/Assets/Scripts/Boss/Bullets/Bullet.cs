using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BulletBehaviour, BulletDamageTrigger.BulletDamageCallback, Damage.DamageCallback
{
    public static string BULLET_HIT_SCARLET_EVENT = "bullet_hit_scarlet";

    public BulletDamage m_Damage;
    public BulletDamageTrigger m_DamageTrigger;

    public BulletHomingMovement m_DeflectedMovement;
    public BulletExpirationBehaviour m_DeflectedExpiration;

    public override void Launch(BulletCallbacks callbacks)
    {
        base.Launch(callbacks);
        m_DamageTrigger.m_Callback = this;
        m_DamageTrigger.m_Active = true;

        m_Damage.m_Callback = this;
    }

    public override void Kill()
    {
        m_KillBullet = true;
        if (this.gameObject != null)
            Destroy(this.gameObject);
    }

    public override void MoveBy(Vector3 movement)
    {
        transform.position += movement;
    }

    public void OnBulletCollision(Collider other)
    {

    }

    public void OnScarletCollidesWithBullet(GameObject scarlet)
    {
        if (!m_Damage.m_Deflectable || m_Damage.m_Deflectable && !m_Damage.m_Deflected)
        {
            m_DamageTrigger.m_Active = false;

            PlayerHittable hittable = scarlet.GetComponentInChildren<PlayerHittable>();
            if (hittable != null)
            {
                EventManager.TriggerEvent(BULLET_HIT_SCARLET_EVENT);
                hittable.Hit(m_Damage);
            }
        }
    }

    public void OnBossCollidesWithBullet(GameObject boss)
    {
        if (m_Damage.m_Deflectable && m_Damage.m_Deflected)
        {
            m_DamageTrigger.m_Active = false;

            WerewolfHittable hittable = boss.GetComponentInChildren<WerewolfHittable>();
            if (hittable != null)
            {
                hittable.Hit(m_Damage);
            }
            Kill();
        }
    }

    public void OnParryDamage()
    {
        m_BulletCallbacks.OnBulletParried(this);
        HandleDeflect();
    }

    public void OnBlockDamage()
    {
        m_BulletCallbacks.OnBulletParried(this);
        HandleDeflect();
    }

    public void OnSuccessfulHit()
    {
        m_BulletCallbacks.OnBulletHitTarget(this);
    }

    private void HandleDeflect()
    {
        if (m_Damage.m_Deflected)
        {
            return;
        }

        if (m_Damage.m_Deflectable)
        {
            m_DamageTrigger.m_Active = true;
            m_BulletCallbacks.LoseBullet(this);

            m_Movement = Instantiate(m_DeflectedMovement);
            ((BulletHomingMovement)m_Movement).m_Target = m_Damage.m_DeflectTarget.transform;
            m_Expiration.CancelBehaviour(this);
            m_Expiration = m_DeflectedExpiration;

            m_Damage.m_Deflected = true;
        }
        else
        {
            m_BulletCallbacks.OnBulletHitTarget(this);
        }
    }
}
