using UnityEngine;
using UnityEngine.Events;

public class WorldSwitcher : MonoBehaviour
{
    public delegate void SwitchWorldCallback();

    public GameObject m_RealWorld, m_ParallelWorld;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (other.tag == "Player")
        {
            EffectController.Instance.SwitchWorld(new SwitchWorldCallback(SwitchWorld));
            hasTriggered = true;
        }
    }

    public void SwitchWorld()
    {
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
