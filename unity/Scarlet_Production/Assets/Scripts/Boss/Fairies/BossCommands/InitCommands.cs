using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// You don't have to set values for Commands on an object if you set them
/// here and add it to the same object.
/// Careful: That means you have to use the commands' "InitCommand" method for startup
/// rather than their start method!
/// </summary>
public class InitCommands : MonoBehaviour {

    public GameObject m_Boss;
    public Animator m_BossAnimator;

	void Start ()
    {
		foreach(BossCommand command in GetComponents<BossCommand>())
        {
            command.InitCommand(m_Boss, m_BossAnimator);
        }
	}
}
