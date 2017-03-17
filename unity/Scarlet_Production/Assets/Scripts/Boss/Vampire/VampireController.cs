using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VampireController : BossController {

    protected static float[][] s_LightAttackSounds = {
        new float[] {119.845f, 121.301f},
        new float[] {121.8f, 123.512f},
        new float[] {123.8f, 125.109f},
        new float[] {126.7f, 128.304f},
    };
    private int s_LightAttackSoundCount = -1;

    protected static float[][] s_HeavyAttackSounds = {
        new float[] {128.9f, 130.725f},
        new float[] {133.8f, 135.46f},
        new float[] {136.3f, 137.92f},
        new float[] {138.9f, 140.28f},
    };

    public Animator m_VampireAnimator;
    public BossfightCallbacks m_Callback;
    public BossMoveCommand m_MoveCommand;

    public GameObject m_LightGuardContainer;
    public LightGuard m_LightGuard;

    public Transform[] m_InLightZones;
    public Transform[] m_BetweenLightZones;

    public float m_InLightZoneProbability;

    public float m_DashTime = 1f;
    public float m_PerfectRotationTime = 0.4f;
    public float m_GatherLightTime = 1.5f;

    private bool m_ForceLightChoice;

    protected bool m_GatheringLight;

    protected IEnumerator m_DashEnumerator;
    protected IEnumerator m_BetweenCombosEnumerator;

    protected FancyAudioRandomClip m_LightAttackPlayer;
    protected FancyAudioRandomClip m_HeavyAttackPlayer;

    public virtual void StartPhase(BossfightCallbacks callbacks)
    {
        m_NotDeactivated = true;
        RegisterComboCallback();
        m_LightGuard = m_LightGuardContainer.GetComponent<LightGuard>();
        m_GatheringLight = false;

        m_BossHittable.RegisterInterject(this);
        m_Callback = callbacks;

        RegisterAnimationEvents();
        SetupAudio();
    }

    protected virtual void SetupAudio()
    {
        m_LightAttackPlayer = new FancyAudioRandomClip(s_LightAttackSounds, this.transform, "vampire");
        m_HeavyAttackPlayer = new FancyAudioRandomClip(s_HeavyAttackSounds, this.transform, "vampire");
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

    public override void OnComboEnd(AttackCombo combo)
    {
        m_ActiveCombo = null;
        m_BossHittable.RegisterInterject(this);
        m_BlockingBehaviour.m_TimesBlockBeforeParry = m_MaxBlocksBeforeParry;

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        VampireGatherLightFlyingObjectsManager vglfom = GetComponent<VampireGatherLightFlyingObjectsManager>();
        if (vglfom != null)
        {
            vglfom.OnAttackEnd();
        }

        m_NextComboTimer = StartNextComboAfter(combo.m_TimeAfterCombo);
        StartCoroutine(m_NextComboTimer);
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        if (m_NotDeactivated)
        {
            DeactivateLightShield();

            Transform t = DecideWhereToDashNext();
            DashTo(t, m_DashTime);
            yield return new WaitForSeconds(m_DashTime);

            StartCoroutine(PerfectTrackingRoutine(m_PerfectRotationTime));
            yield return new WaitForSeconds(m_PerfectRotationTime);

            GatherLight(m_GatherLightTime);
            yield return new WaitForSeconds(m_GatherLightTime);

            m_LightGuard.m_ExpandLightGuardTime = m_PerfectRotationTime;
            ActivateLightShield();
            StartCoroutine(PerfectTrackingRoutine(m_PerfectRotationTime));
            yield return new WaitForSeconds(m_PerfectRotationTime);

            yield return base.StartNextComboAfter(time);
        }
    }

    protected virtual Transform DecideWhereToDashNext()
    {
        bool moveToOpenSpace;
        if (m_ForceLightChoice)
        {
            moveToOpenSpace = false;
        }
        else
        {
            moveToOpenSpace = UnityEngine.Random.Range(0f, 1f) >= m_InLightZoneProbability;
        }

        m_ForceLightChoice = false;

        Transform[] possibleTargets = moveToOpenSpace ? m_BetweenLightZones : m_InLightZones;
        
        while(true)
        {
            int whichSpace = UnityEngine.Random.Range(0, possibleTargets.Length);
            if (Vector3.Distance(possibleTargets[whichSpace].position, transform.position) >= 2)
                return possibleTargets[whichSpace];
        }
    }

    public void GatherLight(float time)
    {
        StartCoroutine(GatherLightRoutine(time));
    }

    protected virtual IEnumerator GatherLightRoutine(float time)
    {
        m_GatheringLight = true;
        m_VampireAnimator.SetTrigger("GatherLightTrigger");

        m_LightGuard.m_ExpandLightGuardTime = time;
        VampireGatherLightFlyingObjectsManager vglfom = GetComponent<VampireGatherLightFlyingObjectsManager>();
        if (vglfom != null)
        {
            vglfom.OnGatherLightStart(time, transform);
        }

        yield return new WaitForSeconds(time);

        m_VampireAnimator.SetTrigger("StopGatherLightTrigger");
        m_GatheringLight = false;
    }

    protected virtual IEnumerator PerfectTrackingRoutine(float time)
    {
        m_VampireAnimator.SetTrigger("RotationTrigger");

        float t = 0;
        while ((t += Time.deltaTime) < time)
        {
            float goalRotation = BossTurnCommand.CalculateAngleTowards(transform, m_Scarlet.transform);

            while (goalRotation > 180)
                goalRotation -= 360;
            while (goalRotation < -180)
                goalRotation += 360;
           
            transform.Rotate(Vector3.up, goalRotation * t / time);
            yield return null;
        }

        m_VampireAnimator.SetTrigger("StopRotationTrigger");
    }

    public void ActivateLightShield()
    {
        m_LightGuardContainer.SetActive(true);
        m_LightGuard.ReattachVisualsToParent();
        if (m_LightGuard != null)
            m_LightGuard.Enable();

        m_LightGuard.DetachVisualsFromParent();
    }

    public void DeactivateLightShield()
    {
       // m_LightGuardContainer.SetActive(false);
        if (m_LightGuard != null)
            m_LightGuard.Disable();

       //m_LightGuard.ReattachVisualsToParent();
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
        if (dmg is BulletDamage)
        {
            return false;
        }

        if (m_LightGuardContainer.activeSelf)
        {
            return true;
        }
        else
        {
            if (m_GatheringLight)
            {
                if (m_BetweenCombosEnumerator != null)
                    StopCoroutine(m_BetweenCombosEnumerator);
                if (m_NextComboTimer != null)
                {
                    StopCoroutine(m_NextComboTimer);
                }

                VampireGatherLightFlyingObjectsManager vglfom = GetComponent<VampireGatherLightFlyingObjectsManager>();
                if (vglfom != null)
                {
                    vglfom.CancelIrregularly();
                }

                DeactivateLightShield();

                if (m_TimeWindowManager != null)
                {
                    m_TimeWindowManager.Activate(this);
                    m_BossHittable.RegisterInterject(m_TimeWindowManager);
                }

                CameraController.Instance.Shake();
                return false;
            }
            else
            {
                dmg.OnBlockDamage();
                return true;
            }
        }
    }

    public override void OnTimeWindowClosed()
    {
        if (m_NotDeactivated)
            StartCoroutine(DashIntoLightThenStartAttack());
    }

    public override void OnBossStaggerOver()
    {
        if (!m_NotDeactivated)
            return;

        m_ActiveCombo = null;
        m_BossHittable.RegisterInterject(this);
        m_BlockingBehaviour.m_TimesBlockBeforeParry = m_MaxBlocksBeforeParry;

        m_ForceLightChoice = true;
        m_BetweenCombosEnumerator = StartNextComboAfter(0f);
        StartCoroutine(m_BetweenCombosEnumerator);
    }

    protected virtual IEnumerator DashIntoLightThenStartAttack()
    {
        m_ForceLightChoice = true;
        Transform t = DecideWhereToDashNext();
        DashTo(t, m_DashTime);
        yield return new WaitForSeconds(m_DashTime);

        
        m_LightGuard.m_ExpandLightGuardTime = m_PerfectRotationTime;
        ActivateLightShield();
        StartCoroutine(PerfectTrackingRoutine(m_PerfectRotationTime));
        yield return new WaitForSeconds(m_PerfectRotationTime);

        yield return base.StartNextComboAfter(0f);
    }

    protected virtual IEnumerator ExecuteDash(Transform target, float time)
    {
        FARQ audio = FancyAudioEffectsSoundPlayer.Instance.PlayHoverDashSound(transform);

        Vector3 initialPos = transform.position + new Vector3();
        Vector3 targetPos = target.position - new Vector3(0, target.position.y - initialPos.y, 0);

        // @todo: determine direction for animation
        m_VampireAnimator.SetInteger("WhichDash", 1);
        m_VampireAnimator.SetTrigger("DashTrigger");

        float totalTime = 0;

        float t = 0;
        while((t += Time.deltaTime) < time - 0.5f)
        {
            totalTime += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos, targetPos, totalTime / time);
            yield return null;
        }
        
        m_VampireAnimator.SetTrigger("DashWindupTrigger");

        t = 0;
        while ((t += Time.deltaTime) < 0.5f)
        {
            totalTime += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos, targetPos, totalTime / time);
            yield return null;
        }
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        audio.StopIfPlaying();
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
    
    protected void PlayAttackSound(bool light)
    {
        if (light)
        {
            if(++s_LightAttackSoundCount % 4 == 0)
                m_LightAttackPlayer.PlayRandomSound();
        }
        else
        {
            m_HeavyAttackPlayer.PlayRandomSound();
        }
    }

    public override void CancelAndReset()
    {
        UnRegisterAnimationEvents();
        DeactivateLightShield();
        m_VampireAnimator.SetBool("RageModeActive", false);
        base.CancelAndReset();
    }

}
