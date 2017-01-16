using UnityEngine;
using System.Collections;

public class OnCollectibleVFX : MonoBehaviour {

    public Material m_Mat = null;

    [Range(0.01f, 0.2f)] public float m_DarkRange = 0.1f;
    [Range(-2.5f, -1f)] public float m_Distortion = -2f;
    [Range(0.05f, 0.3f)] public float m_Form = 0.2f;

    private bool m_IsActive;
    private Vector4 m_ObjectPosition;

    // Use this for initialization
    void Start () {
        m_Mat.SetFloat ("_DarkRange", m_DarkRange);
        m_Mat.SetFloat ("_Distortion", m_Distortion);
        m_Mat.SetFloat ("_Form", m_Form);

        m_IsActive = false;
    }

    void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if(m_IsActive)
        {
            m_Mat.SetVector ("_Center", m_ObjectPosition);
            Graphics.Blit (sourceTexture, destTexture, m_Mat);
        }
        else
            Graphics.Blit (sourceTexture, destTexture);
    }

    public void Activate(Vector3 objectPosition)
    {
        m_IsActive = true;
        objectPosition = ToUV(objectPosition);
        m_ObjectPosition = new Vector4(objectPosition.x, objectPosition.y, objectPosition.z, 0f);
    }

    public void Deactivate() 
    {
        m_IsActive = false;
    }

    public Vector3 ToUV(Vector3 src)
    {
        src.x /= Screen.width;
        src.y /= Screen.height;

        return src;
    }

    // Update is called once per frame
    void Update () {
    
    }
}
