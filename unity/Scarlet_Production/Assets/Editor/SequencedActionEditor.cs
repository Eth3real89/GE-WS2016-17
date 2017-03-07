using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;
using System.Collections.Generic;

namespace SequencedActionCreator
{
    public class SequencedActionEditor : EditorWindow
    {
        private readonly static string m_DefaultSequencedActionName = "SequencedAction";

        private SequencedActionController m_Controller;
        private SerializedObject m_SerializedController;
        private SerializedProperty m_SequencedActionList;
        private Vector2 m_ScrollPos;

        private int m_SelectedSequencedAction;

        [MenuItem("Window/SequencedAction Editor")]
        static void Init()
        {
            SequencedActionEditor window = (SequencedActionEditor)GetWindow(typeof(SequencedActionEditor));
            window.minSize = new Vector2(600, 600);
            window.Show();
        }

        private void OnEnable()
        {
            // Find the SequencedActionController in the Scene or create a new one if it doesn't exist
            SequencedActionController[] controllers = (SequencedActionController[])Resources.FindObjectsOfTypeAll(typeof(SequencedActionController));
            if (controllers.Length > 0)
                m_Controller = controllers[0];
            else
            {
                GameObject controllerHolder = new GameObject("SequencedActionController");
                m_Controller = controllerHolder.AddComponent<SequencedActionController>() as SequencedActionController;
            }

            m_SerializedController = new SerializedObject(m_Controller);
            m_SequencedActionList = m_SerializedController.FindProperty("m_SequencedActions");
        }

        void OnGUI()
        {
            m_SerializedController.Update();
            GUI.changed = false;

            BuildSequencedActionSelection();
            GUILayout.Space(15);
            BuildActions();
            BuildAddActionButton();

            if (GUI.changed)
            {
                // Mark the scene as changed when values are modified
                m_SerializedController.ApplyModifiedProperties();
                EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }

        private void BuildSequencedActionSelection()
        {
            // Get a List of all SequencedActions
            string[] sequencedActions = new string[m_SequencedActionList.arraySize];
            for (int i = 0; i <= m_SequencedActionList.arraySize - 1; i++)
            {
                SerializedProperty action = m_SequencedActionList.GetArrayElementAtIndex(i);
                sequencedActions[i] = action.FindPropertyRelative("m_Name").stringValue;
            }
            GUILayout.BeginHorizontal();
            // Select a SequencedAction element
            m_SelectedSequencedAction = EditorGUILayout.Popup(m_SelectedSequencedAction, sequencedActions, GUILayout.Width(200));
            // Add a new SequencedAction element
            if (GUILayout.Button("Add Sequenced Action", GUILayout.Width(200)))
            {
                m_SequencedActionList.arraySize++;
                m_SelectedSequencedAction = m_SequencedActionList.arraySize - 1;
                // Set a default name for the new element
                m_SequencedActionList.GetArrayElementAtIndex(m_SelectedSequencedAction).FindPropertyRelative("m_Name").stringValue =
                    m_DefaultSequencedActionName + m_SelectedSequencedAction;
            }

            if (m_SequencedActionList.arraySize > m_SelectedSequencedAction)
            {
                // Display the TextField to change the name of the SequencedAction
                SerializedProperty currentSequencedAction = m_SequencedActionList.GetArrayElementAtIndex(m_SelectedSequencedAction);
                currentSequencedAction.FindPropertyRelative("m_Name").stringValue =
                    EditorGUILayout.TextField(currentSequencedAction.FindPropertyRelative("m_Name").stringValue, GUILayout.Width(200));
            }

            GUILayout.EndHorizontal();
        }

        private void BuildActions()
        {
            if (m_SequencedActionList.arraySize <= m_SelectedSequencedAction)
            {
                return;
            }

            SerializedProperty actions = m_SequencedActionList.GetArrayElementAtIndex(m_SelectedSequencedAction).FindPropertyRelative("m_ActionEvents");

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            for (int i = 0; i < actions.arraySize; i++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                BuildActionSettings(actions.GetArrayElementAtIndex(i));

                // Buttons to move or delete actions
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                //if (i == 0)
                //    GUI.enabled = false;
                //if (GUILayout.Button("Move up", GUILayout.Width(150)))
                //{
                //    UnityEngine.Object temp = actions.GetArrayElementAtIndex(i - 1).objectReferenceValue;
                //    actions.GetArrayElementAtIndex(i - 1).objectReferenceValue = actions.GetArrayElementAtIndex(i).objectReferenceValue;
                //    actions.GetArrayElementAtIndex(i).objectReferenceValue = temp;
                //}
                //GUI.enabled = true;
                //if (i == actions.arraySize - 1)
                //    GUI.enabled = false;
                //if (GUILayout.Button("Move down", GUILayout.Width(150)))
                //{
                //    UnityEngine.Object temp = actions.GetArrayElementAtIndex(i + 1).objectReferenceValue as UnityEngine.Object;
                //    actions.GetArrayElementAtIndex(i + 1).objectReferenceValue = actions.GetArrayElementAtIndex(i).objectReferenceValue;
                //    actions.GetArrayElementAtIndex(i).objectReferenceValue = temp;
                //}
                //GUI.enabled = true;
                if (GUILayout.Button("Delete Action", GUILayout.Width(150)))
                {
                    actions.DeleteArrayElementAtIndex(i);
                    i--;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();

            GUILayout.Space(15);
        }

        private void BuildActionSettings(SerializedProperty action)
        {
            EditorGUILayout.BeginHorizontal();
            bool shouldAnimateTransform = ShouldAnimateTransformToggle(action);
            BuildTimeSettings(action, shouldAnimateTransform);
            EditorGUILayout.EndHorizontal();

            if (shouldAnimateTransform)
                BuildTransformAnimationSettings(action);
            else
                BuildFunctionSettings(action);

        }

        private void BuildFunctionSettings(SerializedProperty action)
        {
            // Selection for the method to be called
            EditorGUILayout.BeginHorizontal();
            action.FindPropertyRelative("m_GameObject").objectReferenceValue =
                (GameObject)EditorGUILayout.ObjectField("GameObject:", (GameObject)action.FindPropertyRelative("m_GameObject").objectReferenceValue,
                typeof(GameObject), true, GUILayout.Width(350));
            GUILayout.Space(10);

            GameObject c_GameObject = (GameObject)action.FindPropertyRelative("m_GameObject").objectReferenceValue;

            if (c_GameObject != null)
            {
                List<string> methods = new List<string>();
                List<string> mbNames = new List<string>();

                MonoBehaviour monoBehaviour = (MonoBehaviour)action.FindPropertyRelative("m_ScriptObject").objectReferenceValue;
                int s_MonoBehaviour = 0, s_Method = 0;

                // Find all scripts (MonoBehaviours) which are attached to the selected GameObject
                MonoBehaviour[] mbs = c_GameObject.GetComponents<MonoBehaviour>();
                for (int i = 0; i < mbs.Length; i++)
                {
                    MonoBehaviour mb = mbs[i];
                    mbNames.Add(mb.GetType().Name);

                    if (monoBehaviour != null && mb.GetType().Name == monoBehaviour.GetType().Name)
                        s_MonoBehaviour = i;
                }
                int s_MonoBehaviourTemp = EditorGUILayout.Popup("MonoBehaviour:", s_MonoBehaviour, mbNames.ToArray(), GUILayout.Width(350));
                GUILayout.Space(10);

                // When the MonoBehaviour has been changed, reset the selected method to avoid a nullpointer
                if (s_MonoBehaviour != s_MonoBehaviourTemp)
                {
                    s_Method = 0;
                    s_MonoBehaviour = s_MonoBehaviourTemp;
                }
                if (mbs.Length > s_MonoBehaviour)
                    action.FindPropertyRelative("m_ScriptObject").objectReferenceValue = mbs[s_MonoBehaviour];
                else
                    action.FindPropertyRelative("m_ScriptObject").objectReferenceValue = null;

                // Find all methods in the selected MonoBehaviour
                BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

                if (monoBehaviour != null)
                {
                    MethodInfo[] methodInfos = monoBehaviour.GetType().GetMethods(flags);

                    for (int i = 0; i < methodInfos.Length; i++)
                    {
                        MethodInfo info = methodInfos[i];
                        methods.Add(info.Name);

                        if (info.Name == action.FindPropertyRelative("m_MethodName").stringValue)
                            s_Method = i;
                    }
                }
                s_Method = EditorGUILayout.Popup("Method:", s_Method, methods.ToArray(), GUILayout.Width(350));

                if (methods.Count > s_Method)
                    action.FindPropertyRelative("m_MethodName").stringValue = methods[s_Method];
                else
                    action.FindPropertyRelative("m_MethodName").stringValue = null;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void BuildTransformAnimationSettings(SerializedProperty action)
        {
            EditorGUILayout.BeginHorizontal();
            TransformValues(action.FindPropertyRelative("m_StartTransform"), "Start");
            GUILayout.Space(20);
            TransformValues(action.FindPropertyRelative("m_EndTransform"), "End");
            EditorGUILayout.EndVertical();
        }

        private void TransformValues(SerializedProperty transform, string label)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(400));
            transform.FindPropertyRelative("m_Position").vector3Value =
                EditorGUILayout.Vector3Field(label + " Position:", transform.FindPropertyRelative("m_Position").vector3Value);
            transform.FindPropertyRelative("m_Rotation").vector3Value =
                EditorGUILayout.Vector3Field(label + " Position:", transform.FindPropertyRelative("m_Rotation").vector3Value);
            transform.FindPropertyRelative("m_Scale").vector3Value =
                EditorGUILayout.Vector3Field(label + " Position:", transform.FindPropertyRelative("m_Scale").vector3Value);
            TransformShortcut(transform);
            EditorGUILayout.EndHorizontal();
        }

        private void TransformShortcut(SerializedProperty transform)
        {
            GUILayout.Space(5);
            Transform t = EditorGUILayout.ObjectField("Use Transform Values", null, typeof(Transform), true, GUILayout.Width(400)) as Transform;
            if (t == null)
                return;
            transform.FindPropertyRelative("m_Position").vector3Value = t.position;
            transform.FindPropertyRelative("m_Rotation").vector3Value = t.rotation.eulerAngles;
            transform.FindPropertyRelative("m_Scale").vector3Value = t.localScale;
        }

        private void BuildTimeSettings(SerializedProperty action, bool durationRequired)
        {
            // Time settings for the action
            action.FindPropertyRelative("m_StartTime").floatValue = EditorGUILayout.FloatField("Start Time:",
                action.FindPropertyRelative("m_StartTime").floatValue, GUILayout.Width(350));
            GUILayout.Space(10);
            GUI.enabled = durationRequired;
            action.FindPropertyRelative("m_Duration").floatValue = EditorGUILayout.FloatField("Duration:",
                action.FindPropertyRelative("m_Duration").floatValue, GUILayout.Width(350));
            GUI.enabled = true;
        }

        private bool ShouldAnimateTransformToggle(SerializedProperty action)
        {
            // Toggle for the actionType
            GUILayout.Space(10);
            action.FindPropertyRelative("m_AnimateTransform").boolValue = EditorGUILayout.Toggle("Animate Transform",
                action.FindPropertyRelative("m_AnimateTransform").boolValue, GUILayout.Width(200));
            return action.FindPropertyRelative("m_AnimateTransform").boolValue;
        }

        private void BuildAddActionButton()
        {
            if (m_SequencedActionList.arraySize <= m_SelectedSequencedAction)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Add new Action", GUILayout.Width(100)))
            {
                m_SequencedActionList.GetArrayElementAtIndex(m_SelectedSequencedAction).FindPropertyRelative("m_ActionEvents").arraySize++;
            }
            GUI.enabled = true;
        }

    }
}