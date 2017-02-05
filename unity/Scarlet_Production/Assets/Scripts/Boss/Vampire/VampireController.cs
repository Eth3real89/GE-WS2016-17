using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireController : BossController {

    public Animator m_VampireAnimator;
    public BossfightCallbacks m_Callback;
    public BossMoveCommand m_MoveCommand;

    public GameObject m_LightGuardContainer;
    public LightGuard m_LightGuard;

    protected IEnumerator m_DashEnumerator;

    public virtual void StartPhase(BossfightCallbacks callbacks)
    {
        RegisterComboCallback();
        m_LightGuard = m_LightGuardContainer.GetComponent<LightGuard>();

        m_BossHittable.RegisterInterject(this);
        m_Callback = callbacks;

        RegisterAnimationEvents();
    }

    public void RegisterAnimationEvents()
    {
        EventManager.StartListening(BulletAttack.START_EVENT_NAME, OnBulletAttackStart);
        EventManager.StartListening(BulletAttack.END_EVENT_NAME, OnBulletAttackEnd);
        EventManager.StartListening(BeamAEAttack.START_EVENT_NAME, OnBeamAttackStart);
        EventManager.StartListening(BeamAEAttack.END_EVENT_NAME, OnBeamAttackEnd);
        EventManager.StartListening(BlastWaveAttack.START_EVENT_NAME, OnBlastWaveStart);
    }

    public void UnRegisterAnimationEvents()
    {
        EventManager.StopListening(BulletAttack.START_EVENT_NAME, OnBulletAttackStart);
        EventManager.StopListening(BulletAttack.END_EVENT_NAME, OnBulletAttackEnd);
        EventManager.StopListening(BeamAEAttack.START_EVENT_NAME, OnBeamAttackStart);
        EventManager.StopListening(BeamAEAttack.END_EVENT_NAME, OnBeamAttackEnd);
        EventManager.StopListening(BlastWaveAttack.START_EVENT_NAME, OnBlastWaveStart);
    }

    public void GatherLight(float time)
    {
        StartCoroutine(GatherLightRoutine(time));
    }

    private IEnumerator GatherLightRoutine(float time)
    {
        m_VampireAnimator.SetTrigger("GatherLightTrigger");

        m_LightGuard.m_ExpandLightGuardTime = time;
        yield return new WaitForSeconds(time);

        m_VampireAnimator.SetTrigger("StopGatherLightTrigger");
    }

    public void ActivateLightShield()
    {
        m_LightGuardContainer.SetActive(true);
        if (m_LightGuard != null)
            m_LightGuard.Enable();
    }

    public void DeactivateLightShield()
    {
        m_LightGuardContainer.SetActive(false);
    }

    public void DashTo(Transform target, float time)
    {
        if (m_DashEnumerator != null)
            StopCoroutine(m_DashEnumerator);

        m_DashEnumerator = ExecuteDash(target, time);
        StartCoroutine(m_DashEnumerator);
    }

    public override bool OnHit(Damage dmg)
    {
        if (m_LightGuardContainer.activeSelf)
        {
            return true;
        }
        else
        {
            return base.OnHit(dmg);
        }
    }

    private IEnumerator ExecuteDash(Transform target, float time)
    {
        Vector3 dist = target.position - transform.position;
        dist.y = 0;

        dist /= time; // time it takes to get to that place

        // @todo: determine direction for animation
        m_VampireAnimator.SetInteger("WhichDash", 1);
        m_VampireAnimator.SetTrigger("DashTrigger");

        m_MoveCommand.m_Speed = dist.magnitude;
        m_MoveCommand.DoMove(dist.x, dist.z);

        yield return new WaitForSeconds(time - 0.5f);

        m_VampireAnimator.SetTrigger("DashWindupTrigger");

        yield return new WaitForSeconds(0.5f - (time - 0.5f));

        m_MoveCommand.StopMoving();
    }

    protected virtual void OnBulletAttackStart()
    {
        m_VampireAnimator.SetInteger("WhichBulletStance", 1);
        m_VampireAnimator.SetTrigger("BulletStanceTrigger");
    }

    protected virtual void OnBulletAttackEnd()
    {
        m_VampireAnimator.SetTrigger("BulletWindupTrigger");
    }

    protected virtual void OnBeamAttackStart()
    {
        m_VampireAnimator.SetTrigger("BeamStanceTrigger");
    }

    protected virtual void OnBeamAttackEnd()
    {
        m_VampireAnimator.SetTrigger("BeamWindupTrigger");
    }

    protected virtual void OnBlastWaveStart()
    {
        m_VampireAnimator.SetInteger("WhichBlastWave", 1);
        m_VampireAnimator.SetTrigger("BlastWaveTrigger");
    }
    
}
