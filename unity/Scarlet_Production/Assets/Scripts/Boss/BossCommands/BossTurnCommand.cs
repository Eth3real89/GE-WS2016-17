using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTurnCommand : BossCommand {

    public static float CalculateAngleTowards(Transform boss, Transform goal)
    {
        float angle = CalculateAngleTowards(boss.position, goal.position);

        float currentAngle = boss.rotation.eulerAngles.y;

        return (angle - currentAngle) % 360;
    }

    public static float CalculateAngleTowards(Vector3 boss, Vector3 goal)
    {
        float angle = Mathf.Atan2(
            goal.x - boss.x,
            goal.z - boss.z) * Mathf.Rad2Deg;

        return angle;
    }

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
