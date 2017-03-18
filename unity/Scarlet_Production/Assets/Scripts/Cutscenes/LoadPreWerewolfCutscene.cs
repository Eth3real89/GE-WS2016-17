using SequencedActionCreator;
using UnityEngine;

public class LoadPreWerewolfCutscene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            SequencedActionController.Instance.PlayCutscene("WerewolfTransformation");
    }
}
