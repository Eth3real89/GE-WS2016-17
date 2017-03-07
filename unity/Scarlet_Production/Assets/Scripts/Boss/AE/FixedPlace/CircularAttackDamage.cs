using System;
using System.Collections;
using UnityEngine;

public class CircularAttackDamage : AEAttackDamage {

    public TurnTowardsScarlet m_TurnTowardsScarlet;

    public float m_Angle;
    public float m_Distance;

    public float m_MinDistance = 0;

    public override void Activate()
    {
        base.Activate();

        StartCoroutine(WhileActive());
    }

    private IEnumerator WhileActive()
    {
        while (m_Active)
        {
            if (WithinAngleBounds(m_Angle) && 
                WithinDistanceBounds())
            {
                Hittable hittable = m_TurnTowardsScarlet.m_Scarlet.GetComponentInChildren<Hittable>();
                if (hittable != null)
                {
                    hittable.Hit(this);
                }
            }

            yield return null;
        }
    }

    private bool WithinDistanceBounds()
    {
        float dist = Vector3.Distance(m_TurnTowardsScarlet.m_Scarlet.transform.position, transform.position);

        return dist <= m_Distance / 2 - 0.05 &&
               dist >= m_MinDistance / 2 + 0.05;
    }

    protected virtual bool WithinAngleBounds(float angles)
    {
        if (m_Angle == 0)
            return false;

        float angle = BossTurnCommand.CalculateAngleTowards(transform, m_TurnTowardsScarlet.m_Scarlet.transform);

        if (transform.rotation.eulerAngles.y < 0)
        {
            while (angle > 180)
            {
                angle -= 360;
            }
        }
        else
        {
            while (angle < -180)
            {
                angle += 360;
            }
        }

        float bossAngles = transform.rotation.eulerAngles.y;
        while (bossAngles > 180)
            bossAngles -= 360;
        while (bossAngles < -180)
            bossAngles += 360;

        angle += 180;
        if (angle > 180)
            angle -= 360;

        if (angles != 360)
        {
            angles -= 3; // because the "hitbox" should be smaller than the visuals.
        }

        if (Mathf.Abs(360 - angles) / 2 <= Mathf.Abs(angle))
            return true;

        return false;
    }

    public void DisableDamage()
    {
        m_Active = false;
    }

    public override BlockableType Blockable()
    {
        return BlockableType.None;
    }

    public override float DamageAmount()
    {
        return m_DamageAmount;
    }
}
