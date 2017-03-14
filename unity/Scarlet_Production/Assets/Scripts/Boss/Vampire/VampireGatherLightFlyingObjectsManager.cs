using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireGatherLightFlyingObjectsManager : MonoBehaviour, VampireGatherLightFlyingObject.Callback {

    public Transform[] m_SpawnPoints;
    public VampireGatherLightFlyingObject m_Prefab;

    protected List<VampireGatherLightFlyingObject> m_Instances;

    public void OnGatherLightStart(float gatherLightTime, Transform vampireLocation)
    {
        if (m_Instances == null)
            m_Instances = new List<VampireGatherLightFlyingObject>();

        for(int i = 0; i < m_SpawnPoints.Length; i++)
        {
            VampireGatherLightFlyingObject obj = SpawnFlyingObject(m_SpawnPoints[i], vampireLocation);
            obj.FlyTo(this, vampireLocation.position * 1f, gatherLightTime);
        }
    }

    protected VampireGatherLightFlyingObject SpawnFlyingObject(Transform position, Transform vampireLocation)
    {
        VampireGatherLightFlyingObject obj = Instantiate(m_Prefab).GetComponent<VampireGatherLightFlyingObject>();
        obj.transform.position = position.position * 1f;
        obj.transform.LookAt(vampireLocation);

        // this line makes the objects fly towards the vampire's position without changing their "y" position.
        //obj.transform.rotation = Quaternion.Euler(0, obj.transform.rotation.eulerAngles.y, 0);

        obj.gameObject.SetActive(true);
        return obj;
    }
    
    public void CancelIrregularly()
    {
        if (m_Instances == null)
            m_Instances = new List<VampireGatherLightFlyingObject>();

        // have to do this in reverse: they are removed as soon as they are cancelled.
        for(int i = m_Instances.Count - 1; i >= 0; i--)
        {
            if (m_Instances[i] != null)
                m_Instances[i].CancelFlying();
        }
    }

    public void OnFlyingObjStart(VampireGatherLightFlyingObject obj)
    {
        m_Instances.Add(obj);
    }

    public void OnFlyingObjReachGoal(VampireGatherLightFlyingObject obj)
    {
        if (obj != null && m_Instances.Contains(obj))
        {
            m_Instances.Remove(obj);
        }
    }

    public void OnFlyingObjCancel(VampireGatherLightFlyingObject obj)
    {
        if (obj != null && m_Instances.Contains(obj))
        {
            m_Instances.Remove(obj);
        }
    }
}
