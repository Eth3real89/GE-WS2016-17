using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryCommand : PlayerCommand, HitInterject
{
    public static string COMMAND_EVENT_TRIGGER = "user_parry";

    public PlayerHittable m_ScarletHittable;

    public ParryCallback m_ParryCallback;

    public float m_PerfectParryTime = 0.025f;
    public float m_OkParryTime = 0.125f;
    public float m_ParryCooldownTime = 0.05f;

    public AudioSource m_ParryAudioPlayer;
    public AudioClip m_BlockAudio;
    public AudioClip m_ParryAudio;

    public GameObject m_BlockStanceEffect;

    private enum ParryState { Perfect, Ok, Cooldown, None };
    private ParryState m_CurrentState;
    private IEnumerator m_ParryTimer;


    // EFFECTS
    public GameObject m_MeleeBlockEffect;
    public GameObject m_MeleeParryEffect;

    private Component[] m_CompsMeleeBlock;
    private Light m_ImpactLightMeleeBlock;

    private Component[] m_CompsMeleeParry;
    private Light m_ImpactLightMeleeParry;

    public GameObject m_BulletBlockEffect;
    public GameObject m_BulletDeflectEffect;

    private Component[] m_CompsBulletBlock;
    private Light m_ImpactLightBulletBlock;

    private Component[] m_CompsBulletDeflect;
    private Light m_ImpactLightBulletDeflect;

    private void Start()
    {
        m_CommandName = "Parry";
        m_CurrentState = ParryState.None;
        
        SetupBulletEffects();
        SetupMeleeEffects();
    }

    private void SetupBulletEffects()
    {
        // could be refactored, since all of these effects are essentially the same (not quite, but almost...)
        m_CompsBulletBlock = m_BulletBlockEffect.GetComponentsInChildren<ParticleSystem>(true);
        m_ImpactLightBulletBlock = m_BulletBlockEffect.GetComponentInChildren<Light>(true);
        m_ImpactLightBulletBlock.enabled = false;

        m_CompsBulletDeflect = m_BulletDeflectEffect.GetComponentsInChildren<ParticleSystem>(true);
        m_ImpactLightBulletDeflect = m_BulletDeflectEffect.GetComponentInChildren<Light>(true);
        m_ImpactLightBulletDeflect.enabled = false;
    }

    private void SetupMeleeEffects()
    {
        m_CompsMeleeBlock = m_MeleeBlockEffect.GetComponentsInChildren<ParticleSystem>(true);
        m_ImpactLightMeleeBlock = m_MeleeBlockEffect.GetComponentInChildren<Light>(true);
        m_ImpactLightMeleeBlock.enabled = false;

        m_CompsMeleeParry = m_MeleeParryEffect.GetComponentsInChildren<ParticleSystem>(true);
        m_ImpactLightMeleeParry = m_MeleeParryEffect.GetComponentInChildren<Light>(true);
        m_ImpactLightMeleeParry.enabled = false;
    }

    public override void InitTrigger()
    {
        m_CommandName = "Parry";
        m_Trigger = new PressAxisTrigger(this, m_CommandName);
    }

    public override void TriggerCommand()
    {
        DoParry();
    }

    private void DoParry()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        if (m_ParryTimer != null)
            StopCoroutine(m_ParryTimer);

        EventManager.TriggerEvent("user_parry");

        m_ScarletHittable.RegisterInterject(this);
        m_CurrentState = ParryState.Perfect;
        m_ParryTimer = SetParryState(m_PerfectParryTime, ParryState.Ok);

        m_BlockStanceEffect.SetActive(true);

        StartCoroutine(HideBlockStanceEffect());
        StartCoroutine(m_ParryTimer);

        m_Animator.SetTrigger("ParryTrigger");
    }

    private IEnumerator HideBlockStanceEffect()
    {
        yield return new WaitForSeconds(1.0f);

        m_BlockStanceEffect.SetActive(false);
    }

    private IEnumerator SetParryState(float time, ParryState nextState)
    {
        yield return new WaitForSeconds(time);
        m_CurrentState = nextState;

        if (nextState == ParryState.Ok)
        {
            m_ParryTimer = SetParryState(m_OkParryTime, ParryState.Cooldown);
            StartCoroutine(m_ParryTimer);
        }
        else if (nextState == ParryState.Cooldown) {
            m_ParryTimer = SetParryState(m_ParryCooldownTime, ParryState.None);
            StartCoroutine(m_ParryTimer);
        }
        else if (nextState == ParryState.None)
        {
            m_Callback.OnCommandEnd(m_CommandName, this);
        }
    }

    public override void CancelDelay()
    {
        if (m_ParryTimer != null)
            StopCoroutine(m_ParryTimer);
        m_CurrentState = ParryState.None;
    }

    public bool OnHit(Damage dmg)
    {
        if (dmg.Blockable() == Damage.BlockableType.None)
            return false;

        if (m_CurrentState == ParryState.None || m_CurrentState == ParryState.Cooldown)
        {
            return false;
        }
        else if (m_CurrentState == ParryState.Perfect && dmg.Blockable() != Damage.BlockableType.OnlyBlock && !(dmg is BulletDamage))
        {
            return PerfectParry(dmg);
        }
        else if (m_CurrentState == ParryState.Ok || (m_CurrentState == ParryState.Perfect && (dmg.Blockable() == Damage.BlockableType.OnlyBlock || dmg is BulletDamage)))
        {
            return Block(dmg);
        }

        return false;
    }

    private bool Block(Damage dmg)
    {
        CameraController.Instance.Shake();

        if (m_ParryCallback != null)
            m_ParryCallback.OnBlock();

        if (!(dmg is BulletDamage))
        {
            PlayMeleeBlockEffect(dmg);

            CancelDelay();
            m_Callback.OnCommandEnd(m_CommandName, this);

            LookAtSource(dmg);
        }
        else
        {
            FancyAudioEffectsSoundPlayer.Instance.PlayBulletBlockedAudio(m_Scarlet.transform);
            
            // look up whether a bullet can be deflected or not
            if (((BulletDamage)dmg).m_Deflectable)
            {
                PlayParryDeflectableBulletEffect(dmg);
            }
            else
            {
                PlayParryNonDeflectableBulletEffect(dmg);
            }

            m_Callback.OnCommandEnd(m_CommandName, this);
        }

        PlayAudio(m_BlockAudio);
        dmg.OnBlockDamage();

        return true;
    }

    private void LookAtSource(Damage dmg)
    {
        if (dmg.m_Owner != null)
        {
            var lookPos = dmg.m_Owner.transform.position - m_Scarlet.transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            m_Scarlet.transform.rotation = rotation;
        }
    }

    private bool PerfectParry(Damage dmg)
    {
        CameraController.Instance.Shake();

        if (!(dmg is BulletDamage))
        {
            PlayMeleeParryEffect(dmg);
            CancelDelay();
            m_Callback.OnCommandEnd(m_CommandName, this);
        }

        if (m_ParryCallback != null)
            m_ParryCallback.OnPerfectParry();
        
        dmg.OnParryDamage();
        PlayAudio(m_ParryAudio);
        FancyAudioEffectsSoundPlayer.Instance.PlayParriedSound(m_Scarlet.transform);

        LookAtSource(dmg);

        return true;
    }

    private void PlayAudio(AudioClip clip)
    {
        if (m_ParryAudioPlayer != null)
        {
            m_ParryAudioPlayer.clip = clip;
            m_ParryAudioPlayer.Play();
        }
    }

    // EFFECTS

    private void PlayParryDeflectableBulletEffect(Damage dmg)
    {
        m_ImpactLightBulletDeflect.enabled = true;

        if (dmg.m_Owner != null)
        {
            m_BulletDeflectEffect.transform.localRotation = Quaternion.Euler(0,
                BossTurnCommand.CalculateAngleTowards(m_Scarlet.transform, dmg.m_Owner.transform), 0);
        }

        foreach (ParticleSystem p in m_CompsBulletDeflect)
        {
            p.Play();
        }

        StartCoroutine(HideImpactLightBulletDeflect());
        StartCoroutine(HideBulletDeflectEffect());
    }

    private void PlayParryNonDeflectableBulletEffect(Damage dmg)
    {
        m_ImpactLightBulletBlock.enabled = true;

        if (dmg.m_Owner != null)
        {
            m_BulletBlockEffect.transform.localRotation = Quaternion.Euler(0,
                BossTurnCommand.CalculateAngleTowards(m_Scarlet.transform, dmg.m_Owner.transform), 0);
        }

        foreach (ParticleSystem p in m_CompsBulletBlock)
        {
            p.Play();
        }

        StartCoroutine(HideImpactLightBulletBlock());
        StartCoroutine(HideBulletBlockEffect());
    }

    private void PlayMeleeBlockEffect(Damage dmg)
    {
        m_ImpactLightMeleeBlock.enabled = true;

        foreach (ParticleSystem p in m_CompsMeleeBlock)
        {
            p.Play();
        }

        StartCoroutine(HideImpactLightMeleeBlock());
        StartCoroutine(HideMeleeBlockEffect());
    }

    private void PlayMeleeParryEffect(Damage dmg)
    {
        m_ImpactLightMeleeParry.enabled = true;

        foreach (ParticleSystem p in m_CompsMeleeParry)
        {
            p.Play();
        }

        StartCoroutine(HideImpactLightMeleeParry());
        StartCoroutine(HideMeleeParryEffect());
    }

    private IEnumerator HideImpactLightBulletBlock() 
    {
        yield return new WaitForSeconds(0.1f);

        m_ImpactLightBulletBlock.enabled = false;
    }

    private IEnumerator HideImpactLightBulletDeflect()
    {
        yield return new WaitForSeconds(0.1f);

        m_ImpactLightBulletDeflect.enabled = false;
    }

    private IEnumerator HideBulletBlockEffect() 
    {
        yield return new WaitForSeconds(0.3f);
       
        foreach(ParticleSystem p in m_CompsBulletBlock)
        {
            p.Stop();
        }
    }

    private IEnumerator HideBulletDeflectEffect() 
    {
        yield return new WaitForSeconds(0.3f);
       
        foreach(ParticleSystem p in m_CompsBulletDeflect)
        {
            p.Stop();
        }
    }

    private IEnumerator HideImpactLightMeleeBlock()
    {
        yield return new WaitForSeconds(0.1f);

        m_ImpactLightMeleeBlock.enabled = false;
    }

    private IEnumerator HideImpactLightMeleeParry()
    {
        yield return new WaitForSeconds(0.1f);

        m_ImpactLightMeleeParry.enabled = false;
    }

    private IEnumerator HideMeleeBlockEffect()
    {
        yield return new WaitForSeconds(0.3f);

        foreach (ParticleSystem p in m_CompsMeleeBlock)
        {
            p.Stop();
        }
    }

    private IEnumerator HideMeleeParryEffect()
    {
        yield return new WaitForSeconds(0.3f);

        foreach (ParticleSystem p in m_CompsMeleeParry)
        {
            p.Stop();
        }
    }


    public interface ParryCallback
    {
        void OnPerfectParry();
        void OnBlock();
    }

}
