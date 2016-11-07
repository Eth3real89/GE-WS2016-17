using UnityEngine;
using System.Collections;

public class TargetAttackSeries : AEAttackSeries
{
    public TargetAttackSetup m_Setup;
    public TargetAttack m_Attack;
    private GameObject m_TargetSetupPrefab;
    private GameObject m_TargetAttackPrefab;

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

}
