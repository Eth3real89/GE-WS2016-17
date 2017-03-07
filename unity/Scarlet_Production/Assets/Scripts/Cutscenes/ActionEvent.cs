using UnityEngine;

namespace SequencedActionCreator
{
    [System.Serializable]
    public class ActionEvent
    {
        public float m_StartTime, m_Duration;
        public bool m_AnimateTransform;
        public CustomTransform m_StartTransform, m_EndTransform;
        public GameObject m_GameObject;
        public Object m_ScriptObject;
        public string m_MethodName;
    }

    [System.Serializable]
    public class CustomTransform
    {
        public Vector3 m_Position;
        public Vector3 m_Rotation;
        public Vector3 m_Scale;
    }
}