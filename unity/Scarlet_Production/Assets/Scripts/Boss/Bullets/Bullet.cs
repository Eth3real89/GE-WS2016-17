using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BulletBehaviour, BulletDamageTrigger.BulletDamageCallback, Damage.DamageCallback
{
    public static string BULLET_HIT_SCARLET_EVENT = "bullet_hit_scarlet";
    public enum StaggerScarlet {None, ALittle, Hard };

    public BulletDamage m_Damage;
    public BulletDamageTrigger m_DamageTrigger;

    public BulletHomingMovement m_DeflectedMovement;
    public BulletExpirationBehaviour m_DeflectedExpiration;

    public bool m_IgnoreExpirationBehaviour = false;

    public StaggerScarlet m_StaggerScarlet = StaggerScarlet.None;

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

        if (!m_IgnoreExpirationBehaviour)
            m_OnExpire.OnBulletExpires(this);

        try
        {
            if (this.m_Movement.gameObject != null)
                Destroy(m_Movement.gameObject);
        } catch { }

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

            BossHittable hittable = boss.GetComponentInChildren<BossHittable>();
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
        if (!m_Damage.m_Deflectable)
        {
            if (m_StaggerScarlet == StaggerScarlet.ALittle)
            {
                PlayerStaggerCommand.StaggerScarlet(false);
            }
            else if (m_StaggerScarlet == StaggerScarlet.Hard)
            {
                PlayerStaggerCommand.StaggerScarletAwayFrom(transform.position, 2, true);
            }
        }

        m_IgnoreExpirationBehaviour = true;
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

            ColorBulletRed();

            float speed = -1;
            if (m_Movement is BulletStraightMovement)
                speed = ((BulletStraightMovement)m_Movement).m_Speed;
            else if (m_Movement is BulletHomingMovement)
                speed = ((BulletHomingMovement)m_Movement).m_Speed;

            m_Movement = Instantiate(m_DeflectedMovement);
            ((BulletHomingMovement)m_Movement).m_Target = m_Damage.m_DeflectTarget.transform;
            ((BulletHomingMovement)m_Movement).m_Speed = (speed < 0) ? ((BulletHomingMovement)m_Movement).m_Speed : speed;

            m_IgnoreExpirationBehaviour = true;

            m_Expiration.CancelBehaviour(this);
            m_Expiration = m_DeflectedExpiration;

            m_Damage.m_Deflected = true;
        }
        else
        {
            m_IgnoreExpirationBehaviour = true;
            m_BulletCallbacks.OnBulletHitTarget(this);
        }
    }

    private void ColorBulletRed()
    {
        // simply because we don't want visual-only things to crash anything, ever:
        try
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach(Renderer renderer in renderers)
            {
                Material[] materials = renderer.materials;
                for(int i = 0; i < materials.Length; i++)
                {
                    materials[i].color = Color.red;
                    materials[i].SetColor("_EmissionColor", Color.red);
                    materials[i].SetColor("_EmissionColorUI", Color.red);
                }
            }

            ParticleSystem[] pss = GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem ps in pss)
            {
                var x = ps.main;
                x.startColor = Color.red;
                var col = ps.colorOverLifetime;
                Gradient gradNew = new Gradient();

                gradNew.SetKeys(
                    new GradientColorKey[] {new GradientColorKey(Color.red, 0f), new GradientColorKey(Color.red, 1f) },
                    new GradientAlphaKey[] {new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 0.76f), new GradientAlphaKey(0f, 1f) });

                col.color = gradNew;
            }

        } catch { }
    }
}
