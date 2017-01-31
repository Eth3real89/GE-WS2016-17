using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampirePhase0TutorialController : BossController {

    public BossfightCallbacks m_Callback;

    public void StartPhase(BossfightCallbacks callbacks)
    {
        RegisterComboCallback();

        m_BossHittable.RegisterInterject(this);
        m_Callback = callbacks;

        StartCoroutine(StartAfterDelay());
    }
}
