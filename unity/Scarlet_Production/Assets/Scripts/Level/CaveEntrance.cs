using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CaveEntrance : Interactor
{
    public UnityEvent afterBreakoutEvent;

    public delegate void OpenGateEvent();

    public static OpenGateEvent explodeEvent;
    public static OpenGateEvent dropEvent;
    public static OpenGateEvent destroyEvent;

    public override void Interact()
    {
        if (!m_IsInteractible)
            return;

        m_IsInteractible = false;
        GetComponentInChildren<UIItemPickupController>().StopInteraction();
        new FARQ().ClipName("rocks").Location(transform).Play();
        StartCoroutine(OpenCave());
    }

    IEnumerator OpenCave()
    {
        explodeEvent();
        yield return new WaitForSeconds(3);
        dropEvent();
        if (afterBreakoutEvent != null)
            afterBreakoutEvent.Invoke();
        yield return new WaitForSeconds(3);
        destroyEvent();
    }
}
