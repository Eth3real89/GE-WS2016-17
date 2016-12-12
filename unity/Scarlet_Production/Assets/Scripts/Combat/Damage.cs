using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damage : MonoBehaviour {

    // these are methods because they will get parameters later! (blockable by which kind of parry etc.)

    public abstract bool blockable();
    public abstract float damage();

}
