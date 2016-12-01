using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{

    public bool m_Destroy;

    public float m_MaxLifetime = 5f;
    public float m_Velocity = 2f;
    public float m_Angle = 0f;

    private float m_PassedTime;

    // Use this for initialization
    void Start()
    {
        m_Destroy = false;
        m_PassedTime = 0;
    }

    // Update is called once per frame
    public void Update()
    {
        m_PassedTime += Time.deltaTime;
        if (m_PassedTime > m_MaxLifetime)
        {
            m_Destroy = true;
            this.DestroyBullet();
        }
    }

    public void DestroyBullet()
    {
        GameObject.Destroy(this.transform.gameObject);
    }
}
