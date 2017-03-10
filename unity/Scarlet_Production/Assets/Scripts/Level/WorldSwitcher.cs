using UnityEngine;
using UnityEngine.Events;

public class WorldSwitcher : MonoBehaviour
{
    public delegate void SwitchWorldCallback();

    public GameObject m_RealWorld, m_ParallelWorld;

    private GameObject m_Player;
    private Transform m_Target;
    private DontGoThroughThings m_Blocker;

    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Blocker = m_Player.GetComponent<DontGoThroughThings>();
        m_Target = transform.GetChild(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            EffectController.Instance.SwitchWorld(new SwitchWorldCallback(SwitchWorld));
        }
    }

    public void SwitchWorld()
    {
        m_Player.transform.position = m_Target.position;
        m_Blocker.ResetPosition();

        if (m_RealWorld.activeSelf)
        {
            m_RealWorld.SetActive(false);
            m_ParallelWorld.SetActive(true);
        }
        else
        {
            m_RealWorld.SetActive(true);
            m_ParallelWorld.SetActive(false);
        }
    }
}
