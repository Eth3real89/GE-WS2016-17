using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEFairyPhase3Controller : AEFairyController {

    public Collider m_AEFairyCollider;

    public int m_NumHits = 4;
    public CharacterHealth m_AEFairyHealth;

    public float m_AttackWindow = 3f;

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
        m_Vulnerable = false;

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

            while(m_AEFairyHealth.m_CurrentHealth == m_AEFairyHealth.m_HealthOld)
            {
                yield return null;
            }

            m_Vulnerable = false;
            ExpandLightGuard();

            yield return new WaitForSeconds(1f);
        }

        yield return base.StartNextComboAfter(time);
    }

    public override bool OnHit(Damage dmg)
    {
        if (m_Vulnerable)
        {
            CameraController.Instance.Shake();
            m_CurrentHits++;

            m_Vulnerable = false;
            ExpandLightGuard();
            dmg.OnSuccessfulHit();
            m_AEFairyHealth.m_CurrentHealth =  (m_AEFairyHealth.m_MaxHealth * 0.5f) + 0.5f * (m_AEFairyHealth.m_MaxHealth) * (1f - (m_CurrentHits / (float) m_NumHits));
            return true;
        }
        else
        {
            return true;
        }
    }

}
