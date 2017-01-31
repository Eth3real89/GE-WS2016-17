using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBehaviour : MonoBehaviour {

    public BulletMovement m_Movement;
    public BulletExpirationBehaviour m_Expiration;
    public BulletOnExpireBehaviour m_OnExpire;

    public bool m_KillBullet;

    private IEnumerator m_BulletLifeEnumerator;

    public BulletBehaviour.BulletCallbacks m_BulletCallbacks;


    public virtual void Launch(BulletBehaviour.BulletCallbacks callbacks)
    {
        m_BulletCallbacks = callbacks;

        m_KillBullet = false;
        m_BulletLifeEnumerator = LifeCycle();
        StartCoroutine(m_BulletLifeEnumerator);
    }

    protected virtual IEnumerator LifeCycle()
    {
        m_Expiration.OnLaunch(this);

        while(!m_KillBullet)
        {
            m_Movement.HandleMovement(this);

            yield return null;
        }

        m_OnExpire.OnBulletExpires(this);
    }

    public abstract void Kill();
    public abstract void MoveBy(Vector3 movement);


    public interface BulletCallbacks
    {
        void OnBulletCreated(BulletBehaviour bullet);
        void OnBulletHitTarget(BulletBehaviour bullet);
        void OnBulletParried(BulletBehaviour bullet);
        void OnBulletDestroyed(BulletBehaviour bullet);
        void LoseBullet(Bullet bullet);
    }

}
