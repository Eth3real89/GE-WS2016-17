using UnityEngine;
using System.Collections;
using System;

public class PizzaAttackSetup : AEAttackSetup {

    public GameObject m_SliceVisuals;

    public GameObject m_SliceInstance;
    public Vector3 m_StartPosition;
    public Quaternion m_StartRotation;

    public AEAttackSeries m_Series;

    private IEnumerator m_DestroyEnumerator;

    public PizzaAttackSetup(GameObject m_PizzaSetupPrefab, AEAttackSeries series)
    {
        this.m_SliceVisuals = m_PizzaSetupPrefab;
        this.m_Series = series;
    }

    public override void LaunchPart()
    {
        // later on: play some animation, etc. 
        m_SliceInstance = (GameObject) GameObject.Instantiate(m_SliceVisuals, m_StartPosition, m_StartRotation);
        m_SliceInstance.SetActive(true);

        m_DestroyEnumerator = RemoveAfter(1.5f);
        m_Series.m_Behaviour.StartCoroutine(m_DestroyEnumerator);
    }

    public IEnumerator RemoveAfter (float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(m_SliceInstance);
    }

    public override GameObject GetObject()
    {
        return m_SliceInstance;
    }

    public void Destroy()
    {
        if (m_DestroyEnumerator != null)
            m_Series.m_Behaviour.StopCoroutine(m_DestroyEnumerator);
        if (m_SliceInstance != null)
            GameObject.Destroy(m_SliceInstance);
    }
}
