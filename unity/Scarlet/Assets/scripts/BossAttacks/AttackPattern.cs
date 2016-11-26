using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public Material m_HitMaterial;
    public Material m_HighlightMaterial;

    public GameObject m_BloodTrailPrefab;
    public GameObject m_BloodTrailPlaneCollider;
    public float m_TimeBleedAfterHit;
    //public float m_InitialBloodEmissionRate;
    //public float m_InitialBloodSpeed;
    //public float m_EndBloodEmissionRate;
    public float m_InitialBloodShapeRadius;

    public bool m_CancelOnHit = true;

    public AudioClip m_OnBossHitAudio;
    private AudioSource m_AudioSource;

    private IEnumerator m_ColorChangeEnumerator;
    private IDictionary<Renderer, Material> m_OriginalMaterialDictionary;

    private bool m_Dead;

    /*
     * Index of m_Attacks that is currently active; only stored so as not to use the same attack twice in a row.
     */
    private int m_CurrentAttackIndex;

    // Use this for initialization
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_Attacks = new Attack[6];

        m_CurrentAttackIndex = 0;
        StartCoroutine(StartNextAttackAfter(2f));

        InitOriginalColorDictionary();
        m_Dead = false;
    }

    void SetupAttack(int index)
    {
        if (m_Dead)
            return;

        switch(index)
        {
            case 0:
                m_Attacks[index] = new ChaseAttack(this, m_HighlightMaterial);
                break;
            case 1:
                m_Attacks[index] = new ConeAttackSeries(this, m_ConeSetupPrefab, m_ConeAttackPrefab);
                break;
            case 2:
                m_Attacks[index] = new PizzaAttackSeries(this, m_PizzaSetupPrefab, m_PizzaAttackPrefab);
                break;
            case 3:
                m_Attacks[index] = new TargetAttackSeries(this, m_TargetSetupPrefab, m_TargetAttackPrefab);
                break;
            case 4:
                m_Attacks[index] = new BeamAttackSeries(this, m_BeamPrefab, m_Boss);
                break;
            case 5:
                m_Attacks[index] = new DoubleBeamAttackSeries(this, m_BeamPrefab, m_Boss);
                break;
        }

        m_Attacks[index].m_Callbacks = this;
    }

    private IEnumerator StartNextAttackAfter(float time)
    {
        yield return new WaitForSeconds(time);

        if (!m_Dead)
        {
            SetupAttack(m_CurrentAttackIndex);

            m_Attacks[m_CurrentAttackIndex].StartAttack();
            m_CurrentAttack = m_Attacks[m_CurrentAttackIndex];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Dead)
            return;

        if (m_CurrentAttack != null)
        {
            m_CurrentAttack.WhileActive();
        }
    }

    public void OnBossHit()
    {
        if (m_CancelOnHit && m_CurrentAttack != null && m_CurrentAttackIndex != 0) // = any AE attack
        {
            m_Boss.GetComponentInChildren<Animator>().SetTrigger("StaggerTrigger");
            m_CurrentAttack.CancelAttack();
        }

        PlayOnHitSound();
        InitBloodTrail();
        ColorBossRed();
    }

    void AttackCallbacks.OnAttackStart(Attack a)
    {
        m_CurrentAttack = a;
    }

    void AttackCallbacks.OnAttackEnd(Attack a)
    {
        m_CurrentAttack = null;
        
        if (m_CurrentAttackIndex == 0)
        {
            m_CurrentAttackIndex = (int) Random.Range(1, m_Attacks.Length);
        }
        else
        {
            m_CurrentAttackIndex = 0;
        }

        //m_CurrentAttackIndex = 4;

        StartCoroutine(StartNextAttackAfter(2f));
    }

    void AttackCallbacks.OnAttackParried(Attack a)
    {
        m_Boss.GetComponentInChildren<Animator>().SetTrigger("IdleTrigger");
    }

    void AttackCallbacks.OnAttackCancelled(Attack a)
    {
        if (m_CurrentAttack != null)
        {
            m_CurrentAttack = null;
            StartCoroutine(StaggerThenContinueAttacking(1.5f, a));
        }
    }

    private IEnumerator StaggerThenContinueAttacking(float seconds, Attack a)
    {
        yield return new WaitForSeconds(seconds);

        (this as AttackCallbacks).OnAttackEnd(a);
    }

    private void InitOriginalColorDictionary()
    {
        m_OriginalMaterialDictionary = new System.Collections.Generic.Dictionary<Renderer, Material>();
        foreach (Renderer r in m_Boss.GetComponentsInChildren<Renderer>())
        {
            m_OriginalMaterialDictionary.Add(r, r.material);
        }
    }

    private void ColorBossRed()
    {
        foreach (Renderer r in m_Boss.GetComponentsInChildren<Renderer>())
        {
            r.material = m_HitMaterial;
        }

        if (m_ColorChangeEnumerator != null)
        {
            StopCoroutine(m_ColorChangeEnumerator);
        }
        m_ColorChangeEnumerator = RestoreColors();
        StartCoroutine(m_ColorChangeEnumerator);
    }

    public void HighlightBoss()
    {
        foreach(Renderer r in m_Boss.GetComponentsInChildren<Renderer>())
        {
            r.material = m_HighlightMaterial;
        }
    }

    public IEnumerator RestoreColors()
    {
        yield return new WaitForSeconds(0.3f);

        foreach (Renderer r in m_Boss.GetComponentsInChildren<Renderer>())
        {
            r.material = m_OriginalMaterialDictionary[r];
        }
    } 

    private void PlayOnHitSound()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.clip = m_OnBossHitAudio;
            m_AudioSource.Play();
        }
    }

    private void InitBloodTrail()
    {
        GameObject bloodTrailWrapper = (GameObject) Instantiate(m_BloodTrailPrefab, m_Boss.transform.position,
            GameController.Instance.m_Scarlet.transform.rotation);
        Transform bloodTrail = bloodTrailWrapper.transform.FindChild("BloodTrail");

        ParticleSystem ps = bloodTrail.GetComponent<ParticleSystem>();
        //ps.startSpeed = m_InitialBloodSpeed;

        var emission = ps.emission;
       // emission.rate = m_InitialBloodEmissionRate;

        var shape = ps.shape;
        shape.radius = m_InitialBloodShapeRadius;

        ps.collision.SetPlane(0, m_BloodTrailPlaneCollider.transform);

        StartCoroutine(RemoveBloodTrail(bloodTrailWrapper, ps));
    }

    private IEnumerator RemoveBloodTrail(GameObject bloodTrail, ParticleSystem ps)
    {
        float time = 0;
        float endTime = time + m_TimeBleedAfterHit;

        var emission = ps.emission;
        var shape = ps.shape;
        
        while (time < endTime)
        {
            bloodTrail.transform.position = m_Boss.transform.position;

            float level = time / endTime;
            //ps.startSpeed = Mathf.Lerp(m_InitialBloodSpeed, 0, level);
            //emission.rate = Mathf.Lerp(m_InitialBloodEmissionRate, m_EndBloodEmissionRate, level);
            shape.radius = Mathf.Lerp(m_InitialBloodShapeRadius, 0.05f, level);      

            time += Time.deltaTime;
            yield return null;
        }
        GameObject.Destroy(bloodTrail);
    }

    public void Die()
    {
        m_Dead = true;

        if (m_CurrentAttack != null)
        {
            if (m_CurrentAttack is ChaseAttack)
            {
                (m_CurrentAttack as ChaseAttack).m_Ended = true;
            }
            m_CurrentAttack.CancelAttack();
        }
        m_Boss.GetComponentInChildren<Animator>().SetTrigger("DeathTrigger");
    }
}
