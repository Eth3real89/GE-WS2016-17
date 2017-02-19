using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyBossfightPhase3 : FairyBossfightPhase {

    public CharacterHealth m_AEFairyHealth;
    public CharacterHealth m_ArmorFairyHealth;

    public Animator m_ArmorAnimator;
    public GameObject m_AEFairy;
    public GameObject m_Armor;

    public PlayerControls m_PlayerControls;

    public override void StartPhase(FairyPhaseCallbacks callbacks)
    {
        m_Active = true;
        m_Callback = callbacks;

        m_AEFairyController.Initialize(this);

        m_Callback.OnPhaseStart(this);

        StartCombo();
    }

    public override void StartCombo()
    {
        m_AEFairyController.StartCombo(0);
    }

    protected override void Update()
    {
        if (!m_Active)
            return;

        if (m_AEFairyHealth.m_CurrentHealth <= 0)
            EndPhase();
    }

    public override void EndCombo()
    {
        m_AEFairyController.CancelComboIfActive();
    }

    protected override void EndPhase()
    {
        EndCombo();

        m_AEFairyController.StopAllCoroutines();

        m_Active = false;
        StartCoroutine(ReanimateArmor());
    }

    protected virtual IEnumerator ReanimateArmor()
    {
        m_PlayerControls.DisableAllCommands();

        float timeToReachArmor = 2.5f;
        float t = 0;
        while((t += Time.deltaTime) < timeToReachArmor)
        {
            m_AEFairy.transform.position = Vector3.Lerp(m_AEFairy.transform.position, m_Armor.transform.position, t / timeToReachArmor);
            yield return null;
        }

        m_ArmorAnimator.SetBool("Dead", false);
        m_ArmorAnimator.SetTrigger("ReanimationTrigger");

        float reanimateTime = 2f;
        t = 0;
        while ((t += Time.deltaTime) < reanimateTime)
        {
            m_AEFairy.transform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), Vector3.zero, t / reanimateTime);
            m_ArmorFairyHealth.m_CurrentHealth = t / reanimateTime * m_ArmorFairyHealth.m_MaxHealth;
            yield return null;
        }

        m_ArmorFairyHealth.m_CurrentHealth = m_ArmorFairyHealth.m_MaxHealth;

        m_AEFairy.gameObject.SetActive(false);

        m_PlayerControls.EnableAllCommands();

        base.EndPhase();
    }
}
