using UnityEngine;
using System.Collections;

public class ConeAttack : AEAttack {

    public GameObject m_ConePrefab;

    public GameObject m_ConeInstance;
    public Vector3 m_StartPosition;
    public Quaternion m_StartRotation;

    public AEAttackSeries m_Series;

    public ConeAttack(GameObject conePrefab, AEAttackSeries series)
    {
        this.m_ConePrefab = conePrefab;
        this.m_Series = series;
    }

    public override GameObject GetObject()
    {
        return m_ConeInstance;
    }

    public override void LaunchPart()
    {
        // later on: play some animation, etc. 
        m_ConeInstance = (GameObject)GameObject.Instantiate(m_ConePrefab, m_StartPosition, Quaternion.Euler(0, 0, 0));
        m_ConeInstance.SetActive(true);
        m_ConeInstance.transform.LookAt(GameController.Instance.m_Scarlet.transform.position);
        m_ConeInstance.transform.localScale = new Vector3(4, 2, 4);

        m_Series.m_Behaviour.StartCoroutine(RemoveAfter(2.5f));
    }

    public IEnumerator RemoveAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(m_ConeInstance);
    }
}
