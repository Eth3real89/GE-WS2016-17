using UnityEngine;
using System.Collections;

public class DummyBossSeries : MonoBehaviour {

    public AEAttackSeries[] m_Series;

    public GameObject m_PizzaSetupPrefab;
    public GameObject m_PizzaAttackPrefab;

	// Use this for initialization
	void Start () {
        m_Series = new AEAttackSeries[1];

        PizzaAttackSeries p = new PizzaAttackSeries(this, m_PizzaSetupPrefab, m_PizzaAttackPrefab);
        m_Series[0] = p;

        StartCoroutine(InitPizzaAttack());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private IEnumerator InitPizzaAttack ()
    {
        yield return new WaitForSeconds(3f);

        m_Series[0].RunSeries(transform);
    }
}
