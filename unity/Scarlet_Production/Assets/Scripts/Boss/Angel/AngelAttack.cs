using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AngelAttack : BossAttack {

    public int m_SuccessLevel;
    public AngelAttackCallback m_SuccessCallback;

    public override void StartAttack()
    {
        m_SuccessLevel = -1;
        base.StartAttack();
    }

    public interface AngelAttackCallback
    {
        void ReportResult(AngelAttack attack);
    }

}
