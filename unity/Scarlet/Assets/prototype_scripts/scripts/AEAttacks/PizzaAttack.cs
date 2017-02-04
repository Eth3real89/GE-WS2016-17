using UnityEngine;
using System.Collections;
using System;

public class PizzaAttack : AEAttack {

    public GameObject m_SliceVisuals;

    public GameObject m_SliceInstance;
    public Vector3 m_StartPosition;
    public Quaternion m_StartRotation;

    public AEAttackSeries m_Series;

    private IEnumerator m_DestroyEnumerator;

    public PizzaAttack(GameObject m_PizzaSetupPrefab, AEAttackSeries series)
    {
        this.m_SliceVisuals = m_PizzaSetupPrefab;
        this.m_Series = series;
    }

    public override void LaunchPart()
    {
        // later on: play some animation, etc. 
        m_SliceInstance = (GameObject) GameObject.Instantiate(m_SliceVisuals, m_StartPosition, m_StartRotation);
        
        for (int i = 0; i < m_SliceInstance.transform.childCount; i++)
        {
            Renderer r = m_SliceInstance.transform.GetChild(i).GetComponent<Renderer>();
            if (r != null)
                r.enabled = false;
            m_Series.m_Behaviour.StartCoroutine(ShowDelayed(m_SliceInstance.transform.GetChild(i), i * 0.1f));
        }

        m_SliceInstance.SetActive(true);
        CapsuleCollider collider = m_SliceInstance.GetComponent<CapsuleCollider>();

        m_DestroyEnumerator = RemoveAfter(1.5f);
        m_Series.m_Behaviour.StartCoroutine(m_DestroyEnumerator);
    }

    private IEnumerator ShowDelayed(Transform obj, float time)
    {
        yield return new WaitForSeconds(time);

        Renderer r = obj.GetComponent<Renderer>();
        if (r != null)
            r.enabled = true; 
    }

    public IEnumerator RemoveAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy();
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
