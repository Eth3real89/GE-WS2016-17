using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamageTrigger : MonoBehaviour {

    public BulletDamageCallback m_Callback;
    public bool m_Active = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!m_Active)
            return;

        if (other.GetComponentInChildren<PlayerManager>() != null)
        {
            if (m_Callback != null)
                m_Callback.OnScarletCollidesWithBullet(other.gameObject);
        }
        else if (other.GetComponentInChildren<WerewolfHittable>() != null)
        {
            if (m_Callback != null)
                m_Callback.OnBossCollidesWithBullet(other.gameObject);
        }

        if (m_Callback != null)
            m_Callback.OnBulletCollision(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!m_Active)
            return;

        if (other.GetComponentInChildren<PlayerManager>() != null)
        {
            if (m_Callback != null)
                m_Callback.OnScarletCollidesWithBullet(other.gameObject);
        }
        else if (other.GetComponentInChildren<WerewolfHittable>() != null)
        {
            if (m_Callback != null)
                m_Callback.OnBossCollidesWithBullet(other.gameObject);
        }

        if (m_Callback != null)
            m_Callback.OnBulletCollision(other);
    }

    public interface BulletDamageCallback
    {
        void OnScarletCollidesWithBullet(GameObject scarlet);
        void OnBossCollidesWithBullet(GameObject scarlet);
        void OnBulletCollision(Collider other);
    }

}
