using SequencedActionCreator;
using UnityEngine;

public class StartCutscene : MonoBehaviour
{
    public string m_Cutscene;

    private void OnTriggerEnter(Collider other)
    {
        SequencedActionController.Instance.PlayCutscene(m_Cutscene);
        Destroy(this);
    }
}
