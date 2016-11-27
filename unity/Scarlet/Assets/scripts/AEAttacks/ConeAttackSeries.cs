using UnityEngine;
using System.Collections;
using System;

public class ConeAttackSeries : AEAttackSeries
{
    public ConeAttackSetup m_Setup;
    public ConeAttack m_Attack;
    private GameObject m_ConeSetupPrefab;
    private GameObject m_ConeAttackPrefab;

    private IEnumerator m_EndEnumerator;

    public ConeAttackSeries(MonoBehaviour behaviour, GameObject m_ConeSetupPrefab, GameObject m_ConeAttackPrefab) : this(behaviour)
    {
        this.m_ConeSetupPrefab = m_ConeSetupPrefab;
        this.m_ConeAttackPrefab = m_ConeAttackPrefab;
    }

    public ConeAttackSeries(MonoBehaviour behaviour) : base(behaviour)
    {

    }

    public override void BeforeSeries(Transform bossTransform)
    {
        m_Setup = new ConeAttackSetup(m_ConeSetupPrefab, this);
        m_Setup.delay = 0.5f;
        m_Setup.m_StartPosition = bossTransform.position;

        m_Attack = new ConeAttack(m_ConeAttackPrefab, this, m_Setup);
        m_Attack.delay = 2f;
        m_Attack.m_StartPosition = bossTransform.position;

        m_Parts = new AEAttackPart[2];
        m_Parts[0] = m_Setup;
        m_Parts[1] = m_Attack;
    }

    public override void WhileActive()
    {
    }

    public override void StartAttack()
    {
        BeforeSeries(GameController.Instance.m_Boss.transform);
        RunSeries(GameController.Instance.m_Boss.transform);

        m_EndEnumerator = EndAttackAfter(4f);
        m_Behaviour.StartCoroutine(m_EndEnumerator);
    }

    private IEnumerator EndAttackAfter(float time)
    {
        yield return new WaitForSeconds(time);
        m_Callbacks.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        base.m_Cancelled = true;

        m_Behaviour.StopCoroutine(m_EndEnumerator);
        m_Setup.Destroy();
        m_Attack.Destroy();
        this.m_Callbacks.OnAttackCancelled(this);
    }

    public override bool DoCancelOnHit(PlayerControlsCharController.AttackType attackType)
    {
        return attackType == PlayerControlsCharController.AttackType.LastHitInCombo;
    }
}
