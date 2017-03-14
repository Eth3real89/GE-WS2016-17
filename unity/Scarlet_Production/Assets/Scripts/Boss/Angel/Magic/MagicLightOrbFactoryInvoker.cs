using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicLightOrbFactoryInvoker : BulletFactoryInvoker {

    public Bullet m_BaseBullet;

    public override void Launch(BulletSwarm bs, IEnumerator onFinish = null)
    {
        Bullet b = Instantiate(m_BaseBullet);

        b.transform.position = m_Base.position + Vector3.zero;
        b.transform.rotation = Quaternion.Euler(m_Base.rotation.eulerAngles);
        b.Launch(bs);
    }

}
