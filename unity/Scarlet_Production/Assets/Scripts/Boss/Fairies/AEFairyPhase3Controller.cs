using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEFairyPhase3Controller : AEFairyController {

    public Collider m_AEFairyCollider;

    public int m_NumHits = 4;
    public CharacterHealth m_AEFairyHealth;
    private int m_CurrentHits;

    private bool m_Vulnerable;
    private bool m_JustFinishedCombo;

    public override void Initialize(FairyControllerCallbacks callbacks)
    {
        m_JustFinishedCombo = false;
        base.Initialize(callbacks);

        m_AEFairyCollider.isTrigger = false;
        m_AEFairyCollider.enabled = true;

        m_CurrentHits = 0;

        ExpandLightGuard();
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        m_JustFinishedCombo = true;
        base.OnComboEnd(combo);
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        if (m_JustFinishedCombo)
        {
            m_Vulnerable = true;
            DisableLightGuard();

            yield return new WaitForSeconds(2f);

            m_Vulnerable = false;
            ExpandLightGuard();
        }

        yield return base.StartNextComboAfter(time);
    }

    public override bool OnHit(Damage dmg)
    {
        if (m_Vulnerable)
        {
            m_CurrentHits++;

            m_Vulnerable = false;
            ExpandLightGuard();
            dmg.OnSuccessfulHit();
            m_AEFairyHealth.m_CurrentHealth = (m_AEFairyHealth.m_MaxHealth) * (1f - (m_CurrentHits / (float) m_NumHits));
            return true;
        }
        else
        {
            return true;
        }
    }

}
