using UnityEngine;
using System.Collections;
using System;

public class BeamAttackSeries : AEAttackSeries {

    public GameObject m_BeamPrefab;

    public GameObject m_Boss;

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

        BeamAttack beamAttack = new BeamAttack(m_BeamPrefab, this);
        beamAttack.m_StartPosition = bossPos;

        m_Parts[0] = beamAttack;
    }

    public override void WhileActive()
    {
    }

    public override void StartAttack()
    {
        BeforeSeries(m_Boss.transform);
        RunSeries(GameController.Instance.m_Boss.transform);
        m_Behaviour.StartCoroutine(EndAttackAfter(10f));
    }

    private IEnumerator EndAttackAfter(float time)
    {
        yield return new WaitForSeconds(time);
        m_Callbacks.OnAttackEnd(this);
    }
}
