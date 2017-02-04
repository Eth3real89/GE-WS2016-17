using System.Collections;
using UnityEngine;

public class CircularAttackDamage : AEAttackDamage {

    public TurnTowardsScarlet m_TurnTowardsScarlet;

    public float m_Angle;
    public float m_Distance;

    public override void Activate()
    {
        base.Activate();

        StartCoroutine(WhileActive());
    }

    private IEnumerator WhileActive()
    {
        while (m_Active)
        {
            float angleTowardsScarlet = m_TurnTowardsScarlet.CalculateAngleTowardsScarlet();
            if (Mathf.Abs(angleTowardsScarlet) <= m_Angle / 2 && 
                Vector3.Distance(m_TurnTowardsScarlet.m_Scarlet.transform.position, transform.position) <= m_Distance / 2)
            {
                Hittable hittable = m_TurnTowardsScarlet.m_Scarlet.GetComponentInChildren<Hittable>();
                if (hittable != null)
                {
                    hittable.Hit(this);
                    this.m_Active = false;
                }
            }

            yield return null;
        }
    }

    public void DisableDamage()
    {
        m_Active = false;
    }

    public override BlockableType Blockable()
    {
        return BlockableType.None;
    }

    public override float DamageAmount()
    {
        return 40f;
    }
}
