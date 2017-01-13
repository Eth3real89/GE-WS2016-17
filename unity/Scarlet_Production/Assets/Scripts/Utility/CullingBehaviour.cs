using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CullingBehaviour : MonoBehaviour
{
    public bool m_IsCulled;

    public abstract void Cull();
    public abstract void UnCull();
}
