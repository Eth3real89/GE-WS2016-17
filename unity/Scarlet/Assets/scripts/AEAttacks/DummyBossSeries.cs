using UnityEngine;
using System.Collections;

public class DummyBossSeries : MonoBehaviour {

    public AEAttackSeries[] m_Series;

    public GameObject m_PizzaSetupPrefab;
    public GameObject m_PizzaAttackPrefab;

    public GameObject m_BeamPrefab;
    public GameObject m_Boss;

	// Use this for initialization
	void Start () {
        m_Series = new AEAttackSeries[2];

        PizzaAttackSeries p = new PizzaAttackSeries(this, m_PizzaSetupPrefab, m_PizzaAttackPrefab);
        m_Series[0] = p;


        BeamAttackSeries b = new BeamAttackSeries(this, m_BeamPrefab, m_Boss);
        m_Series[1] = b;

        StartCoroutine(InitBeamAttack());
        StartCoroutine(InitPizzaAttack());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private IEnumerator InitPizzaAttack ()
    {
        yield return new WaitForSeconds(15f);

        m_Series[0].RunSeries(transform);
    }

    private IEnumerator InitBeamAttack()
    {
        yield return new WaitForSeconds(3f);

        m_Series[1].RunSeries(transform);
    }
}
