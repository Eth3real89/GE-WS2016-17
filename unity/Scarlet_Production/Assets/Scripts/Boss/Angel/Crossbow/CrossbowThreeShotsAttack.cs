using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowThreeShotsAttack : BulletAttack {

    public float m_YToReach;
    public float m_TimeToReachY;

    public override void StartAttack()
    {
        base.StartAttack();
        m_Animator.SetTrigger("CrossbowAttackTrigger");
    }

    protected override IEnumerator MoveOnTimer()
    {
        float totalTime = m_MoveOnAfter + m_TimeToReachY;

        bool idleTriggerSet = false;

        float t = 0;
        while ((t += Time.deltaTime) < totalTime)
        {
            if (t > m_MoveOnAfter && !idleTriggerSet)
            {
                AngelSoundPlayer.PlayMiscWindupSound();
                m_Animator.SetTrigger("IdleTrigger");
                idleTriggerSet = true;
            }

            m_Boss.transform.position = new Vector3(m_Boss.transform.position.x,
                Mathf.Lerp(m_Boss.transform.position.y, m_YToReach, Mathf.Pow(t / totalTime, 5)),
                m_Boss.transform.position.z);
            yield return null;
        }

        m_Boss.transform.position = new Vector3(m_Boss.transform.position.x,
                m_YToReach,
                m_Boss.transform.position.z);

        m_Callback.OnAttackEnd(this);
    }


}
