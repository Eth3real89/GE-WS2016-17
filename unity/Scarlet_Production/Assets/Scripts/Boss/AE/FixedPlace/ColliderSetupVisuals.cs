using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSetupVisuals : FixedPlaceSetupVisuals {

    public float m_TimeShown = 1f;
    public float m_TrackingSpeed = 1f;

    public GameObject m_Target;

    public Material m_MaterialDuringSetup;

    public override void ShowSetup(SetupCallback callback)
    {
        base.ShowSetup(callback);

        gameObject.SetActive(true);

        transform.position = m_Target.transform.position;

        Renderer r = GetComponentInChildren<Renderer>();
        r.material = this.m_MaterialDuringSetup;

        StartCoroutine(Track());
    }

    private IEnumerator Track()
    {
        float t = 0;
        while ((t += Time.deltaTime) <= m_TimeShown)
        {
            Vector3 distVec = m_Target.transform.position - transform.position;

            float moveDistance = m_TrackingSpeed * Time.deltaTime;

            if (distVec.magnitude < moveDistance)
            {
                transform.position = m_Target.transform.position;
            }
            else
            {
                transform.position += distVec.normalized * moveDistance;
            }

            yield return null;
        }

        m_Callback.OnSetupOver();
    }

    public override void HideAttack()
    {
        base.HideAttack();
    }

}
