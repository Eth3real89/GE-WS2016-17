using UnityEngine;
using System.Collections;

public class DummyBossSeries : MonoBehaviour {

    public AEAttackSeries[] m_Series;

    public GameObject m_PizzaSetupPrefab;
    public GameObject m_PizzaAttackPrefab;

    public GameObject m_BeamPrefab;
    public GameObject m_Boss;

    public GameObject m_ConeSetupPrefab;
    public GameObject m_ConeAttackPrefab;

    public GameObject m_TargetSetupPrefab;
    public GameObject m_TargetAttackPrefab;

    // Use this for initialization
    void Start () {
        m_Series = new AEAttackSeries[4];

        PizzaAttackSeries p = new PizzaAttackSeries(this, m_PizzaSetupPrefab, m_PizzaAttackPrefab);
        m_Series[0] = p;

        BeamAttackSeries b = new BeamAttackSeries(this, m_BeamPrefab, m_Boss);
        m_Series[1] = b;

        ConeAttackSeries c = new ConeAttackSeries(this, m_ConeSetupPrefab, m_ConeAttackPrefab);
        m_Series[2] = c;

        TargetAttackSeries t = new TargetAttackSeries(this, m_TargetSetupPrefab, m_TargetAttackPrefab);
        m_Series[3] = t;

        StartCoroutine(InitTargetAttack());
        StartCoroutine(InitConeAttack());
        StartCoroutine(InitBeamAttack());
        StartCoroutine(InitPizzaAttack());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private IEnumerator InitPizzaAttack ()
    {
        yield return new WaitForSeconds(30f);

        m_Series[0].RunSeries(transform);
    }

    private IEnumerator InitBeamAttack()
    {
        yield return new WaitForSeconds(15f);

        m_Series[1].RunSeries(transform);
    }

    private IEnumerator InitConeAttack()
    {
        yield return new WaitForSeconds(9f);

        m_Series[2].RunSeries(transform);
    }

    private IEnumerator InitTargetAttack()
    {
        yield return new WaitForSeconds(2f);

        m_Series[3].RunSeries(transform);
    }
}
