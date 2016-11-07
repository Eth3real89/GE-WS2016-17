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
        m_Series = new AEAttackSeries[5];

        PizzaAttackSeries p = new PizzaAttackSeries(this, m_PizzaSetupPrefab, m_PizzaAttackPrefab);
        m_Series[0] = p;

        BeamAttackSeries b = new BeamAttackSeries(this, m_BeamPrefab, m_Boss);
        m_Series[1] = b;

        DoubleBeamAttackSeries b2 = new DoubleBeamAttackSeries(this, m_BeamPrefab, m_Boss);
        m_Series[2] = b2;

        ConeAttackSeries c = new ConeAttackSeries(this, m_ConeSetupPrefab, m_ConeAttackPrefab);
        m_Series[3] = c;

        TargetAttackSeries t = new TargetAttackSeries(this, m_TargetSetupPrefab, m_TargetAttackPrefab);
        m_Series[4] = t;
        

        for (int i = 0; i < 10; i++)
        {
           StartCoroutine(StartAttacks(i * 41));
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private IEnumerator StartAttacks(float time)
    {
        yield return new WaitForSeconds(time);

        StartCoroutine(InitAttackAfter(4, 2f));
        StartCoroutine(InitAttackAfter(3, 9f));
        StartCoroutine(InitAttackAfter(1, 13f));
        StartCoroutine(InitAttackAfter(0, 24f));
        StartCoroutine(InitAttackAfter(2, 30f));
    }

    private IEnumerator InitAttackAfter(int which, float time)
    {
        yield return new WaitForSeconds(time);

        m_Series[which].RunSeries(transform);
    }
}
