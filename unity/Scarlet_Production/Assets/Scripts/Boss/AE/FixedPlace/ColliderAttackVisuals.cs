using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderAttackVisuals : FixedPlaceAttackVisuals {

    public Material m_MaterialDuringAttack;

    public override void ShowAttack()
    {
        base.ShowAttack();
 
       

        Renderer r = GetComponentInChildren<Renderer>();
        r.material = this.m_MaterialDuringAttack;
    }

    public override void HideAttack()
    {
        base.HideAttack();
        gameObject.SetActive(false);
    }

}
