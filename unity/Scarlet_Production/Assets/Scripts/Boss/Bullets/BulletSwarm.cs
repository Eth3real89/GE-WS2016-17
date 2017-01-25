using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSwarm : BulletBehaviour {

    public List<BulletBehaviour> m_Instances;

    public BulletFactoryInvoker m_Invoker;

    public override void Launch()
    {
        base.Launch();

        m_Instances = new List<BulletBehaviour>();
        m_Invoker.Launch(this);
    }


    public override void MoveBy(Vector3 movement)
    {
        foreach (BulletBehaviour b in m_Instances)
        {
            b.MoveBy(movement);
        }
    }

    public override void Kill()
    {
        m_KillBullet = true;
        foreach (BulletBehaviour b in m_Instances)
        {
            b.Kill();
        }
    }
}
