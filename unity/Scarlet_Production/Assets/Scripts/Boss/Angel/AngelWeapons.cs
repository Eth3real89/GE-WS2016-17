using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelWeapons : MonoBehaviour {

    public enum Tips {Axe, Crosswbow, Hammer, Hellebarde, Scythe, Spear};

    public GameObject m_Axe;
    public GameObject m_Crossbow;
    public GameObject m_Hammer;
    public GameObject m_Hellebarde;
    public GameObject m_Scythe;
    public GameObject m_Spear;

    protected GameObject m_CurrentTip;

    protected IEnumerator m_WeaponChangeEnumerator;

    public void ChangeTipTo(Tips t, IEnumerator doAfterwards, MonoBehaviour callbackOwner)
    {
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
            if (callbackOwner != null && doAfterwards != null)
                callbackOwner.StartCoroutine(doAfterwards);
        }
        else
        {
            m_WeaponChangeEnumerator = ChangeTipRoutine(tip, doAfterwards, callbackOwner);
            StartCoroutine(m_WeaponChangeEnumerator);
        }
    }

    protected virtual IEnumerator ChangeTipRoutine(GameObject changeTo, IEnumerator doAfterwards, MonoBehaviour callbackOwner)
    {
        yield return null;

        if (callbackOwner != null && doAfterwards != null)
            callbackOwner.StartCoroutine(doAfterwards);

        if (m_CurrentTip != null)
            m_CurrentTip.SetActive(false);

        Vector3 prevScale = changeTo.transform.localScale + new Vector3();

        changeTo.SetActive(true);
        float t = 0;
        while((t += Time.deltaTime) < 0.8f)
        {
            changeTo.transform.localScale = Vector3.Lerp(Vector3.zero, prevScale, t / 0.8f);
        }
        changeTo.transform.localScale = prevScale;

        m_CurrentTip = changeTo;
    }

    public void Cancel()
    {

    }

}
