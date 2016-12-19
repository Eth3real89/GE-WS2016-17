using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTurnCommand : BossCommand {

    public void TurnBossTowards(GameObject another)
    {
        Vector3 old = m_Boss.transform.rotation.eulerAngles;
        m_Boss.transform.LookAt(another.transform);

        m_Boss.transform.rotation = Quaternion.Euler(old.x, m_Boss.transform.rotation.eulerAngles.y, old.z);

    }

    public void TurnBossBy(float angle)
    {
        m_Boss.transform.Rotate(m_Boss.transform.up, angle);
    }

}
