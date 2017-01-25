using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttack : BossAttack {

    public BulletSwarm m_BaseSwarm;

    public override void StartAttack()
    {
        base.StartAttack();

        m_BaseSwarm.Launch();
    }

    public override void CancelAttack()
    {
        if (m_BaseSwarm != null)
            m_BaseSwarm.Kill();
    }
}
