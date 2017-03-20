using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfRagemodeController : WerewolfController
{

    protected static float[][] s_Phase3AttackSounds =
    {
        new float[] {183.2f, 183.7f },
        new float[] {183.9f, 184.4f },
        new float[] {184.6f, 185.0f },
        new float[] {185.2f, 185.7f },
    };

    private BossfightCallbacks m_Callbacks;
    public CharacterHealth m_BossHealth;

    public AttackCombo m_HitCombo;
    public AttackCombo m_LeapCombo;
    public AttackCombo m_ChaseCombo;

    public Animator m_WerewolfAnimator;

    public TurnTowardsScarlet m_TurnTowardsScarlet;

    public int m_TotalAttacks = 30;
    private int m_AttackCount = 0;
    private bool m_CancelAfterComboFinishes = false;

    protected bool m_Killable = false;

    protected FancyAudioRandomClip m_Phase3RandomPlayer;

    public void LaunchPhase(BossfightCallbacks callbacks)
    {
        m_NotDeactivated = true;
        m_AttackCount = 0;
        m_ActiveCombo = null;
        m_ComboActive = false;

        m_BossHittable.RegisterInterject(this);

        m_Callbacks = callbacks;

        m_Combos = new AttackCombo[3];
        m_Combos[0] = m_HitCombo;
        m_Combos[1] = m_LeapCombo;
        m_Combos[2] = m_ChaseCombo;

        base.RegisterComboCallback();

        m_Phase3RandomPlayer = new FancyAudioRandomClip(s_Phase3AttackSounds, this.transform, "werewolf");
        RegisterEventsForSound();

        StartCoroutine(StartAfterDelay());
    }

    private void EndRageMode()
    {
        CancelComboIfActive();
        StartCoroutine(MakeKillable());
    }

    protected IEnumerator MakeKillable()
    {
        m_BossHittable.RegisterInterject(this);
        m_Killable = true;
        yield return null;


        m_BossHealth.m_CurrentHealth = 3f;

        WerewolfHittable hittable = FindObjectOfType<WerewolfHittable>();
        if (hittable != null)
        {
            hittable.StopPlayingCriticalHPSound();
            hittable.m_DontPlaySound = true;
        }

        new FARQ().ClipName("werewolf").Location(this.transform).StartTime(115.4f).EndTime(126.8f).PlayUnlessPlaying();
    }

    protected void Kill()
    {
        m_WerewolfAnimator.SetTrigger("DeathTrigger");
        m_NotDeactivated = false;
        UnRegisterEventsForSound();
        m_Callbacks.PhaseEnd(this);
    }

    private new IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        m_CancelAfterComboFinishes = false;
        DecideNextCombo(null);
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        IncreaseAttackCount(combo);

        m_ActiveCombo = null;

        MLog.Log(LogType.BattleLog, "On Combo End, Controller");

        if (m_AttackCount >= m_TotalAttacks)
        {
            m_CancelAfterComboFinishes = true;
        }

        if (m_CancelAfterComboFinishes)
        {
            EndRageMode();
        }
        else
        {
            DecideNextCombo(combo);
        }
    }

    private IEnumerator StartNextComboAfter(float time, AttackCombo combo)
    {
        yield return new WaitForSeconds(time);
        combo.LaunchCombo();
    }

    public override bool OnHit(Damage dmg)
    {
        if (m_Killable)
        {
            Kill();
            return false;
        }
        else
        {
            m_WerewolfAnimator.SetTrigger("ParryTrigger");
            OnBossParries();
            dmg.OnParryDamage();
        }

        dmg.OnBlockDamage();
        return true;
    }

    private void IncreaseAttackCount(AttackCombo combo)
    {
        if (combo == m_HitCombo)
        {
            m_AttackCount += 2;
        }
        else if (combo == m_LeapCombo)
        {
            m_AttackCount++;
        }
        else if (combo == m_ChaseCombo)
        {
            m_AttackCount++;
        }

        m_AttackCount = Math.Min(m_AttackCount, m_TotalAttacks);

        float healthBefore = m_BossHealth.m_CurrentHealth;
        m_BossHealth.m_CurrentHealth = Mathf.Lerp(m_BossHealth.m_MaxHealth, 0, m_AttackCount / (float) (m_TotalAttacks + 1));

        if (healthBefore >= 0.3f * m_BossHealth.m_MaxHealth && m_BossHealth.m_CurrentHealth < 0.3f * m_BossHealth.m_MaxHealth)
        {
            WerewolfHittable hittable = FindObjectOfType<WerewolfHittable>();
            if (hittable != null)
                hittable.StartPlayingCriticalHPSound();
        }
    }

    private void DecideNextCombo(AttackCombo previous)
    {
        AttackCombo newCombo = m_HitCombo;

        float distance = Vector3.Distance(transform.position, m_Scarlet.transform.position);

        if (distance <= 4)
        {
            if (Mathf.Abs(m_TurnTowardsScarlet.CalculateAngleTowardsScarlet()) >= 40)
            {
                newCombo = m_ChaseCombo;
            }
        }
        else
        {
            newCombo = m_LeapCombo;
        }

        StartCoroutine(StartNextComboAfter(0.3f, newCombo));
    }

    public override void OnInterruptCombo(AttackCombo combo)
    {
        OnComboEnd(combo);
    }

    protected override void OnJumpStart()
    {
        base.OnJumpStart();
        m_Phase3RandomPlayer.PlayRandomSound();
    }

    protected override void OnMeleeDownswing()
    {
        base.OnMeleeDownswing();
        m_Phase3RandomPlayer.PlayRandomSound();
    }
}
