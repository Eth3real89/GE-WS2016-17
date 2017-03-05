using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequencedActionCreator
{
    [System.Serializable]
    public class ActionEvent
    {
        public float m_StartTime, m_Duration;
        public GameObject m_GameObject;
        public Object m_ScriptObject;
        public string m_MethodName;
    }
}