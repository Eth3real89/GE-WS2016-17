using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletNoExpiration : BulletExpirationBehaviour {
    public override void CancelBehaviour(BulletBehaviour b)
    {
        // also do nothing
    }

    public override void OnLaunch(BulletBehaviour b)
    {
        // do nothing
    }
}
