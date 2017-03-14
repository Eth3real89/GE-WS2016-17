using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEFairyBulletHintEffect : PlayableEffect
{

    public override void Play(Vector3 position = default(Vector3))
    {
        print("Playing bullet hint!");
    }

    public override void Hide()
    {
    }
    
}
