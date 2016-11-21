using UnityEngine;
using System.Collections;
using System;

/**
 * 
 */
public class ChaseAttack : Attack {

    private enum ChaseAttackState {Chase, Aim, Damage};

    public static float m_FollowSpeed = 2.5f;
    public static float m_MaxFollowTime = 10f;

    private ChaseAttackState m_State;

    private MonoBehaviour m_Boss;

    private Animator m_Animator;
    private ArrayList m_LastPlayerPositions;
    private BossDamageTrigger m_BossDamageTrigger;

    private TriggerCallback m_ChaseCallback;
    private TriggerCallback m_HitCallback;

    private IEnumerator m_DamageEnumerator;
    private IEnumerator m_ResetEnumerator;

    private bool m_Ended;

    private int m_HitCount;

    public ChaseAttack(MonoBehaviour boss) : base()
    {
        m_Boss = boss;
        m_Animator = m_Boss.GetComponentInChildren<Animator>();
        m_BossDamageTrigger = m_Boss.GetComponentInChildren<BossDamageTrigger>();

        m_LastPlayerPositions = new ArrayList();

        SetupTriggerCallbacks();
    }

    public override void StartAttack()
    {
        m_State = ChaseAttackState.Chase;
        m_Ended = false;

        this.m_Callbacks.OnAttackStart(this);
    }

    public override void WhileActive()
    {
        if (m_State == ChaseAttackState.Chase)
        {
            Chase();
        }
        else if (m_State == ChaseAttackState.Aim)
        {
            Aim();
        }
        else if (m_State == ChaseAttackState.Damage)
        {
            Damage();
        }
    }

    private void Chase()
    {
        GameObject obj = GameController.Instance.m_Scarlet;

        Vector3 avgScarletPos = new Vector3();

        // put that in a separate "BossMove" script?

        m_LastPlayerPositions.Add(obj.transform.position);
        if (m_LastPlayerPositions.Count > 5) m_LastPlayerPositions.RemoveAt(0);
        foreach (Vector3 vec in m_LastPlayerPositions)
        {
            avgScarletPos += vec;
        }
        avgScarletPos /= m_LastPlayerPositions.Count;


        float speed = m_FollowSpeed;        

        m_Boss.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(m_Boss.transform.position, avgScarletPos, speed * Time.deltaTime));

        m_Animator.SetFloat("Speed", speed);
    }

    private void InitAim()
    {
        if (m_Ended)
            return;
        m_HitCount++;

        m_State = ChaseAttackState.Aim;
        m_Animator.SetFloat("Speed", 0f);
        m_Animator.SetTrigger("AttackTrigger");

        m_DamageEnumerator = InitDamage();

        m_Boss.StartCoroutine(m_DamageEnumerator);
    }

    private IEnumerator InitAimAfter(float time)
    {
        yield return new WaitForSeconds(time);
        SetupTriggerCallbacks();
        InitAim();
    }

    private void Damage()
    {
        // Nothing really happens in this state, the Trigger takes care of that.
        // We could check for staggers etc., but even that is better in triggers / with events.
        // Same goes for Aim, probably.
    }

    private void Aim()
    {

    }

    private IEnumerator InitDamage()
    {
        yield return new WaitForSeconds(0.3f);

        m_State = ChaseAttackState.Damage;
        m_BossDamageTrigger.m_Callback = m_HitCallback;

        m_DamageEnumerator = StopDamage();
        m_Boss.StartCoroutine(m_DamageEnumerator);
    }

    private IEnumerator StopDamage()
    {
        yield return new WaitForSeconds(0.2f);

        m_BossDamageTrigger.m_Callback = null;
        m_DamageEnumerator = Reset();

        m_Boss.StartCoroutine(m_DamageEnumerator);
    }

    private void OnHit()
    {
        
        if (m_DamageEnumerator != null)
        {
            m_Boss.StopCoroutine(m_DamageEnumerator);
        }

        m_BossDamageTrigger.m_Callback = null;

        if (m_HitCount >= 3)
        {
            m_DamageEnumerator = EndAttackAfter(2f);
            m_Boss.StartCoroutine(m_DamageEnumerator);
        }
        else
        {
            m_DamageEnumerator = InitAimAfter(0.4f);
            m_Boss.StartCoroutine(m_DamageEnumerator);
        }

        GameController.Instance.HitScarlet(GameController.Instance.m_Boss, 30f, true);
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(2f);

        if (m_HitCount >= 3)
        {
            EndAttack();
        }
        else
        {
            SetupTriggerCallbacks();
            m_State = ChaseAttackState.Chase;
        }
    }

    public void EndAttack()
    {
        if (m_Ended)
            return;

        this.m_Ended = true;
        this.m_Callbacks.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        base.CancelAttack();

        m_BossDamageTrigger.m_Callback = null;
        m_Boss.StopCoroutine(m_DamageEnumerator);

        if (m_ResetEnumerator != null)
            m_Boss.StopCoroutine(m_ResetEnumerator);

        m_Boss.StartCoroutine(EndAttackAfter(2f));
        Animator animator = m_Boss.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("StaggerTrigger");
        }
    }

    private IEnumerator EndAttackAfter(float time)
    {
        yield return new WaitForSeconds(time);

        EndAttack();
    }

    public override void ParryAttack()
    {
        base.ParryAttack();
    }

    private void SetupTriggerCallbacks()
    {
        m_ChaseCallback = new ChaseTriggerCallback(this);
        m_HitCallback = new DamageTriggerCallback(this);

        m_BossDamageTrigger.m_Callback = m_ChaseCallback;
    }

    private class ChaseTriggerCallback : TriggerCallback
    {
        private ChaseAttack m_Attack;
        private bool m_AimInitialized;

        public ChaseTriggerCallback(ChaseAttack attack)
        {
            this.m_Attack = attack;
            this.m_AimInitialized = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            CheckTrigger(other);
        }

        // won't matter / do nothing (once boss is committed to attacking, it will attack!)
        public void OnTriggerLeave(Collider other)
        {
        }
        
        public void OnTriggerStay(Collider other)
        {
            CheckTrigger(other);
        }

        private void CheckTrigger(Collider other)
        {
            if (!m_AimInitialized)
            {
                if (GameController.Instance.IsScarlet(other.GetComponent<Rigidbody>()))
                {
                    m_AimInitialized = true;
                    m_Attack.InitAim();
                }
            }
        }
    }

    private class DamageTriggerCallback : TriggerCallback
    {
        private ChaseAttack m_Attack;

        private bool m_HasHit;

        public DamageTriggerCallback(ChaseAttack attack)
        {
            this.m_Attack = attack;
            this.m_HasHit = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            CheckHit(other);
        }

        public void OnTriggerLeave(Collider other)
        {
        }

        public void OnTriggerStay(Collider other)
        {
            CheckHit(other);
        }

        private void CheckHit(Collider other)
        {
            if (!m_HasHit)
            {
                if (GameController.Instance.IsScarlet(other.GetComponent<Rigidbody>()))
                {
                    m_HasHit = true;
                    m_Attack.OnHit();
                }
            }
        }
    }
}
