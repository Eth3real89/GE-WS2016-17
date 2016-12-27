using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitSignal : MonoBehaviour {

    /// <summary>
    /// @Todo better!! (this should be given to "OnHit", i.e. attacks should probably have an Attacker member.
    /// </summary>
    public GameObject m_Scarlet;

    public GameObject m_RendererContainer;

    public GameObject m_BloodTrailPrefab;
    public GameObject m_BloodTrailPlaneCollider;

    public float m_InitialBloodShapeRadius;
    public float m_TimeBleedAfterHit;

    public Material m_HitMaterial;
    private IEnumerator m_ColorChangeEnumerator;
    private IDictionary<Renderer, Material> m_OriginalMaterialDictionary;

    private void Start()
    {
        InitOriginalColorDictionary();
    }

    private void InitOriginalColorDictionary()
    {
        m_OriginalMaterialDictionary = new System.Collections.Generic.Dictionary<Renderer, Material>();
        foreach (Renderer r in m_RendererContainer.GetComponentsInChildren<Renderer>())
        {
            m_OriginalMaterialDictionary.Add(r, r.material);
        }
    }

    public void OnHit()
    {
        InitBloodTrail(m_Scarlet);
        ColorBossRed();
    }

    private void ColorBossRed()
    {
        foreach (Renderer r in m_RendererContainer.GetComponentsInChildren<Renderer>())
        {
            r.material = m_HitMaterial;
        }

        if (m_ColorChangeEnumerator != null)
        {
            StopCoroutine(m_ColorChangeEnumerator);
        }
        m_ColorChangeEnumerator = RestoreColors();
        StartCoroutine(m_ColorChangeEnumerator);
    }

    public IEnumerator RestoreColors()
    {
        yield return new WaitForSeconds(0.3f);

        foreach (Renderer r in m_RendererContainer.GetComponentsInChildren<Renderer>())
        {
            if (m_OriginalMaterialDictionary.ContainsKey(r))
            {
                r.material = m_OriginalMaterialDictionary[r];
            }
        }
    }

    private void InitBloodTrail(GameObject attacker)
    {
        GameObject bloodTrailWrapper = (GameObject)Instantiate(m_BloodTrailPrefab, transform.position,
            attacker.transform.rotation);
        Transform bloodTrail = bloodTrailWrapper.transform.FindChild("BloodTrail");
        Transform bloodPuddle = bloodTrail.transform.FindChild("BloodPuddle");

        ParticleSystem ps = bloodTrail.GetComponent<ParticleSystem>();
        ParticleSystem ps2 = bloodPuddle.GetComponent<ParticleSystem>();
        //ps.startSpeed = m_InitialBloodSpeed;

        var emission = ps.emission;
        // emission.rate = m_InitialBloodEmissionRate;

        var shape = ps.shape;
        shape.radius = m_InitialBloodShapeRadius;

        ps.collision.SetPlane(0, m_BloodTrailPlaneCollider.transform);

        StartCoroutine(RemoveBloodTrail(bloodTrailWrapper, ps, ps2));
    }


    private IEnumerator RemoveBloodTrail(GameObject bloodTrail, ParticleSystem ps, ParticleSystem ps2)
    {
        float time = 0;
        float endTime = time + m_TimeBleedAfterHit;

        var emission = ps.emission;
        var shape = ps.shape;

        while (time < endTime)
        {
            bloodTrail.transform.position = transform.position;

            float level = time / endTime;
            shape.radius = Mathf.Lerp(m_InitialBloodShapeRadius, 0.05f, level);

            time += Time.deltaTime;
            yield return null;
        }

        emission.rate = 0;

        emission.SetBursts(new ParticleSystem.Burst[] { });

        while (ps2.particleCount > 0)
        {
            yield return null;
        }

        GameObject.Destroy(bloodTrail); // removes all blood instantly
    }

}
