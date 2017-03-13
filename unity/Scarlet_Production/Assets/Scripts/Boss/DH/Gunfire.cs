using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunfire : MonoBehaviour {

    public GameObject m_PistolShotLeft;
    public GameObject m_PistolShotRight;
    public GameObject m_RifleShot;

    protected IEnumerator m_EffectEnumerator;

    public void FireGun(int which)
    {
    /*
        if (which == 0)
        {
            CopyAndPlay(m_PistolShotLeft);
        }
        else if (which == 1)
        {
            CopyAndPlay(m_PistolShotRight);
        }
        else
        {
            CopyAndPlay(m_PistolShotLeft);
            CopyAndPlay(m_PistolShotRight);
        }
    */
        if (which == 0)
        {
            ShootPistol(m_PistolShotLeft);
        }
        else if (which == 1)
        {
            ShootPistol(m_PistolShotRight);
        }
        else
        {
            ShootPistol(m_PistolShotLeft);
            ShootPistol(m_PistolShotRight);
        }
    }

    private void ShootPistol(GameObject obj)
    {   
        SafetyDisable(obj);

        obj.GetComponentInChildren<ParticleSystem>().Play();
        obj.GetComponentInChildren<Light>().enabled = true;

        StartCoroutine(HideMuzzleFlashLight(obj));
        StartCoroutine(HideMuzzleFlashEffect(obj));
    }

    private void SafetyDisable(GameObject obj)
    {
        obj.GetComponentInChildren<ParticleSystem>().Stop();
        obj.GetComponentInChildren<Light>().enabled = false;
    }

    private IEnumerator HideMuzzleFlashLight(GameObject obj)
    {
        yield return new WaitForSeconds(0.05f);

        obj.GetComponentInChildren<Light>().enabled = false;
    }

    private IEnumerator HideMuzzleFlashEffect(GameObject obj)
    {
        yield return new WaitForSeconds(0.4f);

        obj.GetComponentInChildren<ParticleSystem>().Stop();
    }
    /*
    protected void CopyAndPlay(GameObject obj)
    {
       // return;

        if (m_EffectEnumerator != null)
            return;

        GameObject copy = Instantiate(obj, this.transform);
        copy.SetActive(true);
        m_EffectEnumerator = DeleteAfter(copy);

        try
        {
            ParticleSystem p = copy.GetComponentInChildren<ParticleSystem>();
            var x = p.main;
            x.startRotationY = transform.rotation.eulerAngles.y;
            p.Play();
        }catch { }

        StartCoroutine(m_EffectEnumerator);
    }
    
    protected IEnumerator DeleteAfter(GameObject copy)
    {
        yield return new WaitForSeconds(0.4f);
        Destroy(copy);
        m_EffectEnumerator = null;
    }
    */

    public void FireRifle()
    {
        ShootRifle(m_RifleShot);
    }

    private void ShootRifle(GameObject obj)
    {
        SafetyDisableRifle(obj);

        Component[] pss = obj.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem ps in pss)
        {
            ps.Play();
        }

        obj.GetComponentInChildren<Light>().enabled = true;

        StartCoroutine(HideMuzzleFlashLight(obj));
        StartCoroutine(HideMuzzleFlashEffectRifle(obj));
    }

    private void SafetyDisableRifle(GameObject obj)
    {
        obj.GetComponentInChildren<Light>().enabled = false;
        StopRifleEffects(obj);       
    }

    private IEnumerator HideMuzzleFlashEffectRifle(GameObject obj)
    {
        yield return new WaitForSeconds(0.4f);

        StopRifleEffects(obj);
    }

    private void StopRifleEffects(GameObject obj)
    {
        Component[] pss = obj.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem ps in pss)
        {
            ps.Stop();
        }
    }
}
