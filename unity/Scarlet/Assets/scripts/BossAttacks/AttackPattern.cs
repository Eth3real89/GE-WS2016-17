using UnityEngine;
using System.Collections;
using System;

public class AttackPattern : MonoBehaviour, AttackCallbacks
{

    public Attack[] m_Attacks;

    public Attack m_CurrentAttack;

    public GameObject m_PizzaSetupPrefab;
    public GameObject m_PizzaAttackPrefab;

    public GameObject m_BeamPrefab;
    public GameObject m_Boss;

    public GameObject m_ConeSetupPrefab;
    public GameObject m_ConeAttackPrefab;

    public GameObject m_TargetSetupPrefab;
    public GameObject m_TargetAttackPrefab;
    /*
     * Index of m_Attacks that is currently active; only stored so as not to use the same attack twice in a row.
     */
    private int m_CurrentAttackIndex;

    // Use this for initialization
    void Start()
    {
        SetupAttacks();
        m_CurrentAttackIndex = 0;

        StartCoroutine(StartNextAttackAfter(2f));
    }

    void SetupAttacks()
    {
        m_Attacks = new Attack[6];
        m_Attacks[0] = new ChaseAttack(this);
        m_Attacks[1] = new ConeAttackSeries(this, m_ConeSetupPrefab, m_ConeAttackPrefab);
        m_Attacks[2] = new PizzaAttackSeries(this, m_PizzaSetupPrefab, m_PizzaAttackPrefab);
        m_Attacks[3] = new TargetAttackSeries(this, m_TargetSetupPrefab, m_TargetAttackPrefab);
        m_Attacks[4] = new BeamAttackSeries(this, m_BeamPrefab, m_Boss);
        m_Attacks[5] = new DoubleBeamAttackSeries(this, m_BeamPrefab, m_Boss);

        foreach(Attack a in m_Attacks)
        {
            a.m_Callbacks = this;
        }
    }

    private IEnumerator StartNextAttackAfter(float time)
    {
        yield return new WaitForSeconds(time);

        m_Attacks[m_CurrentAttackIndex].StartAttack();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CurrentAttack != null)
        {
            m_CurrentAttack.WhileActive();
        }
    }

    void AttackCallbacks.OnAttackStart(Attack a)
    {
        m_CurrentAttack = a;
    }

    void AttackCallbacks.OnAttackEnd(Attack a)
    {
        Debug.Log("On attack end!");

        m_CurrentAttackIndex += 1;
        if (m_CurrentAttackIndex >= m_Attacks.Length)
            m_CurrentAttackIndex = 0;
        
        StartCoroutine(StartNextAttackAfter(2f));
    }

    void AttackCallbacks.OnAttackParried(Attack a)
    {
    }

    void AttackCallbacks.OnAttackCancelled(Attack a)
    {
    }
}
