using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequencedActionCreator
{
    [System.Serializable]
    public class SequencedActionController : MonoBehaviour
    {
        //List of all SequencedActions
        public List<SequencedAction> m_SequencedActions;

        #region "Singleton"
        public static SequencedActionController Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }
        #endregion

        private void Start()
        {
        }

        #region "Manage SequencedActions"
        public void CreateSequencedAction()
        {
            m_SequencedActions.Add(new SequencedAction());
        }
        #endregion

        #region "Control SequencedActions"
        #endregion
    }
}
