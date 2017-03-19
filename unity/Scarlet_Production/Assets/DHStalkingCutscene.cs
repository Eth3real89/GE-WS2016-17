using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DHStalkingCutscene : MonoBehaviour
{
    public void PlaySoundEffect()
    {
        new FARQ().ClipName("woods").Location(Camera.main.transform).Play();
    }
}
