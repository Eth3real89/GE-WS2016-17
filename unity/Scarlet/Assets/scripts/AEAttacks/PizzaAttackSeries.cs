using UnityEngine;
using System.Collections;
using System;

public class PizzaAttackSeries : AEAttackSeries {

    public PizzaAttackSetup m_Setup;
    public PizzaAttack m_Attack;
    private GameObject m_PizzaSetupPrefab;
    private GameObject m_PizzaAttackPrefab;
    
    public PizzaAttackSeries(MonoBehaviour behaviour, GameObject m_PizzaSetupPrefab, GameObject m_PizzaAttackPrefab) : this(behaviour)
    {
        this.m_PizzaSetupPrefab = m_PizzaSetupPrefab;
        this.m_PizzaAttackPrefab = m_PizzaAttackPrefab;
    }

    public PizzaAttackSeries(MonoBehaviour behaviour) : base(behaviour)
    {

    }

    public override void BeforeSeries(Transform bossTransform)
    {
        m_Parts = new AEAttackPart[8];

        for (int i = 0; i < m_Parts.Length; i += 2)
        {
            int sliceIndex = i / 2;

            PizzaAttackSetup setup = MakeSetup(sliceIndex, bossTransform);
            PizzaAttack attack = MakeAttack(sliceIndex, bossTransform);


            m_Parts[i] = setup;
            m_Parts[i + 1] = attack;
        }
    }

    private PizzaAttackSetup MakeSetup(int sliceIndex, Transform bossTransform)
    {

        PizzaAttackSetup setup = new PizzaAttackSetup(m_PizzaSetupPrefab, this);
        setup.delay = (sliceIndex + 1) * 0.5f;

        float angle = sliceIndex * 90;
        setup.m_StartRotation = Quaternion.Euler(0, angle, 0);
        setup.m_StartPosition = bossTransform.position;

        setup.m_StartPosition = bossTransform.position - new Vector3(0, bossTransform.position.y, 0);

        return setup;
    }

    private PizzaAttack MakeAttack(int sliceIndex, Transform bossTransform)
    {

        PizzaAttack attack = new PizzaAttack(m_PizzaAttackPrefab, this);
        attack.delay = 0.3f + (sliceIndex + 1) * 0.5f;

        float angle = sliceIndex * 90;
        attack.m_StartRotation = Quaternion.Euler(0, angle, 0);
        attack.m_StartPosition = bossTransform.position;

        attack.m_StartPosition = bossTransform.position - new Vector3(0, bossTransform.position.y, 0);

        return attack;
    }

}
