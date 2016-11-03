using UnityEngine;
using System.Collections;
using System;

public class PizzaAttackSeries : AEAttackSeries {

    public PizzaAttackSetup m_Setup;
    public PizzaAttack m_Attack;
    private GameObject m_PizzaSetupPrefab;
    
    public PizzaAttackSeries(MonoBehaviour behaviour, GameObject m_PizzaSetupPrefab) : this(behaviour)
    {
        this.m_PizzaSetupPrefab = m_PizzaSetupPrefab;
    }

    public PizzaAttackSeries(MonoBehaviour behaviour) : base(behaviour)
    {

    }

    public override void BeforeSeries(Transform bossTransform)
    {
        m_Parts = new AEAttackPart[4];

        for (int i = 0; i < m_Parts.Length; i++)
        {
            PizzaAttackSetup setup = new PizzaAttackSetup(m_PizzaSetupPrefab, this);
            setup.delay = (i + 1) * 0.5f;

            float angle = i * 90;
            setup.m_StartRotation = Quaternion.Euler(0, angle, 0);
            setup.m_StartPosition = bossTransform.position;

            setup.m_StartPosition = bossTransform.position - new Vector3(0, bossTransform.position.y, 0);

            m_Parts[i] = setup;
        }
    }

}
