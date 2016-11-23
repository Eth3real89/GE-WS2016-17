using UnityEngine;
using System.Collections;
using System;

public class TargetAttackSetup : AEAttack, Updateable
{
    public GameObject m_TargetPrefab;

    public GameObject m_TargetInstance;

    public AEAttackSeries m_Series;

    private IEnumerator m_DestroyEnumerator;

    public TargetAttackSetup(GameObject targetPrefab, AEAttackSeries series)
    {
        this.m_TargetPrefab = targetPrefab;
        this.m_Series = series;
    }

    public override GameObject GetObject()
    {
        return m_TargetInstance;
    }

    public override void LaunchPart()
    {
        // later on: play some animation, etc. 
        m_TargetInstance = (GameObject)GameObject.Instantiate(m_TargetPrefab,
            GameController.Instance.m_Scarlet.transform.position, Quaternion.Euler(0f, 0f, 0f));
        m_TargetInstance.SetActive(true);

        m_DestroyEnumerator = RemoveAfter(1f);
        m_Series.m_Behaviour.StartCoroutine(m_DestroyEnumerator);
        GameController.Instance.RegisterUpdateable(this);
    }

    public void Update()
    {
        m_TargetInstance.transform.position = Vector3.MoveTowards(m_TargetInstance.transform.position,
            GameController.Instance.m_Scarlet.transform.position,
            0.22f);
    }

    public IEnumerator RemoveAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(m_TargetInstance);
        GameController.Instance.UnregisterUpdateable(this);
    }

    public void Destroy()
    {
        if (m_DestroyEnumerator != null)
            m_Series.m_Behaviour.StopCoroutine(m_DestroyEnumerator);
        GameObject.Destroy(m_TargetInstance);
        GameController.Instance.UnregisterUpdateable(this);
    }
}
