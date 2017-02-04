using UnityEngine;
using System.Collections;
using System;

public class TargetAttack : AEAttack
{
    public GameObject m_TargetPrefab;

    public GameObject m_TargetInstance;

    public AEAttackSeries m_Series;

    public TargetAttackSetup m_Setup;

    private IEnumerator m_DestroyEnumerator;

    public TargetAttack(GameObject targetPrefab, AEAttackSeries series, TargetAttackSetup setup)
    {
        this.m_TargetPrefab = targetPrefab;
        this.m_Series = series;
        this.m_Setup = setup;
    }

    public override GameObject GetObject()
    {
        return m_TargetInstance;
    }

    public override void LaunchPart()
    {
        // later on: play some animation, etc. 
        m_TargetInstance = (GameObject)GameObject.Instantiate(m_TargetPrefab,
            m_Setup.m_TargetInstance.transform.position, Quaternion.Euler(0f, 0f, 0f));
        m_TargetInstance.SetActive(true);

        m_DestroyEnumerator = RemoveAfter(0.3f);
        m_Series.m_Behaviour.StartCoroutine(m_DestroyEnumerator);
    }

    public IEnumerator RemoveAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(m_TargetInstance);
    }

    public void Destroy()
    {
        if (m_DestroyEnumerator != null)
            m_Series.m_Behaviour.StopCoroutine(m_DestroyEnumerator);
        GameObject.Destroy(m_TargetInstance);
    }
}
