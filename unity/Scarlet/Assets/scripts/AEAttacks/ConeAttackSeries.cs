using UnityEngine;
using System.Collections;

public class ConeAttackSeries : AEAttackSeries
{
    public ConeAttackSetup m_Setup;
    public ConeAttack m_Attack;
    private GameObject m_ConeSetupPrefab;
    private GameObject m_ConeAttackPrefab;

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
        ConeAttackSetup setup = new ConeAttackSetup(m_ConeSetupPrefab, this);
        setup.delay = 0.5f;
        setup.m_StartPosition = bossTransform.position;

        ConeAttack attack = new ConeAttack(m_ConeAttackPrefab, this, setup);
        attack.delay = 2f;
        attack.m_StartPosition = bossTransform.position;

        m_Parts = new AEAttackPart[2];
        m_Parts[0] = setup;
        m_Parts[1] = attack;
    }
    
}
