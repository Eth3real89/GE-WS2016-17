using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEFairyBlastWaveHintEffect : PlayableEffect
{

    public override void Play(Vector3 position = default(Vector3))
    {
        print("Playing wave hint!");
    }

    public override void Hide()
    {
    }

}
