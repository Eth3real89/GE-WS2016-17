using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletOnExpDoNothing : BulletOnExpireBehaviour {

    public override void OnBulletExpires(BulletBehaviour b)
    {
        // do nothing :)
    }

}
