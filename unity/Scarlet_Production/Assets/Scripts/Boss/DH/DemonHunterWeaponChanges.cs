using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterWeaponChanges : MonoBehaviour {

    public GameObject m_Rifle;
    public GameObject m_RifleContainerEquipped;
    public GameObject m_RifleContainerUnequipped;

    public GameObject m_PistolRight;
    public GameObject m_PistolLeft;
    public GameObject m_PistolRightContainerUnequipped;
    public GameObject m_PistolLeftContainerUnequipped;
    public GameObject m_PistolRightContainerEquipped;
    public GameObject m_PistolLeftContainerEquipped;

    public int m_CurrentlyEquipped = 0;

    protected virtual void Start()
    {
        if (m_CurrentlyEquipped == 0)
        {
            if (m_PistolRight.transform.parent == m_PistolRightContainerEquipped)
            {
                PistolsPutAwayAnimEvent();
            }

            if (m_Rifle.transform.parent == m_RifleContainerEquipped)
            {
                RiflePutAwayAnimEvent();
            }
        }

    }

    public void PistolsPutAwayAnimEvent()
    {
        m_PistolLeft.transform.parent = m_PistolLeftContainerUnequipped.transform;
        m_PistolRight.transform.parent = m_PistolRightContainerUnequipped.transform;

        m_PistolLeft.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_PistolLeft.transform.localPosition = new Vector3(0, 0, 0);

        m_PistolRight.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_PistolRight.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void PistolsDrawnAnimEvent()
    {
        m_PistolLeft.transform.parent = m_PistolLeftContainerEquipped.transform;
        m_PistolRight.transform.parent = m_PistolRightContainerEquipped.transform;

        m_PistolLeft.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_PistolLeft.transform.localPosition = new Vector3(0, 0, 0);

        m_PistolRight.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_PistolRight.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void RiflePutAwayAnimEvent()
    {
        m_Rifle.transform.parent = m_RifleContainerUnequipped.transform;
        m_Rifle.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_Rifle.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void RifleDrawnAnimEvent()
    {
        m_Rifle.transform.parent = m_RifleContainerEquipped.transform;
        m_Rifle.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_Rifle.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void GrenadeTakenRight()
    {

    }

    public void GrenadeThrownRight()
    {

    }

    public void GrenadeTakenLeft()
    {

    }

    public void GrenadeThrownLeft()
    {

    }

    public void GrenadesThrowGroundAnimEvent()
    {

    }

}
