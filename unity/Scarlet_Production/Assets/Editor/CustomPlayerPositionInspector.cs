using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(PlayerPosition))]
public class CustomPlayerPositionInspector : Editor
{
    private PlayerPosition m_PlayerPosition;
    private int m_Selected = 0;

    private void OnEnable()
    {
        m_PlayerPosition = (PlayerPosition)target;
    }

    public override void OnInspectorGUI()
    {
        m_PlayerPosition.Update();
        List<Transform> startingPoints = m_PlayerPosition.m_StartingPoint;

        string[] options = new string[startingPoints.Count];
        for (int i = 0; i < startingPoints.Count; i++)
        {
            options[i] = startingPoints[i].name;
        }
        GUILayout.Label("Current starting point:");
        GUILayout.Space(10);
        m_Selected = EditorGUILayout.Popup(m_Selected, options, GUILayout.Width(250));
        m_PlayerPosition.m_StartAtPoint = m_Selected;

        GUILayout.Space(15);
        GUILayout.Label("Starting points:");
        GUILayout.Space(10);

        for (int i = 0; i < startingPoints.Count; i++)
        {
            GUILayout.BeginHorizontal();
            startingPoints[i] = (Transform)EditorGUILayout.ObjectField(startingPoints[i], typeof(Transform), true, GUILayout.Width(250));
            if (i > 0)
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUILayout.Width(20), GUILayout.Height(15)))
                {
                    startingPoints.RemoveAt(i);
                }
            }
            GUILayout.EndVertical();
        }

        if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), GUILayout.Width(250)))
        {
            startingPoints.Add(startingPoints[startingPoints.Count - 1]);
        }
    }
}
