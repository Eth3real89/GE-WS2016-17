using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour {

    public bool m_Active;
    public float m_Damage;

    private void OnTriggerEnter(Collider other)
    {
        if (m_Active)
        {
            // @todo: check if other is boss
            // @todo: deal damage
            // @todo: then, set active to false.
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // same as above (make that 1 method)
    }

}
