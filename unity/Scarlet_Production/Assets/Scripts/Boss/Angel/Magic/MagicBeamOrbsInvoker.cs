using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBeamOrbsInvoker : BulletFactoryInvoker {

    public BulletBehaviour m_Prefab;

    public Transform m_LeftShoulder;
    public Transform m_RightShoulder;

    protected float m_FirstBulletSpawnTime;

    protected override BulletBehaviour CreateBullet(int factoryIndex)
    {
        BulletBehaviour b = Instantiate(m_Prefab);
        b.m_Movement = Instantiate(b.m_Movement);
        b.m_OnExpire = Instantiate(b.m_OnExpire);

        if (m_CurrentIteration == 0)
        {
            b.transform.position = m_LeftShoulder.position;
            b.transform.parent = m_LeftShoulder;
            //((BulletAbsoluteHomingMovement)((BulletMultiStageMovement)b.m_Movement).m_Movements[0]).m_Target = m_RightShoulder;
            m_FirstBulletSpawnTime = Time.timeSinceLevelLoad;
        }
        else if (m_CurrentIteration == 1)
        {
            b.transform.position = m_RightShoulder.position;
            b.transform.parent = m_RightShoulder;
            //((BulletAbsoluteHomingMovement)((BulletMultiStageMovement)b.m_Movement).m_Movements[0]).m_Target = m_LeftShoulder;
            ((MagicBeamOrbMovement)b.m_Movement).m_MinMovementTime -= Time.timeSinceLevelLoad - m_FirstBulletSpawnTime;

        }

        return b;
    }

}
