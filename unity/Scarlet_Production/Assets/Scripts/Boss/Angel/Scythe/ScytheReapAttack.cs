using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheReapAttack : AngelMeleeAttack {

    public PlayerControls m_PlayerControls;
    public PlayerHittable m_PlayerHittable;

    public float m_PullForceForward;

    public float m_EarliestTimePullingStarts;
    protected IEnumerator m_PullTowardsBossEnumerator;

    protected override void SetDamageActive()
    {
        base.SetDamageActive();

        m_PullTowardsBossEnumerator = ActivatePullingAfterWaiting();
        StartCoroutine(m_PullTowardsBossEnumerator);
    }

    protected IEnumerator ActivatePullingAfterWaiting()
    {
        yield return new WaitForSeconds(AdjustTime(m_EarliestTimePullingStarts));

        if (m_SuccessLevel == 1)
        {
            Rigidbody scarletBody = m_PlayerHittable.GetComponent<Rigidbody>();
            PullScarlet(scarletBody);
        }
    }

    public override void OnSuccessfulHit()
    {
        base.OnSuccessfulHit();
        m_PlayerControls.DisableAllCommands();

        Rigidbody scarletBody = m_PlayerHittable.GetComponent<Rigidbody>();

        Vector3 lookPosition = m_Boss.transform.position - scarletBody.transform.position;
        lookPosition.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(lookPosition);
        // look at boss
        scarletBody.transform.rotation = lookRotation;

        scarletBody.velocity = Vector3.zero;

        // apply "pull" force:
        if (m_PullTowardsBossEnumerator == null)
            PullScarlet(scarletBody);
    }

    protected void PullScarlet(Rigidbody scarletBody)
    {
        scarletBody.AddForce(scarletBody.transform.forward * m_PullForceForward * scarletBody.mass, ForceMode.Impulse);
    }

    public override void HandleCollision(Collider other, bool initialCollision)
    {
    }

    public override void HandleScarletLeave(Collider other)
    {
    }

    public override void OnBlockDamage()
    {
    }

    public override void OnParryDamage()
    {
    }

    public override void CancelAttack()
    {
        base.CancelAttack();

        if (m_PullTowardsBossEnumerator != null)
            StopCoroutine(m_PullTowardsBossEnumerator);
    }
}
