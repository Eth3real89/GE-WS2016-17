using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBulletBeamOrbsInvoker : BulletFactoryInvoker {

    public BulletBehaviour m_Prefab;

    public Transform m_LeftShoulder;
    public Transform m_RightShoulder;

    protected float m_FirstBulletSpawnTime;

    protected override BulletBehaviour CreateBullet(int factoryIndex)
    {
        BulletBehaviour b = Instantiate(m_Prefab);
        b.m_Movement = Instantiate(b.m_Movement);
        b.m_OnExpire = Instantiate(b.m_OnExpire);
        b.m_Expiration = Instantiate(b.m_Expiration);

        if (m_CurrentIteration == 0)
        {
            b.transform.position = m_LeftShoulder.position;
            b.transform.parent = m_LeftShoulder;
            m_FirstBulletSpawnTime = Time.timeSinceLevelLoad;
        }
        else if (m_CurrentIteration == 1)
        {
            b.transform.position = m_RightShoulder.position;
            b.transform.parent = m_RightShoulder;
        }

        return b;
    }

}
