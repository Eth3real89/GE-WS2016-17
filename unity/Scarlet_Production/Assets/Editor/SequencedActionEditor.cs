using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SequencedActionCreator
{
    public class SequencedActionEditor : EditorWindow
    {
        private SequencedActionController m_Controller;
        private SerializedObject m_SerializedController;
        private SerializedProperty m_SequencedActionList;

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
            // Select a SequencedAction Element
            m_SelectedSequencedAction = EditorGUILayout.Popup(m_SelectedSequencedAction, sequencedActions, GUILayout.Width(200));
            // Add a new SequencedAction Element
            if (GUILayout.Button("Add Sequenced Action", GUILayout.Width(200)))
            {
                m_SequencedActionList.arraySize++;
                m_SelectedSequencedAction = m_SequencedActionList.arraySize - 1;
            }
            GUILayout.EndHorizontal();
        }
    }
}