using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSwayingBeamAttack : SwayingBeamAttack
{

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
        yield return base.BeforeExpansion();


        if (!m_InitiallyAimAtScarlet)
        {
            m_SecondDamage.SetAngle(-m_RotationAngle / 2);
        }

        m_Damage.transform.Rotate(Vector3.up, m_AngleBetweenBeams / 2);
        m_SecondDamage.transform.Rotate(Vector3.up, -m_AngleBetweenBeams / 2);
        m_SecondDamage.Expand(m_ExpandTime, m_ExpandScale, this);
    }

    public override void OnExpansionOver()
    {
        if (m_IgnoreNextEvent)
        {
            m_IgnoreNextEvent = false;
            return;
        }

        if (m_SecondDamage is SwayingAEDamage)
        {
            ((SwayingAEDamage)m_SecondDamage).m_InitiallyAimAtScarlet = this.m_InitiallyAimAtScarlet;
            ((SwayingAEDamage)m_SecondDamage).m_NumSways = this.m_NumSways;
        }
        m_IgnoreNextEvent = true;
        m_SecondDamage.Rotate(m_RotationTime, m_RotationAngle, this);
        base.OnExpansionOver();
    }

    public override void OnRotationOver()
    {
        if (m_IgnoreNextEvent)
        {
            m_IgnoreNextEvent = false;
            return;
        }

        m_IgnoreNextEvent = true;
        base.OnRotationOver();
    }

    public override void CancelAttack()
    {
        base.CancelAttack();
        m_SecondDamage.CancelDamage();
    }

    protected override IEnumerator RemoveBeamAfterWaiting()
    {
        yield return base.RemoveBeamAfterWaiting();
        m_SecondDamage.gameObject.SetActive(false);
        m_SecondDamage.m_Active = false;
    }

}
