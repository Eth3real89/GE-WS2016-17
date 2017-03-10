using System.Collections;
using UnityEngine;

public class HealthPotionVisuals : MonoBehaviour, PlayerHealCommand.NumPotionListener {

    //public ParticleSystem m_ImagePrefab;
    public GameObject m_ChargesPrefab;

    private ParticleSystem[] m_Charges;

    public void OnNumberOfPotionsUpdated(int num)
    {
        if (m_ChargesPrefab == null)
            return;

        ClearExistingCharges();
        m_Charges = new ParticleSystem[num];

        ShowPotionIcons(num);
    }

    private void ShowPotionIcons(int num)
    {
        ParticleSystem[] existingCharges = m_ChargesPrefab.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < existingCharges.Length && i < num; i++)
        {
            m_Charges[i] = existingCharges[i];
            existingCharges[i].Play();
        }
    }

    private void ClearExistingCharges()
    {
        if (m_Charges != null)
        {
            for(int i = 0; i < m_Charges.Length; i++)
            {
                if (i == m_Charges.Length - 1)
                {
                    m_Charges[i].Emit(250) ;  
                }

                m_Charges[i].Stop() ;    
            }           
        }
    }
}