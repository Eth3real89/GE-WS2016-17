using UnityEngine;
using System.Collections;
using System;

public class ConeAttack : AEAttack {

    public GameObject m_ConePrefab;

    public GameObject m_ConeInstance;
    public Vector3 m_StartPosition;

    public AEAttackSeries m_Series;

    public ConeAttackSetup m_Setup;

    private IEnumerator m_DestroyEnumerator;

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
        m_ConeInstance.transform.localScale = new Vector3(10, 1, 10);
        

        Light l = m_ConeInstance.GetComponentInChildren<Light>();
        if (l != null)
        {
            l.intensity = 20f;
            l.range = 40f;
            l.spotAngle = 170f;
        }

        m_DestroyEnumerator = RemoveAfter(1.5f);

        m_Series.m_Behaviour.StartCoroutine(m_DestroyEnumerator);
    }

    public IEnumerator RemoveAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(m_ConeInstance);
    }

    public void Destroy()
    {
        if (m_DestroyEnumerator != null)
            m_Series.m_Behaviour.StopCoroutine(m_DestroyEnumerator);
        GameObject.Destroy(m_ConeInstance);
    }
}
