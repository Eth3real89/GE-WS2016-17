using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AngelAttack : BossAttack {

    protected static float s_SpeedMultiplier = 1f;
    protected static float s_DamageMultiplier = 1f;

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

    public static void SetSpeedMultiplier(float multiplier)
    {
        s_SpeedMultiplier = multiplier;
    }

    public static void SetDamageMultiplier(float multiplier)
    {
        s_DamageMultiplier = multiplier;
    }

    public static float GetDamageMultiplier()
    {
        return s_DamageMultiplier;
    }

    protected float AdjustSpeed(float val)
    {
        return val * s_SpeedMultiplier;
    }

    protected float AdjustTime(float val)
    {
        return val / s_SpeedMultiplier;
    }

    protected float AdjustDmg(float val)
    {
        return val * s_DamageMultiplier;
    }

}
