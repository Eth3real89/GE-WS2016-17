using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SequencedActionCreator
{
    [System.Serializable]
    public class SequencedActionController : MonoBehaviour
    {
        public delegate void FinishedCutsceneCallback();

        public CameraTracking m_Tracking;
        public PlayerControls m_Controls;
        public bool m_LogTime;

        private PlayerMoveCommand m_MoveCommand;

        //List of all SequencedActions
        public List<SequencedAction> m_SequencedActions;
        private Dictionary<string, SequencedAction> m_SequencedActionMap;

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

        private void Start()
        {
            m_MoveCommand = FindObjectOfType<PlayerMoveCommand>();
            m_SequencedActionMap = new Dictionary<string, SequencedAction>();
            foreach (SequencedAction s in m_SequencedActions)
            {
                m_SequencedActionMap.Add(s.m_Name, s);
            }
        }

        public void PlayCutscene(string cutscene, FinishedCutsceneCallback callback = null)
        {
            if (!m_SequencedActionMap.ContainsKey(cutscene))
            {
                Debug.Log("Cutscene " + cutscene + " not found!");
                return;
            }

            BlockControl();
            ActionEvent[] actionEvents = m_SequencedActionMap[cutscene].m_ActionEvents.ToArray();
            float cutsceneDuration = 0;
            foreach (ActionEvent action in actionEvents)
            {
                cutsceneDuration = Mathf.Max(cutsceneDuration, action.m_StartTime + action.m_Duration);
                StartCoroutine(PlayActionEvent(action));
            }
            StartCoroutine(FinishCutscene(cutsceneDuration, callback));
        }

        IEnumerator FinishCutscene(float cutsceneDuration, FinishedCutsceneCallback callback)
        {
            float start = Time.time;
            float end = start + cutsceneDuration;

            while (Time.time < end)
            {
                if (m_LogTime)
                    Debug.Log(Time.time - start);
                yield return null;
            }

            if (callback != null)
                callback();
            m_Controls.EnableAndUnlock(m_MoveCommand);
            m_Tracking.m_BlockTracking = false;
        }

        IEnumerator PlayActionEvent(ActionEvent action)
        {
            yield return new WaitForSeconds(action.m_StartTime);
            if (action.m_AnimateTransform)
            {
                StartCoroutine(PlayTransformAnimation(action));
            }
            else
            {
                TriggerMethod(action);
            }
        }

        IEnumerator PlayTransformAnimation(ActionEvent action)
        {
            LerpTimer timer = new LerpTimer(action.m_Duration);
            timer.Start();

            while (timer.GetLerpProgress() < 1)
            {
                action.m_GameObject.transform.position =
                    Vector3.Lerp(action.m_StartTransform.m_Position, action.m_EndTransform.m_Position, timer.GetLerpProgress());
                action.m_GameObject.transform.rotation =
                    Quaternion.Lerp(Quaternion.Euler(action.m_StartTransform.m_Rotation),
                    Quaternion.Euler(action.m_EndTransform.m_Rotation), timer.GetLerpProgress());
                action.m_GameObject.transform.localScale =
                    Vector3.Lerp(action.m_StartTransform.m_Scale, action.m_EndTransform.m_Scale, timer.GetLerpProgress());
                yield return null;
            }
        }

        private void TriggerMethod(ActionEvent action)
        {
            Type thisType = action.m_ScriptObject.GetType();
            MethodInfo theMethod = thisType.GetMethod(action.m_MethodName);
            theMethod.Invoke(action.m_ScriptObject, null);
        }

        private void BlockControl()
        {
            m_Controls.DisableAndLock(m_MoveCommand);
            m_Controls.StopMoving();
            m_Tracking.m_BlockTracking = true;
        }

        public void CreateSequencedAction()
        {
            m_SequencedActions.Add(new SequencedAction());
        }
    }
}
