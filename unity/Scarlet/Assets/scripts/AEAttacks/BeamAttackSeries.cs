using UnityEngine;
using System.Collections;
using System;

public class BeamAttackSeries : AEAttackSeries {

    public GameObject m_BeamPrefab;

    public GameObject m_Boss;

    private BeamAttack m_Attack;
    private IEnumerator m_EndEnumerator;

    public BeamAttackSeries(MonoBehaviour behaviour, GameObject breamPrefab, GameObject boss) : this(behaviour)
    {
        this.m_BeamPrefab = breamPrefab;
        this.m_Boss = boss;
    }

    public BeamAttackSeries(MonoBehaviour behaviour) : base(behaviour)
    {
    }

    public override void BeforeSeries(Transform bossTransform)
    {
        Vector3 scarletPos = GameController.Instance.m_Scarlet.transform.position;
        Vector3 bossPos = m_Boss.transform.position;

        m_Parts = new AEAttackPart[1];

        m_Attack = new BeamAttack(m_BeamPrefab, this);
        m_Attack.m_StartPosition = bossPos;

        m_Parts[0] = m_Attack;

        m_Boss.GetComponentInChildren<Animator>().SetTrigger("SpellTrigger");
        m_Boss.GetComponentInChildren<AttackPattern>().SetInvincible(true);
    }

    public override void WhileActive()
    {
    }

    public override void StartAttack()
    {
        BeforeSeries(m_Boss.transform);
        m_Behaviour.StartCoroutine(RunSeriesAfter(1f));
        m_EndEnumerator = EndAttackAfter(11f);
        m_Behaviour.StartCoroutine(m_EndEnumerator);
    }

    private IEnumerator RunSeriesAfter(float time)
    {
        yield return new WaitForSeconds(time);

        if (!m_Cancelled)
            RunSeries(GameController.Instance.m_Boss.transform);
    }

    private IEnumerator EndAttackAfter(float time)
    {
        yield return new WaitForSeconds(time);
        m_Boss.GetComponentInChildren<AttackPattern>().SetInvincible(false);
        m_Callbacks.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        m_Boss.GetComponentInChildren<AttackPattern>().SetInvincible(false);
        base.m_Cancelled = true;
        m_Behaviour.StopCoroutine(m_EndEnumerator);
        m_Attack.Destroy();
        this.m_Callbacks.OnAttackCancelled(this);
    }

    public override bool DoCancelOnHit(PlayerControlsCharController.AttackType attackType)
    {
        return false;
    }
}
