using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBeamOrbsInvoker : BulletFactoryInvoker {

    public Transform m_LeftShoulder;
    public Transform m_RightShoulder;

    protected override BulletBehaviour CreateBullet(int factoryIndex)
    {
        BulletBehaviour b = base.CreateBullet(factoryIndex);

        if (m_CurrentIteration == 0)
        {
            b.transform.position = m_RightShoulder.position;
            b.transform.parent = m_RightShoulder;
            ((BulletAbsoluteHomingMovement)((BulletMultiStageMovement)b.m_Movement).m_Movements[0]).m_Target = m_RightShoulder;
        }
        else if (m_CurrentIteration == 1)
        {
            b.transform.position = m_LeftShoulder.position;
            b.transform.parent = m_LeftShoulder;
            ((BulletAbsoluteHomingMovement)((BulletMultiStageMovement)b.m_Movement).m_Movements[0]).m_Target = m_LeftShoulder;
        }

        return b;
    }

}
