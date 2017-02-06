using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWaveThatStays : BlastWaveAttack {

    public float m_StayTime = 3f;
    protected IEnumerator m_StayEnumerator;

    protected override IEnumerator GrowWave()
    {
        yield return GrowRoutine();

        m_Callback.OnAttackEnd(this);

        float t = 0;
        while ((t += Time.deltaTime) < m_StayTime)
        {
            float distance = Vector3.Distance(m_Target.transform.position - new Vector3(0, m_Target.transform.position.y, 0), m_InitialCenterPos - new Vector3(0, m_InitialCenterPos.y, 0));

            if (WithinDistanceBounds(m_WaveSize, distance) && WithinAngleBounds(m_Angles))
            {
                DealDamage();
            }
            yield return null;
        }
        HideLightGuard();
        m_Visuals.gameObject.SetActive(false);
    }

}
