using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunfire : MonoBehaviour {

    public GameObject m_PistolShotLeft;
    public GameObject m_PistolShotRight;

    protected IEnumerator m_EffectEnumerator;
    
    public void FireGun(int which)
    {
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
    }

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
        yield return new WaitForSeconds(0.5f);
        Destroy(copy);
        m_EffectEnumerator = null;
    }

    public void FireRifle()
    {

    }

}
