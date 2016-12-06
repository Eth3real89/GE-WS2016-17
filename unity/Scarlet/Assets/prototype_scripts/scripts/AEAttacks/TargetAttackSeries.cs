using UnityEngine;
using System.Collections;
using System;

public class TargetAttackSeries : AEAttackSeries
{
    public TargetAttackSetup m_Setup;
    public TargetAttack m_Attack;
    private GameObject m_TargetSetupPrefab;
    private GameObject m_TargetAttackPrefab;

    private IEnumerator m_EndEnumerator;

    public TargetAttackSeries(MonoBehaviour behaviour, GameObject m_TargetSetupPrefab, GameObject m_TargetAttackPrefab) : this(behaviour)
    {
        this.m_TargetSetupPrefab = m_TargetSetupPrefab;
        this.m_TargetAttackPrefab = m_TargetAttackPrefab;
    }

    public TargetAttackSeries(MonoBehaviour behaviour) : base(behaviour)
    {

    }

    public override void BeforeSeries(Transform bossTransform)
    {
        m_Parts = new AEAttackPart[6];

        for (int i = 0; i < 3; i++)
        {
            TargetAttackSetup setup = new TargetAttackSetup(m_TargetSetupPrefab, this);
            setup.delay = i * 2f;

            TargetAttack attack = new TargetAttack(m_TargetAttackPrefab, this, setup);
            attack.delay = setup.delay + 1f;

            m_Parts[i * 2] = setup;
            m_Parts[i * 2 + 1] = attack;
        }
    }

    public override void WhileActive()
    {
    }

    public override void StartAttack()
    {
        BeforeSeries(GameController.Instance.m_Boss.transform);
        RunSeries(GameController.Instance.m_Boss.transform);

        m_EndEnumerator = EndAttackAfter(7f);
        m_Behaviour.StartCoroutine(m_EndEnumerator);
    }

    private IEnumerator EndAttackAfter(float time)
    {
        yield return new WaitForSeconds(time);
        m_Callbacks.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        m_Behaviour.StopCoroutine(m_EndEnumerator);
        base.m_Cancelled = true;

        ((TargetAttackSetup)m_Parts[0]).Destroy();
        ((TargetAttack)m_Parts[1]).Destroy();

        ((TargetAttackSetup)m_Parts[2]).Destroy();
        ((TargetAttack)m_Parts[3]).Destroy();

        ((TargetAttackSetup)m_Parts[4]).Destroy();
        ((TargetAttack)m_Parts[5]).Destroy();

        this.m_Callbacks.OnAttackCancelled(this);
    }
}
