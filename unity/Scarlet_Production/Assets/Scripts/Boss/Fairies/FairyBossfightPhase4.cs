﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyBossfightPhase4 : FairyBossfightPhase {

    public CharacterHealth m_ArmorFairyHealth;

    public RectTransform m_ArmorHealthBar;
    public RectTransform m_AEFairyHealthBar;

    public Animator m_ArmorAnimator;
    public GameObject m_Armor;
    public GameObject m_Scarlet;

    public GameObject m_Sword;
    public GameObject m_Shield;
    public GameObject m_BigSword;

    public CombatCamera[] m_Cameras;

    public PlayerControls m_PlayerControls;

    protected float m_InitialArmorBarYPos = -1;

    public override void StartPhase(FairyPhaseCallbacks callbacks)
    {
        m_Active = true;
        m_Callback = callbacks;

        m_ArmorFairyHealth.m_CurrentHealth = m_ArmorFairyHealth.m_MaxHealth / 2;
        m_ArmorFairyController.Initialize(this);

        m_AEFairyHealthBar.gameObject.SetActive(false);

        m_Callback.OnPhaseStart(this);
        StartCoroutine(EquipTwoHandedSword());

        foreach (CombatCamera c in m_Cameras)
        {
            c.m_Targets = new GameObject[] {m_Scarlet, m_Armor};
        }
    }

    private IEnumerator EquipTwoHandedSword()
    {
        m_PlayerControls.DisableAllCommands();
        m_PlayerControls.StopMoving();

        //Vector3 prefScaleSword = new Vector3(m_Sword.transform.localScale.x, m_Sword.transform.localScale.y, m_Sword.transform.localScale.z);
        //Vector3 prefScaleShield = new Vector3(m_Shield.transform.localScale.x, m_Shield.transform.localScale.y, m_Shield.transform.localScale.z);

        //m_Shield.transform.localScale = new Vector3(0, 0, 0);
        //m_Sword.transform.localScale = new Vector3(0, 0, 0);
        float t = 0;
        float equipTime = 1f;
        Material[] ms = m_Shield.GetComponent<Renderer>().materials;


        m_InitialArmorBarYPos = m_ArmorHealthBar.localPosition.y;
        while ((t += Time.deltaTime) < equipTime)
        {
            foreach (Material m in ms)
            {
                m.SetFloat("_Cutoff", t / equipTime);
            }
            m_Sword.GetComponent<Renderer>().material.SetFloat("_Cutoff", t / equipTime);

            //m_Shield.transform.localScale = Vector3.Lerp(prefScaleShield, Vector3.zero, t / equipTime);
            //m_Sword.transform.localScale = Vector3.Lerp(prefScaleSword, Vector3.zero, t / equipTime);

            Vector3 hbPos = m_ArmorHealthBar.localPosition;
            m_ArmorHealthBar.localPosition = Vector3.Lerp(new Vector3(hbPos.x, hbPos.y, hbPos.z), new Vector3(hbPos.x, 25, hbPos.z), t / equipTime);
            yield return null;
        }
        foreach (Material m in ms)
        {
            m.SetFloat("_Cutoff", 1);
        }
        m_Sword.GetComponent<Renderer>().material.SetFloat("_Cutoff", 1);


        m_Shield.gameObject.SetActive(false);
        m_Sword.gameObject.SetActive(false);

        m_ArmorAnimator.SetBool("TwoHand", true);

        m_BigSword.gameObject.SetActive(true);
        //Vector3 prefScaleBigSword = new Vector3(m_BigSword.transform.localScale.x, m_BigSword.transform.localScale.y, m_BigSword.transform.localScale.z);
        //m_BigSword.transform.localScale = new Vector3(0, 0, 0);

        MakeSwordShiny(true);

        FancyAudioEffectsSoundPlayer.Instance.PlayWeaponSpawnSound(m_BigSword.transform);
        t = 2f;
        equipTime = 2f;
        while ((t -= Time.deltaTime) > 0)
        {
            m_BigSword.GetComponent<Renderer>().material.SetFloat("_Cutoff", t / equipTime);
            //m_BigSword.transform.localScale = Vector3.Lerp(Vector3.zero, prefScaleBigSword, t / equipTime);
            yield return null;
        }
        m_BigSword.GetComponent<Renderer>().material.SetFloat("_Cutoff", 0);

        yield return new WaitForSeconds(0.5f);

        MakeSwordShiny(false);

        m_PlayerControls.EnableAllCommands();

        yield return new WaitForSeconds(0.5f);
        StartCombo();
    }

    private void MakeSwordShiny(bool v)
    {
        Renderer swordRenderer = m_BigSword.GetComponent<Renderer>();
        swordRenderer.material.SetColor("_EmissionColor", v ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 0));
        DynamicGI.UpdateMaterials(swordRenderer);
    }

    public override void StartCombo()
    {
        m_ArmorFairyController.StartCombo(0);
    }

    protected override void Update()
    {
        if (!m_Active)
            return;

        if (m_ArmorFairyHealth.m_CurrentHealth <= 0)
            EndPhase();
    }

    public void ResetHealthBars()
    {
        m_AEFairyHealthBar.gameObject.SetActive(true);

        if (m_InitialArmorBarYPos != -1)
            m_ArmorHealthBar.localPosition = new Vector3(m_ArmorHealthBar.localPosition.x, m_InitialArmorBarYPos, m_ArmorHealthBar.localPosition.z);
    }

    public override void EndCombo()
    {
        m_ArmorFairyController.CancelComboIfActive();
    }

    protected override void EndPhase()
    {
        EndCombo();

        m_ArmorFairyController.StopAllCoroutines();
        m_ArmorFairyController.m_NotDeactivated = false;

        m_Active = false;
        StartCoroutine(Die());
    }

    protected virtual IEnumerator Die()
    {
        m_ArmorFairyController.ForceCancelHitBehaviours();
        m_ArmorAnimator.SetBool("Dead", true);
        m_ArmorAnimator.SetTrigger("DeathTriggerBack");
        m_ArmorFairyController.m_BossHittable.RegisterInterject(null);

        m_Callback.SetPhaseIndicatorsEnabled(0);
        yield return new WaitForSeconds(1f);

        base.EndPhase();
    }

}
