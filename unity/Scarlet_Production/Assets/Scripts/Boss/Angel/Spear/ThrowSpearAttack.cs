using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSpearAttack : AngelAttack
{

    public TurnTowardsScarlet m_TurnCommand;

    public float m_TimeBeforeHurl;
    public float m_HurlSpeed;
    public float m_SpearLifetime;

    public Spear m_SpearToHurl;
    public Transform m_StartPoint;

    public AngelWeapons m_Weapons;
    public GameObject m_AngelStaff;
    public float m_TimeStaffRespawns;
    public float m_TimeBeforeEnd;

    protected IEnumerator m_Timer;

    public override void StartAttack()
    {
        base.StartAttack();

        m_Animator.SetTrigger("ThrowSpearTrigger");

        m_Timer = WaitBeforeHurl();
        StartCoroutine(m_Timer);
    }

    protected IEnumerator WaitBeforeHurl()
    {
        float t = 0;
        while((t += Time.deltaTime) < AdjustTime(m_TimeBeforeHurl))
        {
            m_TurnCommand.DoTurn();
            yield return null;
        }

        HurlSpear();
    }

    protected void HurlSpear()
    {
        m_Weapons.RemoveTip();
        m_AngelStaff.SetActive(false);

        m_SpearToHurl.transform.position = m_AngelStaff.transform.position - new Vector3(0, m_AngelStaff.transform.position.y, 0);
        m_SpearToHurl.transform.rotation = Quaternion.Euler(0, m_Boss.transform.rotation.eulerAngles.y, 0);
        m_SpearToHurl.gameObject.SetActive(true);
        m_SpearToHurl.LaunchSpear(AdjustSpeed(m_HurlSpeed), m_SpearLifetime);

        m_Timer = RespawnStaff();
        StartCoroutine(m_Timer);
    }

    protected IEnumerator RespawnStaff()
    {
        yield return new WaitForSeconds(AdjustTime(m_TimeStaffRespawns));
        m_AngelStaff.SetActive(true);

        m_Timer = WaitThenEnd();
        StartCoroutine(m_Timer);
    }

    protected IEnumerator WaitThenEnd()
    {
        yield return new WaitForSeconds(AdjustTime(m_TimeBeforeEnd));
        m_AngelStaff.SetActive(true);

        m_Callback.OnAttackEnd(this);
    }


    public override void CancelAttack()
    {
        if (m_Timer != null)
            StopCoroutine(m_Timer);
    }
}
