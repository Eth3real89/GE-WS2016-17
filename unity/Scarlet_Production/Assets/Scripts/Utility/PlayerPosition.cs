using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[System.Serializable]
public class PlayerPosition : MonoBehaviour
{
    public int m_StartAtPoint;
    public GameObject m_Scarlet;
    public List<Transform> m_StartingPoints;

    private void OnEnable()
    {
        m_Scarlet = GameObject.Find("ScarletWrapper");
    }

    public void Update()
    {
        if (m_StartingPoints.Count == 0 || m_StartingPoints == null)
        {
            m_StartingPoints = new List<Transform>();
            m_StartingPoints.Add(transform);
        }
        if (m_StartingPoints.Count != 0 && m_StartingPoints.Count >= m_StartAtPoint - 1)
            m_Scarlet.transform.position = m_StartingPoints[m_StartAtPoint].position;
    }
}