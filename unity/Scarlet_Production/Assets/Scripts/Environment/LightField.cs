using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightField : MonoBehaviour
{
    public enum RetreatDirection { Left, Right, Top, Bottom, BothX, BothZ }
    public enum LightFieldClass { Regular, Strong };

    public LightFieldClass m_Class = LightFieldClass.Regular;
    public RetreatDirection m_Direction = RetreatDirection.Left;

    private void OnTriggerEnter(Collider other)
    {
        LightFieldResponder responder = other.GetComponentInChildren<LightFieldResponder>();
        if (responder != null)
        {
            responder.OnEnterLightField(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LightFieldResponder responder = other.GetComponentInChildren<LightFieldResponder>();
        if (responder != null)
        {
            responder.OnExitLightField(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        LightFieldResponder responder = other.GetComponentInChildren<LightFieldResponder>();
        if (responder != null)
        {
            responder.OnStayInLightField(m_Class);
        }
    }

    public interface LightFieldResponder
    {
        void OnEnterLightField(GameObject lightField);
        void OnStayInLightField(LightFieldClass lightFieldClass);
        void OnExitLightField(GameObject lightField);
    }

    public Vector3 GetVectorFromDirection(Vector3 velocity)
    {
        switch (m_Direction)
        {
            case RetreatDirection.Left:
                return Vector3.left;
            case RetreatDirection.Right:
                return Vector3.right;
            case RetreatDirection.Top:
                return Vector3.forward;
            case RetreatDirection.Bottom:
                return Vector3.back;
            case RetreatDirection.BothX:
                return GetXVel(velocity);
            case RetreatDirection.BothZ:
                return GetZVel(velocity);
            default:
                return Vector3.left;
        }
    }

    private Vector3 GetXVel(Vector3 velocity)
    {
        if (velocity.x > 0)
            return Vector3.left;
        else
            return Vector3.right;
    }

    private Vector3 GetZVel(Vector3 velocity)
    {
        if (velocity.z > 0)
            return Vector3.back;
        else
            return Vector3.forward;
    }
}
