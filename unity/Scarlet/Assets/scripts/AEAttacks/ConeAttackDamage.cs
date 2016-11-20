using UnityEngine;
using System.Collections;

public class ConeAttackDamage : MonoBehaviour {
    private int m_FrameCount = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider other)
    {
        m_FrameCount++;
        HandleScarletCollision(other, 20f * Time.deltaTime * m_FrameCount);
    }

    void OnTriggerExit(Collider other)
    {
        m_FrameCount = 0;
    }

    private void HandleScarletCollision(Collider other, float damage)
    {
        if (other.GetComponent<Rigidbody>() != null &&
            GameController.Instance.IsScarlet(other.GetComponent<Rigidbody>()))
        {
            GameController.Instance.HitScarlet(GameController.Instance.m_Boss, damage, false);
        }
    }
}
