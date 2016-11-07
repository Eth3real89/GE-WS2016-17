using UnityEngine;
using System.Collections;
using System;

public class DoubleBeamAttackSeries : AEAttackSeries
{

    public GameObject m_BeamPrefab;

    public GameObject m_Boss;

    public DoubleBeamAttackSeries(MonoBehaviour behaviour, GameObject breamPrefab, GameObject boss) : this(behaviour)
    {
        this.m_BeamPrefab = breamPrefab;
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
    }
}
