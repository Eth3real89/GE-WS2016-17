using System;
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

    public float m_MaxAllowedScarletDistance = 4f;
    public float m_MinTimeBetweenWaves = 2.5f;
    public AttackCombo m_OnScarletEscapesDistance;
    protected AttackCombo m_OnScarletEscapesDistanceInstance;
    protected IEnumerator m_KeepScarletCloseEnumerator;
    protected int m_DistanceValuesLag = 4;

    protected float m_TimeLastWaveLaunched;

    public override void Initialize(FairyControllerCallbacks callbacks)
    {
        base.Initialize(callbacks);

        m_TimeLastWaveLaunched = -1;

        m_OnScarletEscapesDistance.m_Callback = this;
    }

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

    public override void OnComboStart(AttackCombo combo)
    {
        base.OnComboStart(combo);

        if (combo.transform.parent == m_OnScarletEscapesDistance)
        {
            combo.m_Animator.SetInteger("WhichAttackAnimation", 3);
            combo.m_Animator.SetTrigger("MeleeDownswingTrigger");
            combo.m_Animator.SetTrigger("MeleeUpswingTrigger");

            m_ActiveCombo = null; // means: this one can't be cancelled!!
        }

        if (m_KeepScarletCloseEnumerator == null && combo.transform.parent != m_OnScarletEscapesDistance)
        {
            m_KeepScarletCloseEnumerator = EnsureScarletIsClose();
            StartCoroutine(m_KeepScarletCloseEnumerator);
        }
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        base.OnComboEnd(combo);

        if (m_KeepScarletCloseEnumerator != null)
            StopCoroutine(m_KeepScarletCloseEnumerator);

        m_KeepScarletCloseEnumerator = EnsureScarletIsClose();
        StartCoroutine(m_KeepScarletCloseEnumerator);
    }

    protected virtual IEnumerator EnsureScarletIsClose()
    {
        List<float> distValues = new List<float>();

        while(true)
        {
            float distanceToScarlet = Vector3.Distance(m_BossHittable.transform.position, m_Scarlet.transform.position);
            distValues.Add(distanceToScarlet);

            bool tooFarAway = CheckDistanceValues(distValues);
            if (tooFarAway)
            {
                break;
            }

            yield return null;
        }

        float t = 0;
        do
        {
            for(int i = 0; i < 4; i++)
                m_TurnTowardsScarletCommand.DoTurn();
            yield return null;
        } while ((t += Time.deltaTime) < 0.1f);

        OnScarletTooFarAway();
    }

    protected void OnScarletTooFarAway()
    {
        CancelComboIfActive();
        if (m_KeepScarletCloseEnumerator != null)
            StopCoroutine(m_KeepScarletCloseEnumerator);

        m_OnScarletEscapesDistanceInstance = Instantiate(m_OnScarletEscapesDistance, m_OnScarletEscapesDistance.transform);
        m_OnScarletEscapesDistanceInstance.m_Callback = this;
        m_OnScarletEscapesDistanceInstance.LaunchCombo();
        m_TimeLastWaveLaunched = Time.timeSinceLevelLoad;
    }

    protected bool CheckDistanceValues(List<float> distValues)
    {
        if (distValues.Count < m_DistanceValuesLag || m_TimeLastWaveLaunched + m_MinTimeBetweenWaves > Time.timeSinceLevelLoad)
            return false;

        while (distValues.Count > m_DistanceValuesLag)
        {
            distValues.RemoveAt(0);
        }

        for (int i = 0; i < distValues.Count; i++)
        {
            if (distValues[i] < m_MaxAllowedScarletDistance)
            {
                return false;
            }
        }

        return true;
    }
}
