using System.Collections;
using UnityEngine;

public class CaveEntrance : Interactor
{
    public delegate void OpenGateEvent();

    public static OpenGateEvent explodeEvent;
    public static OpenGateEvent dropEvent;
    public static OpenGateEvent destroyEvent;

    public override void Interact()
    {
        StartCoroutine(OpenCave());
    }

    IEnumerator OpenCave()
    {
        explodeEvent();
        yield return new WaitForSeconds(3);
        dropEvent();
        yield return new WaitForSeconds(3);
        destroyEvent();
    }
}
