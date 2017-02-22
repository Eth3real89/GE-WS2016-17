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

        Vector3 v1 = m_MoveCommand.m_Boss.transform.position;
        Vector3 v2 = goal.position;

        v1.y = 0;
        v2.y = 0;

        while(Vector3.Distance(v1, v2) > 0.3f)
        {
            m_BossBody.MoveRotation(Quaternion.Euler(0, BossTurnCommand.CalculateAngleTowards(m_BossBody.transform.position, goal.position), 0));

            m_MoveCommand.DoMove(v2.x - v1.x, v2.z - v1.z);

            v1 = m_MoveCommand.m_Boss.transform.position;
            v1.y = 0;

            yield return null;
        }

        m_MoveCommand.StopMoving();
        owner.StartCoroutine(onEvasionFinished);
    }

    public void Cancel()
    {
        if (m_Enumerator != null)
            StopCoroutine(m_Enumerator);
    }

}
