//http://wiki.unity3d.com/index.php?title=FadeInOut
using UnityEngine;

public class FadeToBlack : MonoBehaviour
{
    public delegate void FadeFinishedCallback();

    private GUIStyle m_BackgroundStyle = new GUIStyle();
    private Texture2D m_FadeTexture;
    private Color m_CurrentScreenOverlayColor = new Color(0, 0, 0, 0);
    private Color m_TargetScreenOverlayColor = new Color(0, 0, 0, 0);
    private Color m_DeltaColor = new Color(0, 0, 0, 0);
    private int m_FadeGUIDepth = -1000;
    private bool m_Fading;
    private FadeFinishedCallback m_CurrentCallback;

    private void Awake()
    {
        m_FadeTexture = new Texture2D(1, 1);
        m_BackgroundStyle.normal.background = m_FadeTexture;
        SetScreenOverlayColor(m_CurrentScreenOverlayColor);
    }

    private void OnGUI()
    {
        if (m_CurrentScreenOverlayColor != m_TargetScreenOverlayColor)
        {
            if (Mathf.Abs(m_CurrentScreenOverlayColor.a - m_TargetScreenOverlayColor.a) < Mathf.Abs(m_DeltaColor.a) * Time.deltaTime)
            {
                m_CurrentScreenOverlayColor = m_TargetScreenOverlayColor;
                SetScreenOverlayColor(m_CurrentScreenOverlayColor);
                m_DeltaColor = new Color(0, 0, 0, 0);
            }
            else
            {
                SetScreenOverlayColor(m_CurrentScreenOverlayColor + m_DeltaColor * Time.deltaTime);
            }
        }
        else if (m_Fading)
        {
            m_Fading = false;
            if (m_CurrentCallback != null)
            {
                m_CurrentCallback();
                m_CurrentCallback = null;
            }
        }

        if (m_CurrentScreenOverlayColor.a > 0)
        {
            GUI.depth = m_FadeGUIDepth;
            GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), m_FadeTexture, m_BackgroundStyle);
        }
    }

    public void SetScreenOverlayColor(Color newScreenOverlayColor)
    {
        m_CurrentScreenOverlayColor = newScreenOverlayColor;
        m_FadeTexture.SetPixel(0, 0, m_CurrentScreenOverlayColor);
        m_FadeTexture.Apply();
    }

    public void StartFade(Color newScreenOverlayColor, float fadeDuration, FadeFinishedCallback callback = null)
    {
        m_CurrentCallback = callback;
        m_Fading = true;
        if (fadeDuration <= 0.0f)
        {
            SetScreenOverlayColor(newScreenOverlayColor);
        }
        else
        {
            m_TargetScreenOverlayColor = newScreenOverlayColor;
            m_DeltaColor = (m_TargetScreenOverlayColor - m_CurrentScreenOverlayColor) / fadeDuration;
        }
    }
}