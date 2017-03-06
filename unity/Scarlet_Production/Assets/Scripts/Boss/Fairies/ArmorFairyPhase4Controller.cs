using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorFairyPhase4Controller : ArmorFairyController {

    protected static float[][] s_HeavyAttackSoundsFemale =
    {
        new float[] {69.7f, 71.3f },
    };

    protected static float[][] s_LightAttackSoundsFemale =
    {
        new float[] {69.7f, 71.3f },
        new float[] {76f, 77.2f },
    };

    protected static float[][] s_StaggerSoundsFemale =
    {
        new float[] {76.5f, 77.9f },
        new float[] {79.4f, 80.5f },
    };

    public CharacterHealth m_ArmorHealth;

    public TurnTowardsScarlet m_TurnTowardsScarletCommand;

    protected override void InitializeAudioPlayers()
    {
        m_HeavyAttackPlayer = new FancyAudioRandomClip(s_HeavyAttackSoundsFemale, this.transform, "ae_fairy", 1f);
        m_LightAttackPlayer = new FancyAudioRandomClip(s_LightAttackSoundsFemale, this.transform, "ae_fairy", 1f);
       // m_ParriedPlayer = new FancyAudioRandomClip(s_ParriedAttackSounds, this.transform, "armor_fairy", 1f);
        m_StaggerPlayer = new FancyAudioRandomClip(s_StaggerSoundsFemale, this.transform, "ae_fairy", 1f);

        ArmorFairyHittable hittable = FindObjectOfType<ArmorFairyHittable>();
        if (hittable != null)
            hittable.SetSoundset(false);
    }

    protected override bool HandleHitDuringCombo(Damage dmg)
    {
        if (dmg.m_Type == Damage.DamageType.Riposte)
        {
            CancelComboIfActive();

            dmg.OnSuccessfulHit();
            m_TimeWindowManager.ActivateViaRiposte(this);
            m_BossHittable.RegisterInterject(m_TimeWindowManager);

            return false;
        }
        else
        {
            return HandleArmorPhase4Hit(dmg);
        }
    }

    protected virtual bool HandleArmorPhase4Hit(Damage dmg)
    {
        if (IsBackAttack(dmg) && !m_OnlyJustStaggered)
        {
            MLog.Log(LogType.BattleLog, 0, "Back Attack! " + this);
            CancelComboIfActive();

            dmg.OnSuccessfulHit();

            if (m_TimeWindowManager != null)
            {
                m_TimeWindowManager.Activate(this);
                m_BossHittable.RegisterInterject(m_TimeWindowManager);
            }
            return false;
        }
        else
        {
            dmg.OnSuccessfulHit();
            m_ArmorHealth.m_CurrentHealth -= dmg.DamageAmount() / 2f;

            return true;
        }
    }

    protected override bool HandleHitOutsideOfCombo(Damage dmg)
    {
        CancelComboIfActive();

        if (dmg.m_Type == Damage.DamageType.Riposte)
        {
            dmg.OnSuccessfulHit();
            m_TimeWindowManager.ActivateViaRiposte(this);
            m_BossHittable.RegisterInterject(m_TimeWindowManager);

            return false;
        }
        else
        {
            if (!m_OnlyJustStaggered)
            {
                MLog.Log(LogType.BattleLog, 0, "Armor Phase 4 successful Attack! " + this);

                dmg.OnSuccessfulHit();

                if (m_TimeWindowManager != null)
                {
                    m_TimeWindowManager.Activate(this);
                    m_BossHittable.RegisterInterject(m_TimeWindowManager);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public override void OnTimeWindowClosed()
    {
        MLog.Log(LogType.BattleLog, "On Time Window Was Closed, Controller");

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);
        CancelComboIfActive();

        if (!m_NotDeactivated)
            return;

        m_NextComboTimer = BrieflyRotatePerfectlyThenStartAttack();
        StartCoroutine(m_NextComboTimer);

        m_IFramesAfterStaggerTimer = InvulnerableAfterStagger();
        StartCoroutine(m_IFramesAfterStaggerTimer);
    }

    public override void OnBossStaggerOver()
    {
        MLog.Log(LogType.BattleLog, "On Boss Stagger Over, Controller");

        m_OnlyJustStaggered = true;

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);
        CancelComboIfActive();

        if (!m_NotDeactivated)
            return;

        m_NextComboTimer = BrieflyRotatePerfectlyThenStartAttack();
        StartCoroutine(m_NextComboTimer);

        m_IFramesAfterStaggerTimer = InvulnerableAfterStagger();
        StartCoroutine(m_IFramesAfterStaggerTimer);
    }

    protected virtual IEnumerator BrieflyRotatePerfectlyThenStartAttack()
    {
        float t = 0;
        while((t += Time.deltaTime) < 0.2f)
        {
            m_TurnTowardsScarletCommand.DoTurn();
            m_TurnTowardsScarletCommand.DoTurn();
            yield return null;
        }

        StartNextCombo();
    }

}
