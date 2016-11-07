using UnityEngine;
using System.Collections;

public class ConeAttack : AEAttack {

    public GameObject m_ConePrefab;

    public GameObject m_ConeInstance;
    public Vector3 m_StartPosition;

    public AEAttackSeries m_Series;

    public ConeAttackSetup m_Setup;

    public ConeAttack(GameObject conePrefab, AEAttackSeries series, ConeAttackSetup setup)
    {
        this.m_ConePrefab = conePrefab;
        this.m_Series = series;

        this.m_Setup = setup;
    }

    public override GameObject GetObject()
    {
        return m_ConeInstance;
    }

    public override void LaunchPart()
    {
        // later on: play some animation, etc. 
        m_ConeInstance = (GameObject)GameObject.Instantiate(m_ConePrefab, m_StartPosition, m_Setup.m_LookAngles);
        m_ConeInstance.SetActive(true);
        m_ConeInstance.transform.localScale = new Vector3(4, 1, 4);

        m_Series.m_Behaviour.StartCoroutine(RemoveAfter(0.3f));
    }

    public IEnumerator RemoveAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(m_ConeInstance);
    }
}
