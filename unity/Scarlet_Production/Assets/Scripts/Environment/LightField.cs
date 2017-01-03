using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightField : MonoBehaviour {

    public enum LightFieldClass {Regular, Strong};

    public LightFieldClass m_Class = LightFieldClass.Regular;

    private void OnTriggerEnter(Collider other)
    {
        LightFieldResponder responder = other.GetComponentInChildren<LightFieldResponder>();
        if (responder != null)
        {
            responder.OnEnterLightField(m_Class);
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
        void OnEnterLightField(LightFieldClass lightFieldClass);
        void OnStayInLightField(LightFieldClass lightFieldClass);
        void OnExitLightField(LightFieldClass lightFieldClass);
    }

}
