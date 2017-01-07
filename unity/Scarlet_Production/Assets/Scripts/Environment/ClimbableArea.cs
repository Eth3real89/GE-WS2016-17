using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableArea : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
       
        ClimbAreaResponder responder = collision.collider.GetComponentInChildren<ClimbAreaResponder>();
        if (responder != null)
        {
            responder.OnEnterClimbArea();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        ClimbAreaResponder responder = collision.collider.GetComponentInChildren<ClimbAreaResponder>();
        if (responder != null)
        {
            responder.OnExitClimbArea();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ClimbAreaResponder responder = collision.collider.GetComponentInChildren<ClimbAreaResponder>();
        if (responder != null)
        {
            responder.OnStayInClimbArea();
        }
    }

    public interface ClimbAreaResponder
    {
        void OnEnterClimbArea();
        void OnStayInClimbArea();
        void OnExitClimbArea();
    }
}
