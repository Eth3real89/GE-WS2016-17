using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingBehaviour : MonoBehaviour, HitInterject {

    public int m_TimesBlockBeforeParry;
    private int m_BlockCount;

    public float m_MaxBlockTime;

    private BossBlockCallback m_Callback;

    public Animator m_Animator;

    private bool m_Active = false;
    private IEnumerator m_BlockTimer;

    public AudioSource m_BlockAudio;

    public TurnTowardsScarlet m_TurnCommand;

    public void Activate(BossBlockCallback callback)
    {
        MLog.Log(LogType.BattleLog, 0, "Activating BlockBehaviour!");
        m_Animator.SetTrigger("BlockTrigger");
        m_Callback = callback;

        m_Active = true;
        m_BlockCount = 1;

        m_BlockTimer = StopBlockingAfter();
        StartCoroutine(m_BlockTimer);
    }

    private IEnumerator StopBlockingAfter()
    {
        yield return new WaitForSeconds(m_MaxBlockTime);

        m_Animator.SetTrigger("BlockWindupTrigger");

        m_Active = false;
        m_Callback.OnBlockingOver();
    }

    public bool OnHit(Damage dmg)
    {
        MLog.Log(LogType.BattleLog, "OnHit, BlockingBehaviour");

        if (!m_Active)
            return false;

        if (dmg is BulletDamage)
        {
            dmg.OnSuccessfulHit();
            return false;
        }

        if (m_TurnCommand != null)
            m_TurnCommand.DoTurn();
        CameraController.Instance.Shake();

        m_BlockCount++;

        if (m_BlockAudio != null)
            m_BlockAudio.Play();

        if (m_BlockTimer != null)
            StopCoroutine(m_BlockTimer);

        if (m_BlockCount > m_TimesBlockBeforeParry)
        {
            m_Animator.SetTrigger("ParryTrigger");
            m_Callback.OnBossParries();
            dmg.OnParryDamage();

            return true;
        }
        else
        {
            m_Callback.OnBossBlocks();
            dmg.OnBlockDamage();
        }


        m_BlockTimer = StopBlockingAfter();
        StartCoroutine(m_BlockTimer);

        return true;
    }

    public interface BossBlockCallback
    {
        void OnBossBlocks();
        void OnBossParries();

        void OnBlockingOver();
    }
}
