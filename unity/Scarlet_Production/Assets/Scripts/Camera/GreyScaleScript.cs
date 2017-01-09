using UnityEngine;
using System.Collections;

public class GreyScaleScript : MonoBehaviour
{

    public Texture m_TextureRamp;
    public float m_RampOffset;
    public float m_RedPower;
    public float m_RedDelta;

    public Material m_Mat;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        m_Mat.SetTexture("_RampTex", m_TextureRamp);
        m_Mat.SetFloat("_RampOffset", m_RampOffset);
        m_Mat.SetFloat("_RedPower", m_RedPower);
        m_Mat.SetFloat("_RedDelta", m_RedDelta);

        Graphics.Blit(src, dest, m_Mat);
    }
}
