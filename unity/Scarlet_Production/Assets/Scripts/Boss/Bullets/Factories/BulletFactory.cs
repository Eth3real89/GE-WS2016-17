using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFactory : MonoBehaviour {

    public BulletBehaviour m_Base;
    public BulletMovement m_Movement;
    public BulletExpirationBehaviour m_Expire;
    public BulletOnExpireBehaviour m_OnExpire;

    public virtual BulletBehaviour CreateBullet()
    {
        BulletBehaviour copy = Instantiate(m_Base, this.transform.position, this.transform.rotation);

        copy.m_Movement = Instantiate(m_Movement);
        copy.m_Movement.transform.parent = copy.transform;

        copy.m_Expiration = Instantiate(m_Expire);
        copy.m_Expiration.transform.parent = copy.transform;

        copy.m_OnExpire = Instantiate(m_OnExpire);
        copy.m_OnExpire.transform.parent = copy.transform;

        return copy;
    }

}
