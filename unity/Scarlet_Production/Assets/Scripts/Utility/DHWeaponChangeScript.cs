using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DHWeaponChangeScript : MonoBehaviour {

    public Animator m_DHAnimator;

    public GameObject m_Rifle;
    public GameObject m_RifleContainerEquipped;
    public GameObject m_RifleContainerUnequipped;

    public GameObject m_PistolRight;
    public GameObject m_PistolLeft;
    public GameObject m_PistolRightContainerUnequipped;
    public GameObject m_PistolLeftContainerUnequipped;
    public GameObject m_PistolRightContainerEquipped;
    public GameObject m_PistolLeftContainerEquipped;


    public int m_CurrentlyEquipped = 1;

    private void Start()
    {
        TriggerEquipNothing();

        Time.timeScale = 0.4f;
    }

    void Update () {
	    
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TriggerEquipNothing();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TriggerEquipRifle();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TriggerEquipPistols();
        }
    }

    private void TriggerEquipNothing()
    {
        m_DHAnimator.SetInteger("Weapon", 1);

        if (m_CurrentlyEquipped == 1)
        {
            print("already equipped nothing!");
        }
        else if (m_CurrentlyEquipped == 2)
        {
            m_DHAnimator.SetTrigger("UnequipRifleTrigger");
        }
        else if (m_CurrentlyEquipped == 3)
        {
            m_DHAnimator.SetTrigger("UnequipPistolsTrigger");
        }
        m_CurrentlyEquipped = 1;
    }

    private void TriggerEquipRifle()
    {
        m_DHAnimator.SetInteger("Weapon", 2);

        if (m_CurrentlyEquipped == 1)
        {
            m_DHAnimator.SetTrigger("EquipRifleTrigger");
        }
        else if (m_CurrentlyEquipped == 2)
        {
            print("already equipped rifle!");
        }
        else if (m_CurrentlyEquipped == 3)
        {
            m_DHAnimator.SetTrigger("UnequipPistolsTrigger");
        }
        m_CurrentlyEquipped = 2;
    }

    private void TriggerEquipPistols()
    {
        m_DHAnimator.SetInteger("Weapon", 3);

        if (m_CurrentlyEquipped == 1)
        {
            m_DHAnimator.SetTrigger("EquipPistolsTrigger");
        }
        else if (m_CurrentlyEquipped == 2)
        {
            m_DHAnimator.SetTrigger("UnequipRifleTrigger");
        }
        else if (m_CurrentlyEquipped == 3)
        {
            print("already equipped pistols!");
        }
        m_CurrentlyEquipped = 3;
    }

    public void PistolsDrawn()
    {
        m_PistolLeft.transform.parent = m_PistolLeftContainerEquipped.transform;
        m_PistolRight.transform.parent = m_PistolRightContainerEquipped.transform;

        m_PistolLeft.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_PistolLeft.transform.localPosition = new Vector3(0, 0, 0);

        m_PistolRight.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_PistolRight.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void PistolsPutBack()
    {
        m_PistolLeft.transform.parent = m_PistolLeftContainerUnequipped.transform;
        m_PistolRight.transform.parent = m_PistolRightContainerUnequipped.transform;

        m_PistolLeft.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_PistolLeft.transform.localPosition = new Vector3(0, 0, 0);

        m_PistolRight.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_PistolRight.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void PistolsPutBackAnimationOver()
    {
        if (m_CurrentlyEquipped == 2)
        {
            m_DHAnimator.SetTrigger("EquipRifleTrigger");
        }
    }

    public void RifleDrawn()
    {
        if (m_CurrentlyEquipped != 2)
        {
            RiflePutBack();
            return;
        }

        m_Rifle.transform.parent = m_RifleContainerEquipped.transform;
        m_Rifle.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_Rifle.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void RiflePutBack()
    {
        m_Rifle.transform.parent = m_RifleContainerUnequipped.transform;
        m_Rifle.transform.localEulerAngles = new Vector3(0, 0, 0);
        m_Rifle.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void RiflePutBackAnimationOver()
    {
        if (m_CurrentlyEquipped == 3)
        {
            m_DHAnimator.SetTrigger("EquipPistolsTrigger");
        }
    }

}
