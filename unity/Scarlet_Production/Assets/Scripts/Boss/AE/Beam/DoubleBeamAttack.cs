using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleBeamAttack : BeamAEAttack {

    public BeamAEDamage m_SecondDamage;
    public float m_AngleBetweenBeams = 45f;

    private bool m_IgnoreNextEvent = false;

    public override void StartAttack()
    {
        base.StartAttack();
        m_IgnoreNextEvent = false;
    }

    protected override IEnumerator BeforeExpansion()
    {
        // @todo test this!
        yield return base.BeforeExpansion();

        if (!m_InitiallyAimAtScarlet)
        {
            m_SecondDamage.SetAngle(-m_RotationAngle / 2);
        }

        m_Damage.transform.Rotate(Vector3.up, m_AngleBetweenBeams / 2);
        m_SecondDamage.gameObject.SetActive(true);
        m_SecondDamage.transform.Rotate(Vector3.up, -m_AngleBetweenBeams / 2);
        m_SecondDamage.Expand(m_ExpandTime, m_ExpandScale, this);
    }

    public override void OnExpansionOver(BeamAEDamage dmg)
    {
        if (m_IgnoreNextEvent)
        {
            m_IgnoreNextEvent = false;
            return;
        }

        m_IgnoreNextEvent = true;
        m_SecondDamage.Rotate(m_RotationTime, m_RotationAngle, this);
        base.OnExpansionOver(dmg);
    }

    public override void OnRotationOver(BeamAEDamage dmg)
    {
        if (m_IgnoreNextEvent)
        {
            m_IgnoreNextEvent = false;
            return;
        }

        m_IgnoreNextEvent = true;
        base.OnRotationOver(dmg);
    }

    public override void CancelAttack()
    {
        base.CancelAttack();
        m_SecondDamage.CancelDamage();
        m_SecondDamage.gameObject.SetActive(false);
    }


    public override void OnRotation(BeamAEDamage damage, float angle)
    {
        if (damage == m_Damage)
            base.OnRotation(damage, angle - m_AngleBetweenBeams / 2);
    }

    protected override IEnumerator RemoveBeamAfterWaiting()
    {
        yield return base.RemoveBeamAfterWaiting();
        m_SecondDamage.gameObject.SetActive(false);
        m_SecondDamage.m_Active = false;
    }

}
