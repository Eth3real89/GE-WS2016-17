using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevAEController : BossController {

    private new void Start()
    {
        RegisterComboCallback();
        m_CurrentComboIndex = 0;
        StartCoroutine(StartAfterDelay());
    }

}
