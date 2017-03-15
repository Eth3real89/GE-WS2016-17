using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Damage {

    public float m_DamageAmount;

    protected float m_Speed;
    protected float m_MaxLifetime;

    protected IEnumerator m_Timer;

    public void LaunchSpear(float speed, float maxLifetime)
    {
        transform.parent = null;
        m_Speed = speed;
        m_MaxLifetime = maxLifetime;

        m_Active = true;

        m_Timer = Fly();
        StartCoroutine(m_Timer);
    }

    protected IEnumerator Fly()
    {
        bool first = true;
        float t = 0;
        while((t += Time.deltaTime) < m_MaxLifetime)
        {
            if (!first && gameObject == null || !gameObject.activeInHierarchy)
                yield break;

            first = false;
            transform.position += transform.forward * m_Speed * Time.deltaTime;
            yield return null;
        }

        if (gameObject != null && gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Active)
            HandleTriggerHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_Active)
            HandleTriggerHit(other);
    }

    protected void HandleTriggerHit(Collider other)
    {
        PlayerHittable hittable = other.GetComponent<PlayerHittable>();
        if (hittable != null)
        {
            hittable.Hit(this);

            m_Active = false;
            gameObject.SetActive(false);
        }
    }

    public override BlockableType Blockable()
    {
        return BlockableType.None;
    }

    public override float DamageAmount()
    {
        return AngelAttack.GetDamageMultiplier() * m_DamageAmount;
    }
}
