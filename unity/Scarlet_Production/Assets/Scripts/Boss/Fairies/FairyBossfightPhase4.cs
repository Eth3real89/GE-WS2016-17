using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyBossfightPhase4 : FairyBossfightPhase {

    public CharacterHealth m_ArmorFairyHealth;

    public Animator m_ArmorAnimator;
    public GameObject m_Armor;

    public GameObject m_Sword;
    public GameObject m_Shield;
    public GameObject m_BigSword;

    public PlayerControls m_PlayerControls;

    public override void StartPhase(FairyPhaseCallbacks callbacks)
    {
        m_Active = true;
        m_Callback = callbacks;

        m_ArmorFairyController.Initialize(this);

        m_Callback.OnPhaseStart(this);
        StartCoroutine(EquipTwoHandedSword());
    }

    private IEnumerator EquipTwoHandedSword()
    {
        m_PlayerControls.DisableAllCommands();

        Vector3 prefScaleSword = new Vector3(m_Sword.transform.localScale.x, m_Sword.transform.localScale.y, m_Sword.transform.localScale.z);
        Vector3 prefScaleShield = new Vector3(m_Shield.transform.localScale.x, m_Shield.transform.localScale.y, m_Shield.transform.localScale.z);

        m_Shield.transform.localScale = new Vector3(0, 0, 0);
        m_Sword.transform.localScale = new Vector3(0, 0, 0);

        float t = 0;
        float equipTime = 1f;
        while ((t += Time.deltaTime) < equipTime)
        {
            m_Shield.transform.localScale = Vector3.Lerp(prefScaleShield, Vector3.zero, t / equipTime);
            m_Sword.transform.localScale = Vector3.Lerp(prefScaleSword, Vector3.zero, t / equipTime);
            yield return null;
        }

        m_Shield.gameObject.SetActive(false);
        m_Sword.gameObject.SetActive(false);

        m_ArmorAnimator.SetBool("TwoHand", true);

        m_BigSword.gameObject.SetActive(true);
        Vector3 prefScaleBigSword = new Vector3(m_BigSword.transform.localScale.x, m_BigSword.transform.localScale.y, m_BigSword.transform.localScale.z);
        m_BigSword.transform.localScale = new Vector3(0, 0, 0);

        t = 0;
        equipTime = 2f;
        while ((t += Time.deltaTime) < equipTime)
        {
            m_BigSword.transform.localScale = Vector3.Lerp(Vector3.zero, prefScaleBigSword, t / equipTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        m_PlayerControls.EnableAllCommands();

        yield return new WaitForSeconds(0.5f);
        StartCombo();
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

    public override void EndCombo()
    {
        m_ArmorFairyController.CancelComboIfActive();
    }

    protected override void EndPhase()
    {
        EndCombo();

        m_ArmorFairyController.StopAllCoroutines();

        m_Active = false;
        StartCoroutine(Die());
    }

    protected virtual IEnumerator Die()
    {
        yield return new WaitForSeconds(1f);

        base.EndPhase();
    }

}
