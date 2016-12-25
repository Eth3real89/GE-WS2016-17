using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class DrawPath : MonoBehaviour
{
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.childCount > i + 1)
            {
                Debug.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
            }
        }
    }
}