using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchStepSound : MonoBehaviour
{
    public StepsHandler.StepType m_Type;
    private StepsHandler m_StepHander;

    private void Start()
    {
        m_StepHander = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<StepsHandler>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            m_StepHander.SetStepSound(m_Type);
        }
    }
}
