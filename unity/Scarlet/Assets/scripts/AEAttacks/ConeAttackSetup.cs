using UnityEngine;
using System.Collections;
using System;

public class ConeAttackSetup : AEAttackSetup, Updateable
{
    public GameObject m_ConePrefab;

    public GameObject m_ConeInstance;
    public Vector3 m_StartPosition;

    public AEAttackSeries m_Series;

    public ConeAttackSetup(GameObject conePrefab, AEAttackSeries series)
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
        m_ConeInstance = (GameObject) GameObject.Instantiate(m_ConePrefab, m_StartPosition, Quaternion.Euler(0, 0, 0));
        m_ConeInstance.SetActive(true);
        m_ConeInstance.transform.LookAt(GameController.Instance.m_Scarlet.transform.position);
        m_ConeInstance.transform.localScale = new Vector3(4, 2, 4);

        m_Series.m_Behaviour.StartCoroutine(RemoveAfter(1.5f));

        GameController.Instance.RegisterUpdateable(this);
    }

    public void Update()
    {
        m_ConeInstance.transform.LookAt(GameController.Instance.m_Scarlet.transform.position);
    }

    public IEnumerator RemoveAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        GameObject.Destroy(m_ConeInstance);
        GameController.Instance.UnregisterUpdateable(this);
    }
}
