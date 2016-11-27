using UnityEngine;
using System.Collections;
using System;

public class DoubleBeamAttackSeries : AEAttackSeries
{

    public GameObject m_BeamPrefab;

    public GameObject m_BeamWarningPrefab;
    private GameObject m_BeamWarning;

    public GameObject m_Boss;
    
    private IEnumerator m_EndEnumerator;

    public DoubleBeamAttackSeries(MonoBehaviour behaviour, GameObject breamPrefab, GameObject beamWarningPrefab, GameObject boss) : this(behaviour)
    {
        this.m_BeamPrefab = breamPrefab;
        this.m_BeamWarningPrefab = beamWarningPrefab;
        this.m_Boss = boss;
    }

    public DoubleBeamAttackSeries(MonoBehaviour behaviour) : base(behaviour)
    {
    }

    public override void BeforeSeries(Transform bossTransform)
    {
        Vector3 scarletPos = GameController.Instance.m_Scarlet.transform.position;
        Vector3 bossPos = m_Boss.transform.position;

        m_Parts = new AEAttackPart[2];

        BeamAttack beamAttack = new BeamAttack(m_BeamPrefab, this);
        beamAttack.m_StartPosition = bossPos;

        m_Parts[0] = beamAttack;

        ReverseBeamAttack reverseBeamAttack = new ReverseBeamAttack(m_BeamPrefab, this);
        reverseBeamAttack.m_StartPosition = bossPos;

        m_Parts[1] = reverseBeamAttack;

        m_Boss.GetComponentInChildren<Animator>().SetTrigger("SpellTrigger");
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

        m_BeamWarning = (GameObject) GameObject.Instantiate(m_BeamWarningPrefab, m_Boss.transform);
    }

    private IEnumerator RunSeriesAfter(float time)
    {
        yield return new WaitForSeconds(time);

        if (!m_Cancelled)
        {
            RemoveBeamWarning();
            m_Boss.GetComponentInChildren<AttackPattern>().SetInvincible(true);
            RunSeries(GameController.Instance.m_Boss.transform);
        }
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
        ((BeamAttack) m_Parts[0]).Destroy();
        ((ReverseBeamAttack) m_Parts[1]).Destroy();

        this.m_Callbacks.OnAttackCancelled(this);
    }

    public override bool DoCancelOnHit(PlayerControlsCharController.AttackType attackType)
    {
        return false;
    }

    private void RemoveBeamWarning()
    {
        if (m_BeamWarning != null)
        {
            GameObject.Destroy(m_BeamWarning);
            m_BeamWarning = null;
        }
    }
}
