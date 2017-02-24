using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterEvasionCommand : BossCommand {

    public GameObject m_Scarlet;
    public Rigidbody m_BossBody;

    public BossMoveCommand m_MoveCommand;

    protected IEnumerator m_Enumerator;

    public void EvadeTowards(Transform goal, MonoBehaviour owner, IEnumerator onEvasionFinished)
    {
        m_Enumerator = EvasionRoutine(goal, owner, onEvasionFinished);
        StartCoroutine(m_Enumerator);
    }

    protected virtual IEnumerator EvasionRoutine(Transform goal, MonoBehaviour owner, IEnumerator onEvasionFinished)
    {
        m_Animator.SetTrigger("DodgeRightTrigger");

        yield return new WaitForSeconds(0.3f);

        yield return ReachSpot(goal);

        owner.StartCoroutine(onEvasionFinished);
    }

    public IEnumerator ReachSpot(Transform goal)
    {
        Vector3 v1 = m_MoveCommand.m_Boss.transform.position;
        Vector3 v2 = goal.position;

        v1.y = 0;
        v2.y = 0;

        while (Vector3.Distance(v1, v2) > 0.3f)
        {
            m_BossBody.MoveRotation(Quaternion.Euler(0, BossTurnCommand.CalculateAngleTowards(m_BossBody.transform.position, goal.position), 0));

            m_MoveCommand.DoMove(v2.x - v1.x, v2.z - v1.z);

            v1 = m_MoveCommand.m_Boss.transform.position;
            v1.y = 0;

            yield return null;
        }

        m_MoveCommand.StopMoving();

    }

    public void Cancel()
    {
        if (m_Enumerator != null)
            StopCoroutine(m_Enumerator);
    }

    public virtual IEnumerator QuickPerfectRotationRoutine(float time, Transform goal = null)
    {
        if (goal == null)
            goal = m_Scarlet.transform;

        m_Animator.SetTrigger("RotationTrigger");

        float t = 0;
        while ((t += Time.deltaTime) < time)
        {
            float goalRotation = BossTurnCommand.CalculateAngleTowards(m_Boss.transform, goal);

            while (goalRotation > 180)
                goalRotation -= 360;
            while (goalRotation < -180)
                goalRotation += 360;

            m_Boss.transform.Rotate(Vector3.up, goalRotation * t / time);
            yield return null;
        }

        m_Animator.SetTrigger("StopRotationTrigger");
    }

}
