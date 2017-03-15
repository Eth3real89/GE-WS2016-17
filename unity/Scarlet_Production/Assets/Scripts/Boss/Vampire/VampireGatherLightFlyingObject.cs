using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireGatherLightFlyingObject : MonoBehaviour {

    public float m_FlySpeed;

    protected IEnumerator m_FlyTimer;
    protected Callback m_Callback;

	public void FlyTo(Callback callback, Vector3 goal, float gatherLightTime)
    {
        m_Callback = callback;

        m_FlyTimer = FlyRoutine(goal, gatherLightTime);
        StartCoroutine(m_FlyTimer);

        callback.OnFlyingObjStart(this);
    }

    // gatherLightTime is (intentionally) unused - do whatever you want with it.
    // (that's the time you have left before the bubble spawns).
    protected IEnumerator FlyRoutine(Vector3 goal, float gatherLightTime)
    {
        while(true)
        {
            Vector3 distance = goal - transform.position;
            distance.y = 0;

            if (distance.magnitude < 0.5)
            {
                break;
            }

            transform.position += transform.forward * m_FlySpeed * Time.deltaTime;
            yield return null;
        }

        //OnReachGoal();
    }

    protected void OnReachGoal()
    {
        m_Callback.OnFlyingObjReachGoal(this);
        Destroy(this.gameObject);
    }

    public void CancelFlying()
    {
        if (m_FlyTimer != null)
        {
            StopCoroutine(m_FlyTimer);
            m_FlyTimer = null;
        }

        m_Callback.OnFlyingObjCancel(this);
        Destroy(gameObject);
    }

    public interface Callback {
        void OnFlyingObjStart(VampireGatherLightFlyingObject obj);
        void OnFlyingObjReachGoal(VampireGatherLightFlyingObject obj);
        void OnFlyingObjCancel(VampireGatherLightFlyingObject obj);
    }

}
