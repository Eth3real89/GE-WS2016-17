using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : AEAttackDamage {

    public bool m_Deflectable;
    public GameObject m_DeflectTarget;
    public bool m_Deflected = false;
}
