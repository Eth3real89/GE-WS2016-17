using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStaggerCommand : BossCommand {

    public void DoStagger(string trigger = "StaggerTrigger")
    {
        m_Animator.SetTrigger(trigger);
    }

}
