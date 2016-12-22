using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generally, attacks are part of a combo and consist of general commands and their own
/// specific effects / behaviour.
/// </summary>
public abstract class BossAttack : MonoBehaviour {

    public AttackCombo.Interruptability m_Interruptable;
    public AttackCombo.TriggerParry m_TriggerParry;

    public float m_TimeAfterAttack;

    public Animator m_Animator;
    public GameObject m_Boss;
    public Hittable m_BossHittable;

    public AttackCallback m_Callback;
    
	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public abstract void CancelAttack();
    public abstract void StartAttack();
    
    public interface AttackCallback
    {
        /*** notification when this attack starts. */
        void OnAttackStart(BossAttack attack);

        /*** this will only be called if everything went perfectly well & the attack is just over. */
        void OnAttackEnd(BossAttack attack);
        
        /*** e.g. by a timer: When chasing scarlet, the boss never manages to even catch her, so the attack wouldn't end as planned (with an attempted hit). */
        void OnAttackInterrupted(BossAttack attack);
    }

}
