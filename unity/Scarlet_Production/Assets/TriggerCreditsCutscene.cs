using SequencedActionCreator;
using UnityEngine;

public class TriggerCreditsCutscene : MonoBehaviour
{
    public GameObject earthAura, scarletAura;
    public Animation lightAnim;

    public void TriggerCutscene()
    {
        SequencedActionController.Instance.PlayCutscene("Credits");
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
