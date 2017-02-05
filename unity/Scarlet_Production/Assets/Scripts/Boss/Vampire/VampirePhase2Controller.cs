using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampirePhase2Controller : VampireController
{

    public override void StartPhase(BossfightCallbacks callbacks)
    {
        base.StartPhase(callbacks);

        StartCoroutine(StartAfterDelay());
    }
}
