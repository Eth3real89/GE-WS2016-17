using SequencedActionCreator;
using UnityEngine;

public class TriggerCreditsCutscene : MonoBehaviour
{
    public GameObject earthAura, scarletAura, m_CageBreakEffect;
    public Animation lightAnim;

    public void TriggerCutscene()
    {
        SequencedActionController.Instance.PlayCutscene("Credits");

        // HERE!

        m_CageBreakEffect.SetActive(true);
    }

    public void EnabledEarthAura()
    {
        earthAura.SetActive(true);
    }

    public void EnabledScarletAura()
    {
        scarletAura.SetActive(true);
    }

    public void ChangeLight()
    {
        lightAnim.Play();
    }
}
