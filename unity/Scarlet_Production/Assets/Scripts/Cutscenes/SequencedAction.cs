using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequencedActionCreator
{
    [System.Serializable]
    public class SequencedAction
    {
        public string m_Name;
        public List<ActionEvent> m_ActionEvents;
    }
}