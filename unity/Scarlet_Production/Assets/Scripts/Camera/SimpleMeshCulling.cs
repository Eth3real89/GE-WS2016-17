using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMeshCulling : CullingBehaviour
{
    private Renderer[] m_Renderer;

    void Start()
    {
        m_Renderer = GetComponentsInChildren<Renderer>();
    }

    public override void Cull()
    {
        m_IsCulled = true;
        foreach (Renderer r in m_Renderer)
        {
            r.enabled = false;
        }
    }

    public override void UnCull()
    {
        m_IsCulled = false;
        foreach (Renderer r in m_Renderer)
        {
            r.enabled = true;
        }
    }
}
