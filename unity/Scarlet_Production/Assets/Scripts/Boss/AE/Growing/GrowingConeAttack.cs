using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingConeAttack : GrowingAEAttack {

    public CircularAttackVisuals m_AttackVisuals;
    public CircularAttackDamage m_Damage;

    public VolumetricLines.VolumetricLineStripBehavior m_volumetricBehavior;

    public float m_Angle;
    public float m_EndSize;
    public float m_GrowTime;

    protected IEnumerator m_GrowEnumerator;

    private Material[] m_LineMaterials;

    public override void StartAttack()
    {
        base.StartAttack();

        m_AttackVisuals.m_Angle = m_Angle;
        
        m_Damage.m_Angle = m_Angle;

        m_GrowEnumerator = Grow();
        StartCoroutine(m_GrowEnumerator);
    }


    private IEnumerator Grow()
    {
        m_AttackVisuals.m_SetFixedNumLines = true;
        m_AttackVisuals.m_FixedNumLines = 30;

        m_Damage.m_Distance = 0f;
        m_Damage.Activate();
        m_AttackVisuals.m_Size = 1f;
        m_AttackVisuals.ShowAttack();

        GameObject[] lines = m_AttackVisuals.GetLines();
        m_LineMaterials = new Material[lines.Length];
        for(int i = 0; i < lines.Length; i++)
        {
            m_LineMaterials[i] = lines[i].GetComponent<MeshRenderer>().material;
        }

        int id = Shader.PropertyToID("_LineWidth");
        int id2 = Shader.PropertyToID("_LineScale");

        float t = 0;
        while((t += Time.deltaTime) < m_GrowTime)
        {
            float currentSize = (t / m_GrowTime) * m_EndSize;

            m_AttackVisuals.transform.localScale = new Vector3(1, 0, 1) * currentSize;
            m_Damage.m_Distance = currentSize;

            for(int i = 0; i < m_LineMaterials.Length; i++)
            {
                m_LineMaterials[i].SetFloat(id, 1 / m_LineMaterials[i].GetFloat(id2));
            }

            yield return null;
        }

        AfterGrow();
    }

    protected virtual void AfterGrow()
    {
        m_Damage.DisableDamage();
        m_AttackVisuals.HideAttack();

        m_Callback.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        base.CancelAttack();

        m_AttackVisuals.HideAttack();
        m_Damage.DisableDamage();
        m_Damage.m_Active = false;

        if (m_GrowEnumerator != null)
            StopCoroutine(m_GrowEnumerator);
    }


}
