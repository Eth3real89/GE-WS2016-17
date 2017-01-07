using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : GenericSingletonClass<LevelManager>
{
    public enum ControlMode
    {
        Exploration = 0,
        Combat = 1
    }

    public ControlMode m_ControlMode = ControlMode.Exploration;
}
