using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAEDamage : AEAttackDamage
{

    public VolumetricLines.VolumetricLineStripBehavior m_VolumetricBehavior;
    private DefaultCollisionHandler m_CollisionHandler;

    private BoxCollider m_BoxCollider;

    private IEnumerator m_Timer;

    protected Transform m_OldParent;

    public virtual void SetAngle(float angle)
    {
        transform.localRotation = Quaternion.Euler(0, angle, 0);
    }

    public virtual void Expand(float time, float size, ExpandingDamageCallbacks callback)
    {
        m_OldParent = this.transform.parent;
        this.transform.parent = null;

        m_Active = true;
        m_BoxCollider = GetComponent<BoxCollider>();

        gameObject.SetActive(true);
        m_CollisionHandler = new DefaultCollisionHandler(this);

        m_Timer = ExpansionRoutine(time, size, callback);
        StartCoroutine(m_Timer);
    }

    public virtual void Rotate(float time, float angles, ExpandingDamageCallbacks callback)
    {
        if (m_Timer != null)
            StopCoroutine(m_Timer);

        m_Timer = RotationRoutine(time, angles, callback);
        StartCoroutine(m_Timer);
    }

    protected virtual IEnumerator ExpansionRoutine(float time, float size, ExpandingDamageCallbacks callback)
    {
        float t = 0;

        Vector3[] points = m_VolumetricBehavior.LineVertices;
        Vector3 lastPoint = new Vector3(0, 0, 1);

        m_BoxCollider.center = new Vector3(0, 0, 0);
        float initialZOffset = m_BoxCollider.center.z;

        while ((t += Time.deltaTime) < time)
        {
            float sizeDelta = size * t / time;
            lastPoint.z = sizeDelta;

            m_BoxCollider.center = new Vector3(0, 0, initialZOffset + sizeDelta / 2);
            m_BoxCollider.size = new Vector3(m_BoxCollider.size.x, m_BoxCollider.size.y, sizeDelta);

            points[points.Length - 1] = lastPoint;

            m_VolumetricBehavior.UpdateLineVertices(points);

            yield return null;
        }


        callback.OnExpansionOver(this);
    }

    internal void CancelDamage()
    {
        m_Active = false;

        if (m_Timer != null)
            StopCoroutine(m_Timer);
    }

    protected virtual IEnumerator RotationRoutine(float time, float angle, ExpandingDamageCallbacks callback)
    {
        float t = 0;
        float prevAngleChange = 0;

        while ((t += Time.deltaTime) < time)
        {
            float angleDelta = t / time * angle;
            transform.Rotate(Vector3.up, angleDelta - prevAngleChange);
            callback.OnRotation(this, angleDelta);

            prevAngleChange = angleDelta;

            yield return null;
        }

        callback.OnRotationOver(this);
        this.transform.parent = m_OldParent;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!m_Active)
            return;

        Hittable hittable = other.GetComponentInChildren<Hittable>();
        if (hittable != null && hittable is PlayerHittable)
        {
            m_CollisionHandler.HandleScarletCollision(other);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        Hittable hittable = other.GetComponentInChildren<Hittable>();
        if (!m_Active)
        {
            if (hittable != null && hittable is PlayerHittable)
                m_Active = true;
            else
                return;
        }

        if (hittable != null && hittable is PlayerHittable)
        {
            m_CollisionHandler.HandleScarletLeave(other);
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (!m_Active)
            return;

        Hittable hittable = other.GetComponentInChildren<Hittable>();
        if (hittable != null && hittable is PlayerHittable)
        {
            m_CollisionHandler.HandleScarletCollision(other);
        }
    }

    public interface ExpandingDamageCallbacks
    {
        void OnExpansionOver(BeamAEDamage damage);
        void OnRotationOver(BeamAEDamage damage);
        void OnRotation(BeamAEDamage damage, float angle);
    }
}
