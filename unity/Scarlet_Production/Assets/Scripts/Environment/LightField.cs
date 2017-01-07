using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightField : MonoBehaviour
{
    public enum RetreatDirection { Left, Right, Top, Bottom }
    public enum LightFieldClass { Regular, Strong };

    public LightFieldClass m_Class = LightFieldClass.Regular;
    public RetreatDirection m_Direction = RetreatDirection.Left;

    private void OnTriggerEnter(Collider other)
    {
        LightFieldResponder responder = other.GetComponentInChildren<LightFieldResponder>();
        if (responder != null)
        {
            responder.OnEnterLightField(m_Class, GetVectorFromDirection());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LightFieldResponder responder = other.GetComponentInChildren<LightFieldResponder>();
        if (responder != null)
        {
            responder.OnExitLightField(m_Class);
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
        void OnEnterLightField(LightFieldClass lightFieldClass, Vector3 retreatDirection);
        void OnStayInLightField(LightFieldClass lightFieldClass);
        void OnExitLightField(LightFieldClass lightFieldClass);
    }

    private Vector3 GetVectorFromDirection()
    {
        switch(m_Direction)
        {
            case RetreatDirection.Left:
                return Vector3.left;
            case RetreatDirection.Right:
                return Vector3.right;
            case RetreatDirection.Top:
                return Vector3.forward;
            case RetreatDirection.Bottom:
                return Vector3.back;
            default:
                return Vector3.left;
        }
    }
}
