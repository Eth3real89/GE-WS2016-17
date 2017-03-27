using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorFairyOnHitSignal : OnHitSignal {

    protected override void InitBloodTrail(GameObject attacker)
    {
        GameObject sparks = (GameObject)Instantiate(m_BloodTrailPrefab, transform.position,
            transform.rotation, transform);
        sparks.SetActive(true);
        
        foreach (ParticleSystem ps in sparks.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }

        StartCoroutine(RemoveSparks(sparks));
    }

    protected IEnumerator RemoveSparks(GameObject sparks)
    {
        yield return new WaitForSeconds(m_TimeBleedAfterHit);

        if (sparks != null)
        {
            Destroy(sparks);
        }
    }

}
