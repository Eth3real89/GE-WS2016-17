using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStaggerCommand : BossCommand {

    public void DoStagger()
    {
        m_Animator.SetTrigger("StaggerTrigger");
    }

}
