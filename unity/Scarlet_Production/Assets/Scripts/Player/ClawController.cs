using UnityEngine;

public class ClawController : MonoBehaviour
{
    [System.Serializable]
    public struct HandModels
    {
        public GameObject left, right;

        public void SetActive(bool active)
        {
            left.SetActive(active);
            right.SetActive(active);
        }
    }

    public HandModels m_Hands, m_Claws;

    public void ShowHands()
    {
        m_Claws.SetActive(false);
        m_Hands.SetActive(true);
    }

    public void ShowClaws()
    {
        m_Hands.SetActive(false);
        m_Claws.SetActive(true);
    }
}
