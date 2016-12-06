using UnityEngine;
using System.Collections;

public abstract class Attack {

    public float m_Probability;

    public AttackCallbacks m_Callbacks;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public abstract void WhileActive();

    public abstract void StartAttack();

    public virtual void CancelAttack()
    {
        m_Callbacks.OnAttackCancelled(this);
    }

    public virtual void ParryAttack()
    {
        m_Callbacks.OnAttackParried(this);
    }

    public virtual bool DoCancelOnHit(PlayerControlsCharController.AttackType attackType)
    {
        return true;
    }
}
