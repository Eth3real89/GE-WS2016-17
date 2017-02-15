using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEFairyPhase1Controller : AEFairyController {

    public override void Continue()
    {
        int before = m_CurrentComboIndex;

        while(true)
        {
            m_CurrentComboIndex = Random.Range(1, m_Combos.Length - 1);
            if (m_CurrentComboIndex != before)
                break;
        }

        base.Continue();
    }

}
