using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelWeapons : MonoBehaviour {

    protected static AngelWeapons _Instance;

    public static AngelWeapons Instance {
        get
        {
            return _Instance;
        }
    }

    public static void SetGlow(float glow)
    {
        if (_Instance == null)
            return;

        if (_Instance.m_CurrentTip != null)
        {
            SetRendererEmission(_Instance.m_CurrentTip, glow);
        }
    }

    public static void SetRendererEmission(GameObject go, float glow)
    {
        Renderer r = go.GetComponent<Renderer>();
        if (r != null)
        {
            try
            {
                foreach (Material m in r.materials)
                {
                    Color emissionColor = new Color(glow, glow, glow, 1f);
                    m.SetColor("_EmissionColor", emissionColor);
                    m.SetColor("_EmissionColorUI", emissionColor);
                }
                DynamicGI.UpdateMaterials(r);
            }
            catch { }
        }
    }

    public enum Tips {Axe, Crosswbow, Hammer, Hellebarde, Scythe, Spear, Magic };

    public GameObject m_Axe;
    public GameObject m_Crossbow;
    public GameObject m_Hammer;
    public GameObject m_Hellebarde;
    public GameObject m_Scythe;
    public GameObject m_Spear;

    public Material m_ChangeMaterial;
    public Material m_NormalMaterial;

    public ControlAngelVisualisation m_Effects;

    protected GameObject m_CurrentTip;

    protected IEnumerator m_WeaponChangeEnumerator;

    protected bool m_TipIsMagic;

    private void Start()
    {
        if (_Instance == null)
            _Instance = this;

        m_TipIsMagic = false;
    }

    public void RemoveTip()
    {
        if (m_CurrentTip != null)
            m_CurrentTip.SetActive(false);
        m_TipIsMagic = false;
    }

    public void CancelChange()
    {
        if (m_CurrentTip != null)
            m_CurrentTip.SetActive(false);
        m_TipIsMagic = false;

        if (m_WeaponChangeEnumerator != null)
            StopCoroutine(m_WeaponChangeEnumerator);
    }

    public void ChangeTipTo(Tips t, IEnumerator doAfterwards, MonoBehaviour callbackOwner)
    {
        if (t == Tips.Magic)
        {
            m_TipIsMagic = true;
            if (callbackOwner != null && doAfterwards != null)
                callbackOwner.StartCoroutine(doAfterwards);
            return;
        }
        m_TipIsMagic = false;

        GameObject tip = null;

        if (t == Tips.Axe)
            tip = m_Axe;
        else if (t == Tips.Crosswbow)
            tip = m_Crossbow;
        else if (t == Tips.Hammer)
            tip = m_Hammer;
        else if (t == Tips.Hellebarde)
            tip = m_Hellebarde;
        else if (t == Tips.Scythe)
            tip = m_Scythe;
        else if (t == Tips.Spear)
            tip = m_Spear;

        if (m_CurrentTip == tip)
        {
            if (m_CurrentTip != null)
                m_CurrentTip.SetActive(true); // just to be safe!

            if (callbackOwner != null && doAfterwards != null)
                callbackOwner.StartCoroutine(doAfterwards);
        }
        else
        {
            if (m_WeaponChangeEnumerator != null)
                StopCoroutine(m_WeaponChangeEnumerator);

            m_WeaponChangeEnumerator = ChangeTipRoutine(tip, doAfterwards, callbackOwner);
            StartCoroutine(m_WeaponChangeEnumerator);
        }
    }

    protected virtual IEnumerator ChangeTipRoutine(GameObject changeTo, IEnumerator doAfterwards, MonoBehaviour callbackOwner)
    {
        print("Init tip change");
        float t = 0f;
        if (m_CurrentTip != null)
        {
            ParticleSystem[] particlesOld = m_CurrentTip.GetComponentsInChildren<ParticleSystem>();
            Renderer currentRenderer = m_CurrentTip.GetComponent<Renderer>();

            for (int i = 0; i < currentRenderer.materials.Length; i++) currentRenderer.materials[i] = m_ChangeMaterial;
            particlesOld[particlesOld.Length - 1].Play();
            while ((t += Time.deltaTime) > 0.8f)
            {
                for (int i = 0; i < currentRenderer.materials.Length; i++) currentRenderer.materials[i].SetFloat("_Cutoff", t / 0.8f);
                yield return null;
            }
            particlesOld[particlesOld.Length - 1].Stop();
            for (int i = 0; i < currentRenderer.materials.Length; i++) currentRenderer.materials[i].SetFloat("_Cutoff", 1f);
            for (int i = 0; i < currentRenderer.materials.Length; i++) currentRenderer.materials[i] = m_NormalMaterial;
            m_CurrentTip.SetActive(false);
        }
        Vector3 prevScale = changeTo.transform.localScale + new Vector3();

        changeTo.SetActive(true);

        t = 0.8f;
        ParticleSystem[] particles = changeTo.GetComponentsInChildren<ParticleSystem>();
        particles[particles.Length-1].Play();

        Renderer newRenderer = changeTo.GetComponent<Renderer>();
        for (int i = 0; i < newRenderer.materials.Length; i++) newRenderer.materials[i] = m_ChangeMaterial;
        while ((t -= Time.deltaTime) > 0)
        {
            for (int i = 0; i < newRenderer.materials.Length; i++) newRenderer.materials[i].SetFloat("_Cutoff", t / 0.8f);
            //changeTo.transform.localScale = Vector3.Lerp(Vector3.zero, prevScale, t / 0.8f);
            yield return null;
        }
        for (int i = 0; i < newRenderer.materials.Length; i++) newRenderer.materials[i].SetFloat("_Cutoff", 0);
        //changeTo.transform.localScale = prevScale;
        for (int i = 0; i < newRenderer.materials.Length; i++) newRenderer.materials[i] = m_NormalMaterial;
        SetRendererEmission(changeTo, 1f);

        particles[particles.Length - 1].Stop();

        m_CurrentTip = changeTo;
        m_CurrentTip.SetActive(true); // just to be safe!

        m_Effects.OnWeaponTipChanged(m_CurrentTip);

        if (callbackOwner != null && doAfterwards != null)
            callbackOwner.StartCoroutine(doAfterwards);
    }

    public void Cancel()
    {

    }

    public bool CurrentWeaponIs(Tips tip)
    {
        if (tip == Tips.Axe)
            return m_CurrentTip == m_Axe;
        else if (tip == Tips.Crosswbow)
            return m_CurrentTip == m_Crossbow;
        else if (tip == Tips.Hammer)
            return m_CurrentTip == m_Hammer;
        else if (tip == Tips.Hellebarde)
            return m_CurrentTip == m_Hellebarde;
        else if (tip == Tips.Scythe)
            return m_CurrentTip == m_Scythe;
        else if (tip == Tips.Spear)
            return m_CurrentTip == m_Spear;
        else if (tip == Tips.Magic)
            return m_TipIsMagic;

        return false;
    }

}
