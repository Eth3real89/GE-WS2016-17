using SequencedActionCreator;
using UnityEngine;

public class StartWerewolfCutscene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SequencedActionController.Instance.PlayCutscene("WerewolfLowerBridge");
            Destroy(gameObject);
        }
    }
}
