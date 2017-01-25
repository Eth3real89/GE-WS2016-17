using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletOnExpireBehaviour : MonoBehaviour {

    public abstract void OnBulletExpires(BulletBehaviour b);

}
