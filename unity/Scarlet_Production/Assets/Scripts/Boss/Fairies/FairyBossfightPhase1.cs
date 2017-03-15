using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyBossfightPhase1 : FairyBossfightPhase {

    public CharacterHealth m_ArmorHealth;
    public CharacterHealth m_AEFairyHealth;
    public Collider m_AEFairyCollider;

    public Animator m_ArmorAnimator;
    public GameObject m_Sword;
    public GameObject m_Shield;

    public PlayerControls m_PlayerControls;

    protected bool m_EndInitialized = false;

    public override void StartPhase(FairyPhaseCallbacks callbacks)
    {
        m_Active = true;
        m_EndInitialized = false;
        m_Callback = callbacks;

        m_AEFairyController.Initialize(this);
        m_ArmorFairyController.Initialize(this);

        m_Callback.OnPhaseStart(this);

        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        m_ArmorAnimator.SetBool("Dead", false);
        m_PlayerControls.DisableAllCommands();

        //Vector3 prefScaleSword = new Vector3(m_Sword.transform.localScale.x, m_Sword.transform.localScale.y, m_Sword.transform.localScale.z);
        //Vector3 prefScaleShield = new Vector3(m_Shield.transform.localScale.x, m_Shield.transform.localScale.y, m_Shield.transform.localScale.z);

        //m_Shield.transform.localScale = new Vector3(0, 0, 0);
        //m_Sword.transform.localScale = new Vector3(0, 0, 0);

        m_Shield.gameObject.SetActive(true);
        m_Sword.gameObject.SetActive(true);

        MakeSwordAndShieldShiny(true);

        yield return new WaitForEndOfFrame();

        m_ArmorAnimator.SetTrigger("EquipSwordShieldTrigger");

        float t = 2f;
        float equipTime = 2f;
        Material[] ms = m_Shield.GetComponent<Renderer>().materials;
        while ((t -= Time.deltaTime) > 0)
        {
            foreach(Material m in ms)
            {
                m.SetFloat("_Cutoff", t / equipTime);
            }
            m_Sword.GetComponent<Renderer>().material.SetFloat("_Cutoff", t / equipTime);
            //m_Shield.transform.localScale = Vector3.Lerp(new Vector3(0, 0, 0), prefScaleShield, t / equipTime);
            //m_Sword.transform.localScale = Vector3.Lerp(new Vector3(0, 0, 0), prefScaleSword, t / equipTime);
            yield return null;
        }
        
        foreach (Material m in ms)
        {
            m.SetFloat("_Cutoff", 0);
        }

        //m_Shield.transform.localScale = prefScaleShield;
        //m_Sword.transform.localScale = prefScaleSword;

        yield return new WaitForSeconds(0.5f);

        MakeSwordAndShieldShiny(false);

        m_AEFairyController.ExpandLightGuard();
        m_PlayerControls.EnableAllCommands();

        yield return new WaitForSeconds(m_AEFairyController.m_LightGuard.m_ExpandLightGuardTime);

        StartCombo();
    }

    private void MakeSwordAndShieldShiny(bool v)
    {
        Renderer swordRenderer = m_Sword.GetComponent<Renderer>();
        Renderer shieldRenderer = m_Shield.GetComponent<Renderer>();
        

        DynamicGI.SetEmissive(swordRenderer, v ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 0));
        DynamicGI.SetEmissive(shieldRenderer, v ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 0));

        DynamicGI.UpdateMaterials(swordRenderer);
        DynamicGI.UpdateMaterials(shieldRenderer);
    }

    protected override void Update()
    {
        if (!m_Active)
            return;

        if (!m_EndInitialized && m_ArmorHealth.m_CurrentHealth <= 0)
            EndPhase();
    }

    protected override void EndPhase()
    {
        EndCombo();
        m_EndInitialized = true;

        m_AEFairyController.CancelAndReset();
        m_AEFairyController.StopAllCoroutines();
        m_ArmorFairyController.StopAllCoroutines();

        m_AEFairyController.m_NotDeactivated = false;
        m_ArmorFairyController.m_NotDeactivated = false;
        m_ArmorFairyController.UnRegisterEventsForSound();

        m_Active = false;
        m_ArmorHealth.m_CurrentHealth = m_ArmorHealth.m_MaxHealth;

        StartCoroutine(RegenerateThenEnd());
    }

    protected virtual IEnumerator RegenerateThenEnd()
    {
        m_AEFairyCollider.isTrigger = false;
        m_AEFairyCollider.enabled = true;

        m_PlayerControls.DisableAllCommands();
        m_AEFairyController.ExpandLightGuard();

        float t = 0;
        float reanimateTime = 2f;
        while ((t += Time.deltaTime) < reanimateTime)
        {
            m_ArmorHealth.m_CurrentHealth = t / reanimateTime * m_ArmorHealth.m_MaxHealth;
            m_AEFairyHealth.m_CurrentHealth = Mathf.Lerp(m_AEFairyHealth.m_CurrentHealth, m_AEFairyHealth.m_MaxHealth, t / reanimateTime);
            yield return null;
        }

        m_AEFairyHealth.m_CurrentHealth = m_AEFairyHealth.m_MaxHealth;
        m_ArmorHealth.m_CurrentHealth = m_ArmorHealth.m_MaxHealth;

        m_AEFairyHealth.transform.rotation = Quaternion.Euler(0, 0, 0);
        m_PlayerControls.EnableAllCommands();

        base.EndPhase();
    }

}
