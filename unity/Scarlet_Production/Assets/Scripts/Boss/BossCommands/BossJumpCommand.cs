using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossJumpCommand : BossCommand {

    // https://forum.unity3d.com/threads/how-to-calculate-force-needed-to-jump-towards-target-point.372288/

    public void JumpAt(Transform target, JumpCallback callback = null)
    {
        float jumpTime = 0f;
        Vector3 force = CalculateRequiredForce(m_Boss.transform, target, ref jumpTime);
        m_Animator.SetTrigger("JumpTrigger");

        m_Boss.GetComponentInChildren<Rigidbody>().AddForce(
            force * m_Boss.GetComponentInChildren<Rigidbody>().mass,
            ForceMode.Impulse);
        StartCoroutine(WaitForJumpEnd(jumpTime, callback));
    }

    private IEnumerator WaitForJumpEnd(float time, JumpCallback callback)
    {
        yield return new WaitForSeconds(time);

        m_Animator.SetTrigger("LandTrigger");
        if (callback != null)
            callback.OnLand();
    }
    
    // quite a long method, but I copied it (with minor adjustments) from the URL above.

    private Vector3 CalculateRequiredForce(Transform bossTransform, Transform target, ref float time)
    {
        float angle = 30f * Mathf.Deg2Rad;
        float gravity = Physics.gravity.magnitude;

        Vector3 planarTarget = new Vector3(target.position.x, 0, target.position.z);
        Vector3 planarPostion = new Vector3(bossTransform.position.x, 0, bossTransform.position.z);

        float distance = Vector3.Distance(planarTarget, planarPostion);
        float yOffset = transform.position.y - target.position.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        if (bossTransform.position.x > target.position.x)
            finalVelocity.x *= -1;

        float jumpTime = Mathf.Sqrt(2 * Mathf.Abs(yOffset - finalVelocity.y) / gravity);
        time = jumpTime;

        return finalVelocity;
    }

    public interface JumpCallback
    {
        void OnLand();
    }

}
