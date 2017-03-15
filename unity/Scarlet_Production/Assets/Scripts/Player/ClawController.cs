using UnityEngine;

public class ClawController : MonoBehaviour
{
    [System.Serializable]
    public struct HandModels
    {
        public GameObject left, right;
        public float m_Time;

        public void SetActive(bool active)
        {
            left.SetActive(active);
            right.SetActive(active);
        }

        //public void FadeIn()
        //{
        //    while((m_Time -= Time.deltaTime) > 0)
        //    {
        //        left.GetComponent<Renderer>().material.SetFloat("_Cutoff", m_Time);
        //        right.GetComponent<Renderer>().material.SetFloat("_Cutoff", m_Time);
        //    }
        //    left.GetComponent<Renderer>().material.SetFloat("_Cutoff", 0);
        //    right.GetComponent<Renderer>().material.SetFloat("_Cutoff", 0);
        //    m_Time = 1f;
        //}

        //public void FadeOut()
        //{
        //    while ((m_Time += Time.deltaTime) < 1f)
        //    {
        //        left.GetComponent<Renderer>().material.SetFloat("_Cutoff", m_Time);
        //        right.GetComponent<Renderer>().material.SetFloat("_Cutoff", m_Time);
        //    }
        //    left.GetComponent<Renderer>().material.SetFloat("_Cutoff", 1f);
        //    right.GetComponent<Renderer>().material.SetFloat("_Cutoff", 1f);
        //    m_Time = 1f;
        //}
    }

    public HandModels m_Hands, m_Claws;

    public void ShowHands()
    {
        //m_Claws.FadeOut();
        m_Claws.SetActive(false);
        m_Hands.SetActive(true);
        //m_Hands.FadeIn();
    }

    public void ShowClaws()
    {
        //m_Hands.FadeOut();
        m_Hands.SetActive(false);
        m_Claws.SetActive(true);
        //m_Claws.FadeIn();
    }
}
