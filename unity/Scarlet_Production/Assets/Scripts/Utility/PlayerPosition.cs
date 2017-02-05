using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PlayerPosition : MonoBehaviour
{
    public int m_StartAtPoint;
    public GameObject m_Scarlet;
    public List<Transform> m_StartingPoint;

    public void Update()
    {
        if (m_StartingPoint.Count != 0 && m_StartingPoint.Count >= m_StartAtPoint - 1)
            m_Scarlet.transform.position = m_StartingPoint[m_StartAtPoint].transform.position;
    }
}