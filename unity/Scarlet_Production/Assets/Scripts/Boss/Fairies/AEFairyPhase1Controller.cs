using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEFairyPhase1Controller : AEFairyController {

    public float m_RotationTime = 25f;

    public override void Initialize(FairyControllerCallbacks callbacks)
    {
        base.Initialize(callbacks);

        for(int i = 0; i < m_Combos.Length; i++)
        {
            AttackCombo combo = m_Combos[i];
            for(int j = 0; j < combo.m_Attacks.Length; j++)
            {
                if (combo.m_Attacks[j] is GrowingThenRotatingConeAttack)
                {
                    ((GrowingThenRotatingConeAttack)combo.m_Attacks[j]).m_RotationTime = m_RotationTime;
                }
            }
        }
    }

    public override void Continue()
    {
        int before = m_CurrentComboIndex - 1;

        if (before != -1)
        {
            while (true)
            {
                m_CurrentComboIndex = Random.Range(0, m_Combos.Length - 1);
                if (m_CurrentComboIndex != before && (m_CurrentComboIndex) % 3 != (before) % 3)
                    break;
            }
        }

        base.Continue();
    }

}
