using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFactory : MonoBehaviour {

    public enum Type {NonBlockable, Blockable, Deflectable, Grenade};
    public Vector3 m_BulletScale = new Vector3(0.9f, 0.9f, 0.9f);

    public Type m_Type;
    public BulletMovement m_Movement;
    public BulletExpirationBehaviour m_Expire;
    public BulletOnExpireBehaviour m_OnExpire;

    public float m_Damage = 15f;

    public virtual BulletBehaviour CreateBullet()
    {
        BulletBehaviour prefab = GetPrefab(m_Type);
        prefab.transform.localScale = m_BulletScale;

        BulletBehaviour copy = Instantiate(prefab, this.transform.position, this.transform.rotation);

        copy.m_Movement = Instantiate(m_Movement);
        copy.m_Movement.transform.parent = copy.transform;

        copy.m_Expiration = Instantiate(m_Expire);
        copy.m_Expiration.transform.parent = copy.transform;

        copy.m_OnExpire = Instantiate(m_OnExpire);
        copy.m_OnExpire.transform.parent = copy.transform;

        if (copy is Bullet)
        {
            BulletDamage dmg = copy.GetComponentInChildren<BulletDamage>();
            if (dmg != null)
                dmg.m_DamageAmount = m_Damage;
        }

        return copy;
    }

    private BulletBehaviour GetPrefab(Type m_Type)
    {
        if (m_Type == Type.NonBlockable)
        {
            return BulletPrefabManager.Instance.m_NonBlockable;
        }
        else if (m_Type == Type.Blockable)
        {
            return BulletPrefabManager.Instance.m_Blockable;
        }
        else if (m_Type == Type.Grenade)
        {
            return BulletPrefabManager.Instance.m_Grenade;
        }
        else
        {
            return BulletPrefabManager.Instance.m_Deflectable;
        }
    }
}
