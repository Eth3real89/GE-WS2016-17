using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingConeAttack : GrowingAEAttack {

    public enum ConeVisualsType
    {
        SeeThrough
    }

    public ConeVisualsType m_VisualsType;
    public Transform m_AttackContainer;
    protected GrowingConeAttackVisuals m_AttackVisuals;
    protected CircularAttackDamage m_Damage;

    public float m_Angle;
    public float m_EndSize;
    public float m_GrowTime;

    public float m_LineWidth = 1f;

    protected IEnumerator m_GrowEnumerator;

    public bool m_LeaveCone = false;

    public override void StartAttack()
    {
        base.StartAttack();
        LoadPrefab();

        m_GrowEnumerator = Grow();
        StartCoroutine(m_GrowEnumerator);
    }

    protected void LoadPrefab()
    {
        if (m_Damage != null)
            return;

        if (m_VisualsType == ConeVisualsType.SeeThrough)
        {
            m_Damage = Instantiate(AEPrefabManager.Instance.m_ConeSeeThroughVisuals).GetComponent<CircularAttackDamage>();
            m_AttackVisuals = m_Damage.GetComponent<GrowingConeAttackVisuals>();

            m_Damage.transform.parent = m_AttackContainer;

            m_Damage.transform.localPosition = Vector3.zero;
            m_Damage.transform.localScale = Vector3.one;
            m_Damage.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    protected virtual IEnumerator Grow()
    {
        m_AttackVisuals.SetAngle(m_Angle);
        m_Damage.m_Angle = m_Angle;

        m_Damage.m_Distance = 0f;
        m_Damage.Activate();
        m_AttackVisuals.SetRadius(1f);
        m_AttackVisuals.ShowAttack();


        float t = 0;
        while((t += Time.deltaTime) < m_GrowTime)
        {
            float currentSize = (t / m_GrowTime) * m_EndSize;

            m_AttackVisuals.ScaleTo(new Vector3(1, 1, 1) * currentSize);
            m_Damage.m_Distance = currentSize;

            yield return null;
        }

        AfterGrow();
    }

    protected virtual void AfterGrow()
    {
        if (m_LeaveCone)
        {
            m_Callback.OnAttackEnd(this);
            return;
        }

        m_Damage.m_Active = false;
        m_Damage.DisableDamage();
        m_AttackVisuals.HideAttack();

        m_Callback.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        base.CancelAttack();

        if (m_AttackVisuals != null)
            m_AttackVisuals.HideAttack();
        m_Damage.DisableDamage();
        m_Damage.m_Active = false;

        if (m_GrowEnumerator != null)
            StopCoroutine(m_GrowEnumerator);
    }


}
