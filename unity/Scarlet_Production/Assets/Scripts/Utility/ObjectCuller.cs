using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectCuller : MonoBehaviour
{
    public Camera m_Camera;
    public GameObject m_Scarlet;

    private List<GameObject> m_CurrentHits;
    private List<GameObject> m_CollidingObjects;
    private List<GameObject> m_CulledObjects;

    void Start()
    {
        m_Camera = Camera.main;
        m_Scarlet = GameObject.FindGameObjectWithTag("Player");
        m_CurrentHits = new List<GameObject>();
        m_CulledObjects = new List<GameObject>();
        m_CollidingObjects = new List<GameObject>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!m_CollidingObjects.Contains(other.transform.gameObject))
            m_CollidingObjects.Add(other.transform.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        m_CollidingObjects.Remove(other.transform.gameObject);
    }

    void Update()
    {
        m_CurrentHits.AddRange(m_CollidingObjects);
        RaycastHit[] hits;
        Vector3 dir = m_Scarlet.transform.position - m_Camera.transform.position;
        Ray ray = new Ray(m_Camera.transform.position, dir);
        float dist = Vector3.Distance(m_Scarlet.transform.position, m_Camera.transform.position);
        hits = Physics.RaycastAll(ray, dist);

        foreach (RaycastHit hit in hits)
        {
            m_CurrentHits.Add(hit.transform.gameObject);
        }

        foreach (GameObject objectToCull in m_CurrentHits.Except(m_CulledObjects))
        {
            CullingBehaviour culling = objectToCull.GetComponent<CullingBehaviour>();
            if (culling != null)
            {
                m_CulledObjects.Add(objectToCull);
                culling.Cull();
            }
        }

        GameObject[] objectsToUncull = m_CulledObjects.Except(m_CurrentHits).ToArray<GameObject>();
        foreach (GameObject objectToUncull in objectsToUncull)
        {
            CullingBehaviour culling = objectToUncull.GetComponent<CullingBehaviour>();
            if (culling != null)
            {
                m_CulledObjects.Remove(objectToUncull);
                culling.UnCull();
            }
        }
        m_CurrentHits.Clear();
    }
}
