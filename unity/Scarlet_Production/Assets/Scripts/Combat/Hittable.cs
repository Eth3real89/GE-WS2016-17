using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Hittable {
    void Hit(Damage damage);
    void RegisterInterject(HitInterject interject);
}
