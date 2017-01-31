using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletExpirationBehaviour : MonoBehaviour {

    public abstract void OnLaunch(BulletBehaviour b);
    public abstract void CancelBehaviour(BulletBehaviour b);

}
