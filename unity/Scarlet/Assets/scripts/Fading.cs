using UnityEngine;
using System.Collections;

public class Fading : MonoBehaviour {
    public Texture2D m_FadeTextureBlack;

    public float m_FadeSpeed = 0.8f;
    private float m_Alpha = 1.0f;

    private int m_DrawDepth = -1000;
    private int m_FadeDirection = -1;

    void OnGUI()
    {
        m_Alpha += m_FadeDirection * m_FadeSpeed * Time.deltaTime;
        m_Alpha = Mathf.Clamp01(m_Alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, m_Alpha);
        GUI.depth = m_DrawDepth;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_FadeTextureBlack);
    }

    public float BeginFade(int direction)
    {
        m_FadeDirection = direction;

        return m_FadeSpeed;
    } 

    void OnLevelWasLoaded()
    {
        BeginFade(-1);
    }

    public void SetFadeSpeed(float speed)
    {
        m_FadeSpeed = speed;
    }
}
