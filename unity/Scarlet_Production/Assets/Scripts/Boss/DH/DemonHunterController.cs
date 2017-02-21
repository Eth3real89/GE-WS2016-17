using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DemonHunterController : BossController {

    public enum AttackType {Pistols, Rifle, Grenade };

    public BossfightCallbacks m_Callback;
    public float m_ReloadPistolTime;
    public float m_ReloadRifleTime;

    protected bool m_Reloading;

    public AttackType[] m_Types;

    public void StartPhase(BossfightCallbacks callback)
    {
        this.m_Callback = callback;
        m_Reloading = false;
    }

    protected virtual void RegisterListeners()
    {

    }

    protected virtual void UnregisterListeners()
    {

    }

    protected abstract IEnumerator PrepareAttack(int attackIndex);

    protected virtual IEnumerator ReloadPistols(IEnumerator then)
    {
        m_Reloading = true;
        yield return new WaitForSeconds(m_ReloadPistolTime);
        m_Reloading = false;

        m_NextComboTimer = then;
        StartCoroutine(m_NextComboTimer);
    }

    protected virtual IEnumerator ReloadRifle(IEnumerator then)
    {
        m_Reloading = true;
        yield return new WaitForSeconds(m_ReloadRifleTime);
        m_Reloading = false;

        m_NextComboTimer = then;
        StartCoroutine(m_NextComboTimer);
    }

    public override bool OnHit(Damage dmg)
    {
        if (m_Reloading)
        {
            dmg.OnSuccessfulHit();
            return false;
        }
        else
        {
            return true;
        }
    }

}
