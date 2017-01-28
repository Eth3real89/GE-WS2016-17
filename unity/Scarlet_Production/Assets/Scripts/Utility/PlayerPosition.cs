using UnityEngine;

[ExecuteInEditMode]
public class PlayerPosition : MonoBehaviour
{
    public GameObject m_Scarlet;
    public int m_StartAtPoint;
    public Transform[] m_StartingPoint;

    void Update()
    {
        if (m_StartingPoint.Length != 0 && m_StartingPoint.Length >= m_StartAtPoint - 1)
            m_Scarlet.transform.position = m_StartingPoint[m_StartAtPoint].transform.position;
    }
}